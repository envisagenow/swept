//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2012 Jason Cole and Envisage Technologies Corp.
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
                execute( args, Console.Out );
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine( ex.Message + "\nStack trace:\n" + ex.StackTrace );
            }
        }

        //private static void execute( string[] args, TextWriter fullReportWriter, TextWriter failureReportWriter )
        private static void execute( string[] args, TextWriter fullReportWriter )
        {
            IStorageAdapter storage = new StorageAdapter();

            var arguments = new Arguments( args, storage, fullReportWriter );
            if (arguments.AreInvalid)
                return;

            EventSwitchboard switchboard = new EventSwitchboard();
            ProjectLibrarian librarian = new ProjectLibrarian( storage, switchboard );

            Subscriber subscriber = new Subscriber();
            subscriber.Subscribe( switchboard, librarian );   // FIX:  finish exception subscribing

            var traverser = new Traverser( arguments, storage );
            IEnumerable<string> fileNames = traverser.GetProjectFiles();

            librarian.OpenLibrary( arguments.Library );

            var changes = librarian.GetSortedChanges();

            var gatherer = new Gatherer( changes, fileNames, storage );

            var results = gatherer.GetMatchesPerChange();

            var buildReporter = new BuildReporter();
            var reportXML = buildReporter.ReportOn( results );

            fullReportWriter.WriteLine( reportXML );

            int failureCode = 0;

            FailChecker checker = new FailChecker();
            var failures = checker.Check( results );
            if (failures.Any())
            {
                var message = buildReporter.ReportBuildFailures( failures );
                Console.Error.WriteLine( message );

                failureCode = 20;
            }

            //TODO goal code:
            //var reportXML = buildReporter.ReportOn( results, failures );

            Environment.Exit( failureCode );
        }
    }
}
