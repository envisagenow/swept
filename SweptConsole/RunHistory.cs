//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.Linq;

namespace swept
{
    public class RunHistory
    {
        public const int HighWaterLine = 4000;

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

        public RunHistoryEntry GenerateEntry( Dictionary<Rule, Dictionary<SourceFile, ClauseMatch>> ruleViolations, DateTime runDateTime )
        {
            var checker = new FailChecker( this );
            var failures = checker.Check( ruleViolations );

            var entry = new RunHistoryEntry { Passed = (failures.Count == 0) };
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

        private int countViolations( Dictionary<SourceFile, ClauseMatch> problemsPerFile )
        {
            int count = 0;
            foreach (SourceFile source in problemsPerFile.Keys)
            {
                count += problemsPerFile[source].Count;
            }
            return count;
        }

    }

}
