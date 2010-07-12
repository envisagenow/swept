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

            var arguments = new Arguments( args, starter.StorageAdapter, writer );
            if (arguments.AreInvalid)
                return;

            var traverser = new Traverser( arguments, starter.StorageAdapter );
            IEnumerable<string> fileNames = traverser.GetProjectFiles();

            var changes = starter.ChangeCatalog.GetSortedChanges();

            var gatherer = new Gatherer( changes, fileNames, starter.StorageAdapter );
            
            Dictionary<Change, List<IssueSet>> results = gatherer.GetIssueSetDictionary();

            var buildReporter = new BuildReporter();
            var reportXML = buildReporter.ReportOn( results );

            writer.WriteLine( reportXML );
        }
    }
}
