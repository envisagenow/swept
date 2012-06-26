//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.Linq;

namespace swept
{
    public class FailChecker
    {
        public readonly RunHistory History;
        public FailChecker( RunHistory history )
        {
            History = history;
        }


        public List<string> Check( Dictionary<Rule, FileProblems> ruleFileProblems )
        {
            var failures = new List<string>();

            foreach (var rule in ruleFileProblems.Keys)
            {
                int count = countViolations( ruleFileProblems[rule] );

                int threshold;
                switch (rule.RunFail)
                {
                case RunFailMode.Any:
                    threshold = 0;
                    break;

                case RunFailMode.Over:
                    threshold = rule.RunFailOverLimit;
                    break;

                case RunFailMode.Increase:
                    threshold = History.WaterlineFor( rule.ID );
                    break;

                case RunFailMode.None:
                    threshold = int.MaxValue;
                    break;

                default:
                    throw new Exception( String.Format( "I do not know how to check a failure mode of [{0}].  Please extend FailChecker.Check.", rule.RunFail ) );
                }

                string thresholdPhrase = (threshold == 0) ? "any" : "over [" + threshold + "]";

                if (count > threshold)
                {
                    string failureText = string.Format( "Rule [{0}] has been violated [{1}] times, and it breaks the build if there are {2} violations.", rule.ID, count, thresholdPhrase );
                    failures.Add( failureText );
                }

            }

            return failures;
        }

        //todo, move to a more general purpose location
        private int countViolations( FileProblems fileProblems )
        {
            int count = 0;
            foreach (SourceFile source in fileProblems.Keys)
            {
                count += fileProblems[source].Count;
            }
            return count;
        }
    }
}
