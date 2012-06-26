//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.Linq;
namespace swept
{

    public class FileProblems : Dictionary<SourceFile, ClauseMatch> { }

    public class RunHistory
    {
        public const int HighWaterLine = int.MaxValue;

        public int NextRunNumber
        {
            get
            {
                if (Runs.Count == 0) return 1;
                else return Runs.Max( r => r.Number )  + 1;
            }
        }

        public List<RunHistoryEntry> Runs;

        public void AddRun( RunHistoryEntry run )
        {
            Runs.Add( run );
        }

        public RunHistory()
        {
            Runs = new List<RunHistoryEntry>();
        }

        public RunHistoryEntry GenerateEntry( Dictionary<Rule, FileProblems> ruleViolations, DateTime runDateTime )
        {
            // TODO: when I make this work, have a failing test first.  Maybe then I won't need to rewrite it again, eh?
            //HAIRYTODO:  Make this work again very soon.
            //var failures = checker.Check( ruleViolations );
            var entry = new RunHistoryEntry { Passed = true }; // { Passed = (failures.Count == 0) };
            entry.Number = NextRunNumber;
            entry.Date = runDateTime;

            foreach( var keyRule in ruleViolations.Keys )
            {
                entry.Violations[keyRule.ID] = countViolations( ruleViolations[keyRule] );
            }

            return entry;
        }

        public int WaterlineFor( string ruleID )
        {
            RunHistoryEntry mostRecentlyPassed = null;

            foreach( var run in Runs )
            {
                if (run.Passed)
                    mostRecentlyPassed = run;
            }

            if (mostRecentlyPassed == null)
            {
                return HighWaterLine;
            }
            else if (mostRecentlyPassed.Violations.ContainsKey( ruleID ))
            {
                return mostRecentlyPassed.Violations[ruleID];
            }
            else
            {
                return HighWaterLine;
            }
        }

        private int countViolations( FileProblems fileProblems )
        {
            int count = 0;
            foreach (SourceFile file in fileProblems.Keys)
            {
                count += fileProblems[file].Count;
            }
            return count;
        }

    }

}
