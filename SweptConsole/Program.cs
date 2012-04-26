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
                execute( args, new StorageAdapter(), Console.Out, Console.Error );
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine( ex.Message + "\nStack trace:\n" + ex.StackTrace );
                Environment.Exit( 20 );
            }
        }

        private static void execute( string[] args, IStorageAdapter storage, TextWriter reportWriter, TextWriter errorWriter )
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

            var changes = librarian.GetSortedChanges();

            var gatherer = new Gatherer( changes, fileNames, storage );

            var results = gatherer.GetMatchesPerChange();

            var buildReporter = new BuildReporter( storage );
            var reportXML = buildReporter.ReportOn( results );
            reportWriter.WriteLine( reportXML );

            //todo Run history looks like:
            //buildReporter.WriteRunHistory( runHistory );

            int failureCode = 0;

            FailChecker checker = new FailChecker();
            var failures = checker.Check( results );
            if (failures.Any())
            {
                var message = buildReporter.ReportBuildFailures( failures );
                errorWriter.WriteLine( message );

                failureCode = 10;
            }

            Environment.Exit( failureCode );
        }
    }
}
