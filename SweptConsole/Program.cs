//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;

namespace swept
{
    class Program
    {
        static void Main( string[] args )
        {
            try
            {
                execute( args );
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine( ex.Message + "\nStack trace:\n" + ex.StackTrace );
            }
        }

        private static void execute( string[] args )
        {
            //string[] fs = System.IO.Directory.GetFiles( Environment.CurrentDirectory, "*.(cs|html)", System.IO.SearchOption.AllDirectories );
            //Console.WriteLine( "found " + fs.Length + " files" );

            Starter starter = new Starter();
            starter.Start();

            var arguments = new Arguments( args, starter.StorageAdapter, Console.Out );
            if (arguments.AreInvalid)
                return;

            var traverser = new Traverser( arguments, starter.StorageAdapter );
            var files = traverser.GetProjectFiles();

            var changes = starter.ChangeCatalog.GetSortedChanges();

            var gatherer = new Gatherer( changes, files, starter.StorageAdapter );
            var results = gatherer.GetIssueList();

            var buildReporter = new BuildReporter();
            var reportXML = buildReporter.ReportOn( results );

            Console.Out.WriteLine( reportXML );
        }
    }
}
