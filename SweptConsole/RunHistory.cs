//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.Linq;

namespace swept
{
    public class FileTasks : Dictionary<SourceFile, ClauseMatch> { }
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

            case RuleFailOn.Over:
                threshold = rule.RunFailOverLimit;
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

    }

}
