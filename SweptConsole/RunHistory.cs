//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2013 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.Linq;

namespace swept
{
    //  FileTasks collects a set of SourceFiles and where they need changes.
    //  This 'where' is used per rule by the system, but is not technically 
    //  tied to a single rule, so might be used in other ways.
    public class FileTasks : Dictionary<SourceFile, ClauseMatch>
    {
        public int CountTasks()
        {
            return Keys.Sum( file => this[file].Count );
        }
    }

    //  RuleTasks collects a set of rules and the FileTasks pertaining to them.
    //  One RuleTasks object will hold all the problems Swept finds in a run.
    public class RuleTasks : Dictionary<Rule, FileTasks> { }

    public class RunHistory
    {
        public const int HighWaterLine = int.MaxValue;

        private List<RunEntry> _Runs;
        public IEnumerable<RunEntry> Runs { get { return _Runs; } }
        public RunEntry LatestPassingRun { get; private set; }

        public RunHistory()
        {
            _Runs = new List<RunEntry>();
        }


        public int NextRunNumber
        {
            get
            {
                if (Runs.Count() == 0) return 1;
                else return Runs.Max( r => r.Number ) + 1;
            }
        }

        public void AddEntry( RunEntry run )
        {
            _Runs.Add( run );

            if (run.Passed)
            {
                if (LatestPassingRun == null || LatestPassingRun.Number < run.Number)
                    LatestPassingRun = run;
            }
        }

        public int GetThreshold( Rule rule )
        {
            if (rule.FailOn == RuleFailOn.Increase)
                return WaterlineFor(rule.ID);
            else
            {
                // TODO: somewhat smelly!
                var result = new RuleResult { FailOn = rule.FailOn };
                return result.Threshold;
            }
        }

        public int WaterlineFor( string ruleID )
        {
            RunEntry targetRun = Runs.LastOrDefault();
            if (targetRun == null)
               return HighWaterLine;
            
            return targetRun.WaterlineFor(ruleID);
        }

    }
}
