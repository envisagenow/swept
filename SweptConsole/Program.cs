//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2011 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.IO;

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

        private static void execute( string[] args, TextWriter writer )
        {
            IStorageAdapter storage = new StorageAdapter();

            var arguments = new Arguments( args, storage, writer );
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

            Dictionary<Change, Dictionary<SourceFile, ClauseMatch>> results = gatherer.GetMatchesPerChange();

            var buildReporter = new BuildReporter();
            var reportXML = buildReporter.ReportOn( results );

            writer.WriteLine( reportXML );
        }
    }
}
