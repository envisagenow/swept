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
            else if (mostRecentlyPassed.RuleResults.ContainsKey( ruleID ))
            {
                return mostRecentlyPassed.RuleResults[ruleID].Violations;
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
