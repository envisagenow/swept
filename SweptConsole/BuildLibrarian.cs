//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace swept
{
    public class BuildLibrarian
    {
        private readonly IStorageAdapter _storage;
        private readonly Arguments _args;

        public BuildLibrarian( Arguments args, IStorageAdapter storage )
        {
            _args = args;
            _storage = storage;
        }

        public string ReportOn( Dictionary<Rule, FileProblems> filesPerRule, RunHistory runHistory )
        {
            if (_args.Check)
                return ReportCheckResult( filesPerRule, runHistory );
            else
                return ReportXmlResult( filesPerRule );
        }

        public string ReportCheckResult(Dictionary<Rule, FileProblems> filesPerRule, RunHistory runHistory)
        {
            List<string> problemLines = ListRunFailures(filesPerRule, runHistory);

            if (!problemLines.Any())
                return "Swept check passed!\r\n";

            var sb = new StringBuilder();

            if (problemLines.Count > 0)
            {
                foreach (var line in problemLines)
                {
                    sb.AppendLine(line);
                }
            }
            else
            {
                sb.AppendLine("Swept check passed!");
            }

            return sb.ToString();
        }

        private static string ReportXmlResult( Dictionary<Rule, FileProblems> filesPerRule )
        {
            XDocument report_doc = new XDocument();
            XElement report_root = new XElement( "SweptBuildReport" );

            //  TODO: Get failure details into this XML report
            // Goal code:
            //  var failures_element = GetFailureReportElement( failures );
            //  report_root.Add( failures_element );

            int totalTasks = 0;
            foreach (Rule rule in filesPerRule.Keys.OrderBy( c => c.ID ))
            {
                int totalTasksPerRule = 0;

                var rule_element = new XElement( "Rule",
                    new XAttribute( "ID", rule.ID ),
                    new XAttribute( "Description", rule.Description )
                );

                var fileMatches = filesPerRule[rule];
                foreach (SourceFile file in fileMatches.Keys.OrderBy( file => file.Name ))
                {
                    var match = fileMatches[file];
                    var tasks = Task.FromMatch( match, rule, file );

                    if (tasks.Count == 0)
                        continue;

                    totalTasksPerRule += tasks.Count;

                    var file_tasks = new XElement( "SourceFile",
                        new XAttribute( "Name", file.Name ),
                        new XAttribute( "TaskCount", tasks.Count )
                    );
                    rule_element.Add( file_tasks );
                }

                rule_element.Add( new XAttribute( "TotalTasks", totalTasksPerRule ) );
                totalTasks += totalTasksPerRule;
                report_root.Add( rule_element );
            }

            report_root.Add( new XAttribute( "TotalTasks", totalTasks ) );
            report_doc.Add( report_root );
            return report_doc.ToString();
        }

        public string ReportBuildFailures( List<string> failures )
        {
            if (failures.Count == 0)
                return string.Empty;
            else
            {
                var plurality = failures.Count == 1 ? "" : "s";

                string failuresText = "";
                foreach (string fail in failures)
                {
                    failuresText += fail + "\n";
                }

                string failureMessage = "Swept failed due to build breaking rule failure{0}:\n{1}";
                return string.Format( failureMessage, plurality, failuresText );
            }
        }

        public XElement GenerateBuildFailureXML( List<string> failures )
        {
            XElement xml = new XElement( "SweptBuildFailures" );

            foreach (string fail in failures)
            {
                XElement xmlFailure = new XElement( "SweptBuildFailure", fail );
                xml.Add( xmlFailure );
            }

            return xml;
        }

        public RunHistory ReadRunHistory()
        {
            XDocument doc;
            try
            {
                doc = _storage.LoadRunHistory( _args.History );
            }
            catch (FileNotFoundException)
            {
                doc = new XDocument();
            }
            return ReadRunHistory( doc );
        }

        public RunHistory ReadRunHistory( XDocument historyXml )
        {
            RunHistory runHistory = new RunHistory();

            foreach (var runXml in historyXml.Descendants( "Run" ))
            {
                var run = new RunHistoryEntry {
                    Number = int.Parse(runXml.Attribute("Number").Value),
                    Date = DateTime.Parse(runXml.Attribute("DateTime").Value),
                    Passed = Boolean.Parse(runXml.Attribute("Passed").Value)
                };

                foreach (var ruleXml in runXml.Descendants( "Rule" ))
                {
                    string ruleID = ruleXml.Attribute( "ID" ).Value;

                    int ruleViolations = int.Parse( ruleXml.Attribute( "Violations" ).Value );
                    run.Violations.Add( ruleID, ruleViolations );
                }

                runHistory.Runs.Add( run );
            }

            return runHistory;
        }

        public void WriteRunHistory( RunHistory runHistory )
        {
            var report = new XDocument();

            XElement report_root = new XElement( "RunHistory" );
            report.Add( report_root );

            foreach (var run in runHistory.Runs)
            {
                var runElement = new XElement( "Run",
                    new XAttribute( "Number", run.Number ),
                    new XAttribute( "DateTime", run.Date.ToString() ),
                    new XAttribute( "Passed", run.Passed.ToString() ) 
                );

                foreach (var violation in run.Violations)
                {
                    var ruleElement = new XElement( "Rule",
                        new XAttribute( "ID", violation.Key ),
                        new XAttribute( "Violations", violation.Value )
                    );

                    runElement.Add( ruleElement );
                }

                report_root.Add( runElement );
            }

            _storage.SaveRunHistory( report, _args.History );
        }

        public RunHistoryEntry GenerateEntry(Dictionary<Rule, FileProblems> runDetails, RunHistory runHistory, DateTime runDateTime)
        {
            // TODO: when I make this work, have a failing test first.  Maybe then I won't need to rewrite it again, eh?
            var failures = ListRunFailures( runDetails, runHistory );
            var entry = new RunHistoryEntry {
                Passed = (failures.Count == 0),
                Number = runHistory.NextRunNumber, 
                Date = runDateTime 
            };

            foreach (var keyRule in runDetails.Keys)
            {
                entry.Violations[keyRule.ID] = countViolations(runDetails[keyRule]);
            }

            return entry;
        }

        public List<string> ListRunFailures( Dictionary<Rule, FileProblems> runDetails, RunHistory history )
        {
            var failures = new List<string>();

            foreach (var rule in runDetails.Keys)
            {
                int count = countViolations( runDetails[rule] );

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
                    threshold = history.WaterlineFor( rule.ID );
                    break;

                case RunFailMode.None:
                    threshold = int.MaxValue;
                    break;

                default:
                    System.Reflection.MethodBase thisMethod = new StackTrace().GetFrame(0).GetMethod();
                    throw new Exception(String.Format("I do not know how to check a failure mode of [{0}].  Please extend {1}.{2}.", 
                        rule.RunFail, thisMethod.ReflectedType, thisMethod.Name));
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

        private int countViolations( FileProblems ruleRunDetails )
        {
            int count = 0;
            foreach (SourceFile source in ruleRunDetails.Keys)
            {
                count += ruleRunDetails[source].Count;
            }
            return count;
        }

    }
}
