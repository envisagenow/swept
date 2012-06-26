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
                execute( args, new StorageAdapter(), Console.Out, Console.Error, DateTime.Now );
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine( ex.Message + "\nStack trace:\n" + ex.StackTrace );
                Environment.Exit( 20 );
            }
        }

        private static void execute( string[] args, IStorageAdapter storage, TextWriter reportWriter, TextWriter errorWriter, DateTime startTime )
        {
            var arguments = new Arguments( args, storage, reportWriter );
            if (arguments.AreInvalid)
                return;

            EventSwitchboard switchboard = new EventSwitchboard();
            ProjectLibrarian librarian = new ProjectLibrarian( storage, switchboard );

            Subscriber subscriber = new Subscriber();
            subscriber.Subscribe( switchboard, librarian );
            // TODO: subscriber.SubscribeExceptions( switchboard, this );

            var traverser = new Traverser( arguments, storage );
            IEnumerable<string> fileNames = traverser.GetProjectFiles();

            librarian.OpenLibrary( arguments.Library );

            var rules = librarian.GetSortedRules();

            var gatherer = new Gatherer( rules, fileNames, storage );

            var results = gatherer.GetMatchesPerRule();

            var buildLibrarian = new BuildLibrarian( arguments, storage );

            var runHistory = buildLibrarian.ReadRunHistory();

            if (arguments.Check)
            {
                string checkMessage = String.Format( "Swept checking [{0}] with rules in [{1}] on {2}...", "s:\\code\\swept", "swept.2010.swept.library", "6/26/2012 10:58 AM" );
                reportWriter.WriteLine( checkMessage );
            }
            {
                var reportXML = buildLibrarian.ReportOn( results, runHistory );
                reportWriter.WriteLine( reportXML );
            }
            var newRun = runHistory.GenerateEntry( results, startTime );
            runHistory.AddRun( newRun );

            int failureCode = 0;

            var failures = buildLibrarian.Check( results, runHistory );
            if (failures.Any())
            {
                var message = buildLibrarian.ReportBuildFailures( failures );
                errorWriter.WriteLine( message );

                failureCode = 10;
            }

            buildLibrarian.WriteRunHistory( runHistory );

            Environment.Exit( failureCode );
        }
    }
}
