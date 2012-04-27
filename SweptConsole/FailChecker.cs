//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace swept
{
    public class FailChecker
    {
        public RunHistory History { get; set; }

        public List<string> Check( Dictionary<Change, Dictionary<SourceFile, ClauseMatch>> changeViolations )
        {
            var failures = new List<string>();

            foreach (var change in changeViolations.Keys)
            {
                int count = countViolations( changeViolations[change] );

                int threshold;
                switch (change.RunFail)
                {
                case RunFailMode.Any:
                    threshold = 0;
                    break;

                case RunFailMode.Over:
                    threshold = change.RunFailOverLimit;
                    break;

                case RunFailMode.Increase:
                    threshold = History.WaterlineFor( change.ID );
                    break;

                case RunFailMode.None:
                    threshold = 4000;
                    break;

                default:
                    throw new Exception( String.Format( "I do not know how to check a failure mode of [{0}].  Please extend FailChecker.Check.", change.RunFail ) );
                }

                string thresholdPhrase = (threshold == 0) ?
                    "any"
                    : "over [" + threshold + "]";

                if (count > threshold)
                {
                    string failureText = string.Format( "Rule [{0}] has been violated [{1}] times, and it breaks the build if there are {2} violations.", change.ID, count, thresholdPhrase );
                    failures.Add( failureText );
                }

            }

            return failures;
        }

        //public string GetRunHistory( Dictionary<Change, Dictionary<SourceFile, ClauseMatch>> changeViolations, DateTime runDateTime, int runNumber )
        //{
        //    XDocument historyDoc = new XDocument();
        //    XElement runHistory = new XElement( "RunHistory" );

        //    XElement run = new XElement( "Run" );
        //    run.Add( new XAttribute( "Number", runNumber ) );
        //    run.Add( new XAttribute( "DateTime", runDateTime.ToString() ) );

        //    foreach (Change change in changeViolations.Keys)
        //    {
        //        XAttribute changeID = new XAttribute( "ID", change.ID );
        //        XElement changeElement = new XElement( "Change" );
        //        changeElement.Add( changeID );
        //        changeElement.Add( new XAttribute( "Violations", countViolations( changeViolations[change] ) ) );
        //        run.Add( changeElement );
        //    }

        //    runHistory.Add( run );

        //    historyDoc.Add( runHistory );

        //    return historyDoc.ToString();
        //}

        //todo, it's coming out of here
        private int countViolations( Dictionary<SourceFile, ClauseMatch> problemsPerFile )
        {
            int count = 0;
            foreach (SourceFile source in problemsPerFile.Keys)
            {
                count += problemsPerFile[source].Count;
            }
            return count;
        }

        public RunHistory ReadRunHistory( XDocument historyXml )
        {
            RunHistory runHistory = new RunHistory();

            foreach (var runXml in historyXml.Descendants( "Run" ))
            {
                RunHistoryEntry run = new RunHistoryEntry();

                run.Number = int.Parse( runXml.Attribute( "Number" ).Value );
                run.Date = DateTime.Parse( runXml.Attribute( "DateTime" ).Value );

                foreach (var changeXml in runXml.Descendants( "Change" ))
                {
                    string changeID = changeXml.Attribute( "ID" ).Value;

                    int changeViolations = int.Parse( changeXml.Attribute( "Violations" ).Value );
                    run.Violations.Add( changeID, changeViolations );
                }

                runHistory.Runs.Add( run );
            }

            return runHistory;
        }
    }
}
