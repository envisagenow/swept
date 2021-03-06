﻿//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2015 Jason Cole and Envisage Technologies Corp.
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
        static void Main(string[] args)
        {
            int exitCode = 0;
            try
            {
                exitCode = execute(args, new StorageAdapter(), DateTime.Now);
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(ex.Message + "\nStack trace:\n" + ex.StackTrace);
                exitCode = 20;
            }
            Environment.Exit(exitCode);
        }

        private static int execute(string[] args, IStorageAdapter storage, DateTime startTime)
        {
            var arguments = new Arguments(args, storage);

            if (arguments.ShowUsage)
            {
                Console.Out.WriteLine(Arguments.UsageMessage);
                return 0;
            }

            if (arguments.AreInvalid)
            {
                Console.Out.WriteLine("Swept does not understand the given arguments.  Please correct them.");
                return 8;
            }

            int exitCode = 0;

            EventSwitchboard switchboard = new EventSwitchboard();
            ProjectLibrarian librarian = new ProjectLibrarian(storage, switchboard);

            Subscriber subscriber = new Subscriber();
            subscriber.Subscribe(switchboard, librarian);

            librarian.OpenLibrary(arguments.Library);

            if (!string.IsNullOrEmpty(arguments.Show))
            {
                var rulesToShow = librarian.ShowRules(arguments.Show);
                foreach (var rule in rulesToShow)
                    Console.Out.WriteLine(rule);

                if (rulesToShow.Count == 0)
                    Console.Out.WriteLine("No rules match [{0}].", arguments.Show);

                return 0;
            }

            var rules = librarian.GetSortedRules(arguments.SpecifiedRules, arguments.Picks, arguments.AdHoc);

            arguments.FillExclusions(librarian.GetExcludedFolders());

            var traverser = new Traverser(arguments, storage);
            var files = traverser.GetFilesToScan();


            var gatherer = new Gatherer(rules, arguments.Folder, files, storage);
            var ruleTasks = gatherer.GetRuleTasks();

            var buildLibrarian = new BuildLibrarian(arguments, storage);
            var runHistory = buildLibrarian.ReadRunHistory();

            string header = string.Empty;
            if (arguments.Check)
                header = String.Format(
                    "Swept checking [{0}] with rules in [{1}] on {2}...{3}",
                    storage.GetCWD(), arguments.Library, startTime.ToString("G"), Environment.NewLine);

            var inspector = new RunInspector(runHistory);
            RunEntry newRunEntry = inspector.GenerateEntry(startTime, ruleTasks);

            XElement deltaXml = null;
            if (!string.IsNullOrEmpty(arguments.DeltaFileName))
            {
                //  The delta must be generated before adding the new entry to the history,
                //  in case the new entry is passing, which would make it the .LatestPassingRun, 
                //  making the delta empty.
                deltaXml = inspector.GenerateDeltaXml(newRunEntry);
                //  Todo:  Alter the GenerateDeltaXml to presume the NewRunEntry is in the history,
                //  and create a .LatestPassingBefore( newRunEntry ).
                //  That untangles this special sequencing and the report moves down.
            }

            if (arguments.TeamCity)
            {
                inspector.GenerateDeltaTeamCityOutput(Console.Out, newRunEntry);
            }

            runHistory.AddEntry(newRunEntry);
            var failures = inspector.ListRunFailureMessages(newRunEntry);

            BuildReporter reporter = new BuildReporter();

            string detailReport;
            if (arguments.Check)
                detailReport = reporter.ReportCheckResult(failures);
            else
                detailReport = reporter.ReportDetailsXml(ruleTasks, arguments.FileCountLimit, runHistory.NextRunNumber, arguments.Picks);

            // TODO:  Untangle these
            buildLibrarian.WriteRunHistory(runHistory);

            if (!arguments.Foresight)
            {
                using (TextWriter detailWriter = storage.GetOutputWriter(arguments.DetailsFileName))
                {
                    detailWriter.Write(header);
                    detailWriter.WriteLine(detailReport);
                    detailWriter.Flush();
                }
            }

            if (!string.IsNullOrEmpty(arguments.DeltaFileName))
            {
                using (TextWriter deltaWriter = storage.GetOutputWriter(arguments.DeltaFileName))
                {
                    deltaWriter.Write(deltaXml.ToString());
                    deltaWriter.Flush();
                }
            }

            if (!string.IsNullOrEmpty(arguments.ChangesFileName))
            {
                RunChanges oldChanges = buildLibrarian.ReadRunChanges();
                RunChanges newChanges = oldChanges.InitializeNextRun();
                newChanges.Rules = oldChanges.Rules;

                newChanges.AddRuleTasks(ruleTasks, startTime);

                var changesXml = buildLibrarian.BuildRunChangesDoc(newChanges, rules);
                using (TextWriter writer = storage.GetOutputWriter(arguments.ChangesFileName))
                {
                    writer.Write(changesXml.ToString());
                    writer.Flush();
                }

                if (arguments.Foresight)
                {
                    string foresight = newChanges.ForesightReport();
                    Console.Out.Write(foresight);
                }
            }

            if (!newRunEntry.Passed)
            {
                foreach (string failure in failures)
                    Console.Out.WriteLine(failure);

                if (arguments.BreakOnDeltaDrop)
                    exitCode = 10;
            }

            return exitCode;
        }
    }
}
