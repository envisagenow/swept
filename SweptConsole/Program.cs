//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace swept
{
    class Program
    {
        static void Main( string[] args )
        {
            try
            {
                execute( args, new StorageAdapter(), DateTime.Now );
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine( ex.Message + "\nStack trace:\n" + ex.StackTrace );
                Environment.Exit( 20 );
            }
        }

        private static void execute( string[] args, IStorageAdapter storage, DateTime startTime )
        {
            var arguments = new Arguments( args, storage );
            if (arguments.AreInvalid)
                return;

            int failureCode = 0;

            EventSwitchboard switchboard = new EventSwitchboard();
            ProjectLibrarian librarian = new ProjectLibrarian( storage, switchboard );

            Subscriber subscriber = new Subscriber();
            subscriber.Subscribe( switchboard, librarian );
            // TODO: subscriber.SubscribeExceptions( switchboard, this );

            var traverser = new Traverser( arguments, storage );
            var files = traverser.GetFilesToScan();

            librarian.OpenLibrary( arguments.Library );
            var rules = librarian.GetSortedRules();

            var gatherer = new Gatherer( rules, files, storage );
            var ruleTasks = gatherer.GetRuleTasks();

            var buildLibrarian = new BuildLibrarian( arguments, storage );
            var runHistory = buildLibrarian.ReadRunHistory();

            string header = string.Empty;
            if (arguments.Check)
                header = String.Format( "Swept checking [{0}] with rules in [{1}] on {2}...{3}", storage.GetCWD(), arguments.Library, startTime.ToString( "G" ), Environment.NewLine );


            var inspector = new RunInspector( runHistory );
            RunHistoryEntry newRunEntry = inspector.GenerateEntry( startTime, ruleTasks );
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

            //if (arguments.SummaryFileName != string.Empty)
            //{
            using (TextWriter deltaWriter = storage.GetOutputWriter( arguments.DeltaFileName ))
            {
                var delta = new BreakageDelta { Failures = failures, Fixes = new List<string>() };
                //!!!#!#!#!#!#!!#!#!#!#!#!#!#!#!!#!#!#!##!#!!#!#!###!#!!!!#!#!#!#!##!#!

                var deltaXml = reporter.GenerateBuildDeltaXml( delta );
                deltaWriter.Write( deltaXml.ToString() );
                deltaWriter.Flush();
            }
            //}

            if (!newRunEntry.Passed)
            {
                Console.Out.WriteLine( failures );
                failureCode = 10;
            }

            Environment.Exit( failureCode );
        }
    }
}
