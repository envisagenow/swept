//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace swept
{
    public class FailChecker
    {
        public List<string> Check( Dictionary<Change, Dictionary<SourceFile, ClauseMatch>> changeViolations )
        {
            var failures = new List<string>();

            foreach (var change in changeViolations.Keys)
            {
                int violationCount = 0;
                foreach (var sourcefile in changeViolations[change].Keys)
                {
                    violationCount += changeViolations[change][sourcefile].Count;
                }

                if (change.BuildFail == BuildFailMode.Any && violationCount > 0)
                {
                    string failureText = string.Format( "Rule [{0}] has been violated, and it breaks the build if there are any violations.", change.ID );
                    failures.Add( failureText );
                }
                else if (change.BuildFail == BuildFailMode.Over && violationCount > change.BuildFailOverLimit)
                {
                    string failureText = string.Format( "Rule [{0}] has been violated [{1}] times, and it breaks the build if there are over [{2}] violations.", change.ID, violationCount, change.BuildFailOverLimit );
                    failures.Add( failureText );
                }
            }

            return failures;
        }

        public string GetBuildHistory( Dictionary<Change, Dictionary<SourceFile, ClauseMatch>> changeViolations )
        {
            XDocument buildHistory = new XDocument();
            XElement historyRoot = new XElement( "BuildHistory" );

            XElement build = new XElement( "Build" );
            build.Add( new XAttribute( "Number", "3403" ) );
            build.Add( new XAttribute( "DateTime", "4/4/2012 10:25 AM" ) );

            foreach (Change change in changeViolations.Keys)
            {
                XAttribute changeID = new XAttribute( "ID", change.ID );
                XElement changeElement = new XElement( "Change" );
                changeElement.Add( changeID );
                changeElement.Add( new XAttribute( "Violations", TotalProblems(changeViolations[change]) ) );
                build.Add( changeElement );
            }

            historyRoot.Add( build );

            buildHistory.Add( historyRoot );
                        
            return buildHistory.ToString();
        }
        private int TotalProblems( Dictionary<SourceFile, ClauseMatch> problemsPerFile )
        {
            int totalProblemCount = 0;
            foreach (SourceFile source in problemsPerFile.Keys)
            {
                var clauseMatch = problemsPerFile[source];
                var theseProblemsCount = clauseMatch.Count;
                 totalProblemCount += theseProblemsCount;

            }
            return totalProblemCount;
        }
    }
}
