//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
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
            Starter starter = new Starter();
            starter.Start();
            ProjectLibrarian librarian = starter.Librarian;
            IStorageAdapter storageAdapter = starter.StorageAdapter;

            var arguments = new Arguments( args, storageAdapter, writer );
            if (arguments.AreInvalid)
                return;

            var traverser = new Traverser( arguments, storageAdapter );
            IEnumerable<string> fileNames = traverser.GetProjectFiles();

            librarian.OpenLibrary( arguments.Library );

            var changes = librarian.GetSortedChanges();

            var gatherer = new Gatherer( changes, fileNames, storageAdapter );

            Dictionary<Change, Dictionary<SourceFile, ClauseMatch>> results = gatherer.GetMatchesPerChange();

            var buildReporter = new BuildReporter();
            var reportXML = buildReporter.ReportOn( results );

            writer.WriteLine( reportXML );
        }
    }
}
