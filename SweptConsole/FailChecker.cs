//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.Linq;

namespace swept
{
    public class FailChecker
    {
        public List<string> Check( Dictionary<Change, Dictionary<SourceFile, ClauseMatch>> changeViolations )
        {
            var failures = new List<string>();

            foreach (var change in changeViolations.Keys)
            {
                if (change.BuildFail == BuildFailMode.Any)
                {
                    string failureText = string.Format( "Rule [{0}] has been violated, and it breaks the build if there are any violations.", change.ID );
                    failures.Add( failureText );
                }
                else if (change.BuildFail == BuildFailMode.Over)
                {
                    int violationCount = 0;
                    foreach (var sourcefile in changeViolations[change].Keys)
                    {
                        violationCount += changeViolations[change][sourcefile].Count;
                    }
                    if (violationCount > change.BuildFailOverLimit)
                    {
                        string failureText = string.Format( "Rule [{0}] has been violated [{1}] times, and it breaks the build if there are over [{2}] violations.", change.ID, violationCount, change.BuildFailOverLimit );
                        failures.Add( failureText );
                    }
                }
            }

            return failures;
        }

        public string ReportFailures( List<string> failures )
        {
            if (failures.Count == 0)
                return string.Empty;
            else
            {
                string failureMessage = "Swept failed due to build breaking rule failure{0}:\n{1}";
                string failuresText = "";
                foreach (string fail in failures)
                {
                    failuresText += fail + "\n";
                }

                var plurality = failures.Count > 1? "s" : "";

                return string.Format( failureMessage, plurality, failuresText );
            }
        }

        public string ReportFailureXML( List<string> failures )
        {
            if (failures.Count == 0)
                return string.Empty;
            else
            {
                string failureXML = string.Format( "<{0}s><{0}>{1}</{0}></{0}s>", "SweptRuleFailure", failures[0] );
                return failureXML;
            }
        }
    }
}
