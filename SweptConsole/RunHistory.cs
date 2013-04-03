//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2012 Jason Cole and Envisage Technologies Corp.
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

        private List<RunHistoryEntry> _Runs;
        public IEnumerable<RunHistoryEntry> Runs { get { return _Runs; } }
        public RunHistoryEntry LatestPassingRun { get; private set; }

        public RunHistory()
        {
            _Runs = new List<RunHistoryEntry>();
        }


        public int NextRunNumber
        {
            get
            {
                if (Runs.Count() == 0) return 1;
                else return Runs.Max( r => r.Number )  + 1;
            }
        }

        public void AddEntry( RunHistoryEntry run )
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
            int threshold;
            switch (rule.FailOn)
            {
            case RuleFailOn.Any:
                threshold = 0;
                break;

            case RuleFailOn.Increase:
                threshold = WaterlineFor( rule.ID );
                break;

            case RuleFailOn.None:
                threshold = int.MaxValue;
                break;

            default:
                System.Reflection.MethodBase thisMethod = new System.Diagnostics.StackTrace().GetFrame( 0 ).GetMethod();
                throw new Exception( String.Format( "I do not know how to check a failure mode of [{0}].  Please extend {1}.{2}.",
                    rule.FailOn, thisMethod.ReflectedType, thisMethod.Name ) );
            }

            return threshold;
        }

        public int WaterlineFor( string ruleID )
        {
            if (LatestPassingRun != null && LatestPassingRun.RuleResults.ContainsKey( ruleID ))
                return LatestPassingRun.RuleResults[ruleID].TaskCount;

            return HighWaterLine;
        }

    }
}
