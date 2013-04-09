//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace swept
{
    class Program
    {
        static void Main( string[] args )
        {
            int exitCode = 0;
            try
            {
                exitCode = execute( args, new StorageAdapter(), DateTime.Now );
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine( ex.Message + "\nStack trace:\n" + ex.StackTrace );
                exitCode = 20;
            }
            Environment.Exit( exitCode );
        }

        private static int execute( string[] args, IStorageAdapter storage, DateTime startTime )
        {
            var arguments = new Arguments( args, storage );

            if (arguments.ShowUsage)
                Console.Out.WriteLine( Arguments.UsageMessage );

            if (arguments.AreInvalid)
                return 8;

            int exitCode = 0;

            EventSwitchboard switchboard = new EventSwitchboard();
            ProjectLibrarian librarian = new ProjectLibrarian( storage, switchboard );

            Subscriber subscriber = new Subscriber();
            subscriber.Subscribe( switchboard, librarian );
            // TODO: subscriber.SubscribeExceptions( switchboard, this );

            librarian.OpenLibrary( arguments.Library );
            var rules = librarian.GetSortedRules();

            // Goal code:  var traverser = new Traverser( storage, arguments.Folder, librarian.ExcludedFolders );
            var traverser = new Traverser( arguments, storage );
            var files = traverser.GetFilesToScan();

           

            var gatherer = new Gatherer( rules, files, storage );
            var ruleTasks = gatherer.GetRuleTasks();

            var buildLibrarian = new BuildLibrarian( arguments, storage );
            var runHistory = buildLibrarian.ReadRunHistory();

            string header = string.Empty;
            if (arguments.Check)
                header = String.Format( "Swept checking [{0}] with rules in [{1}] on {2}...{3}", storage.GetCWD(), arguments.Library, startTime.ToString( "G" ), Environment.NewLine );


            var inspector = new RunInspector( runHistory );
            RunHistoryEntry newRunEntry = inspector.GenerateEntry( startTime, ruleTasks );

            XElement deltaXml = null;
            if (!string.IsNullOrEmpty( arguments.DeltaFileName ))
            {
                //  The delta should be generated before adding the new entry to the history,
                //  because if the new entry is passing, it will become the .LatestPassingRun, 
                //  making the delta empty.
                deltaXml = inspector.GenerateDeltaXml( newRunEntry );
            }

            runHistory.AddEntry( newRunEntry );
            var failures = inspector.ListRunFailureMessages( newRunEntry );

            BuildReporter reporter = new BuildReporter();

            string detailReport;
            if (arguments.Check)
                detailReport = reporter.ReportCheckResult( failures );
            else
                detailReport = reporter.ReportDetailsXml( ruleTasks );


            buildLibrarian.WriteRunHistory( runHistory );

            using (TextWriter detailWriter = storage.GetOutputWriter( arguments.DetailsFileName ))
            {
                detailWriter.Write( header );
                detailWriter.WriteLine( detailReport );
                detailWriter.Flush();
            }

            if (!string.IsNullOrEmpty( arguments.DeltaFileName ))
            {
                using (TextWriter deltaWriter = storage.GetOutputWriter( arguments.DeltaFileName ))
                {
                    deltaWriter.Write( deltaXml.ToString() );
                    deltaWriter.Flush();
                }
            }

            if (!newRunEntry.Passed)
            {
                foreach( string failure in failures )
                    Console.Out.WriteLine( failure );
                exitCode = 10;
            }

            return exitCode;
        }
    }
}
