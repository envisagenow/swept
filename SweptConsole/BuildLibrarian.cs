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
        internal List<string> _failures;
        private Dictionary<Rule, FileProblems> _filesPerRule;
        private RunHistory _runHistory;

        public BuildLibrarian( Arguments args, IStorageAdapter storage )
        {
            _args = args;
            _storage = storage;
        }

        public string GetConsoleHeader(DateTime runtime)
        {
            if (!_args.Check)
                return string.Empty;
            return String.Format( "Swept checking [{0}] with rules in [{1}] on {2}...{3}", _storage.GetCWD(), _args.Library, runtime.ToString( "G" ), Environment.NewLine );
        }

        public string ReportOn( Dictionary<Rule, FileProblems> filesPerRule, RunHistory runHistory )
        {
            _filesPerRule = filesPerRule;
            _runHistory = runHistory;
            _failures = ListRunFailures( filesPerRule, runHistory );

            
            if (_args.Check)
                return ReportCheckResult();
            else
                return ReportXmlResult();
        }

        public string ReportFailures()
        {
            if (_failures.Count == 0)
                return string.Empty;
            else
                return "Error:  " + string.Join( "\r\nError:  ", _failures.ToArray() ) + "\r\n";
        }

        public string ReportCheckResult()
        {
            return ReportCheckResult( _failures );
        }

        public string ReportCheckResult( List<string> failures )
        {
            if (!failures.Any())
                return "Swept check passed!\r\n";
            else
                return "Swept check failed!\r\n";
        }

        private string ReportXmlResult()
        {
            XDocument report_doc = new XDocument();
            XElement report_root = new XElement( "SweptBuildReport" );

            //  TODO: Get failure details into this XML report
            // Goal code:
            //  var failures_element = GetFailureReportElement( failures );
            //  report_root.Add( failures_element );

            int totalTasks = 0;
            foreach (Rule rule in _filesPerRule.Keys.OrderBy( c => c.ID ))
            {
                int totalTasksPerRule = 0;

                var rule_element = new XElement( "Rule",
                    new XAttribute( "ID", rule.ID ),
                    new XAttribute( "Description", rule.Description )
                );

                var fileMatches = _filesPerRule[rule];
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

        public string ReportBuildFailures()
        {
            if (_failures.Count == 0)
                return string.Empty;
            else
            {
                var plurality = _failures.Count == 1 ? "" : "s";

                string failuresText = "";
                foreach (string fail in _failures)
                {
                    failuresText += fail + "\n";
                }

                string failureMessage = "Swept failed due to build breaking rule failure{0}:\n{1}";
                return string.Format( failureMessage, plurality, failuresText );
            }
        }

        public XElement GenerateBuildFailureXML()
        {
            XElement xml = new XElement( "SweptBuildFailures" );

            foreach (string fail in _failures)
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

        private RunHistoryEntry _latestPassingRun = null;
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
                    int rulePrior = int.Parse( ruleXml.Attribute( "Prior" ).Value );
                    bool ruleBreaking = bool.Parse( ruleXml.Attribute( "Breaking" ).Value );
                    RuleFailOn ruleFailOn = (RuleFailOn)Enum.Parse( typeof( RuleFailOn ), ruleXml.Attribute( "FailOn" ).Value );
                    run.RuleResults[ruleID] = new RuleResult {
                        ID = ruleID,
                        Violations = ruleViolations,
                        Prior = rulePrior,
                        FailOn = ruleFailOn,
                        Breaking = ruleBreaking
                    };
                }

                runHistory.Runs.Add( run );

                if (run.Passed)
                    _latestPassingRun = run;
            }

            return runHistory;
        }

        public void WriteRunHistory()
        {
            var report = new XDocument();

            XElement report_root = new XElement( "RunHistory" );
            report.Add( report_root );

            foreach (var run in _runHistory.Runs)
            {
                var runElement = new XElement( "Run",
                    new XAttribute( "Number", run.Number ),
                    new XAttribute( "DateTime", run.Date.ToString() ),
                    new XAttribute( "Passed", run.Passed.ToString() ) 
                );

                foreach (RuleResult result in run.RuleResults.Values.OrderBy( r => r.ID ))
                {
                    var ruleElement = new XElement( "Rule",
                        new XAttribute( "ID", result.ID ),
                        new XAttribute( "Violations", result.Violations ),
                        new XAttribute( "Prior", result.Prior ),
                        new XAttribute( "FailOn", result.FailOn ),
                        new XAttribute( "Breaking", result.Breaking )
                    );

                    runElement.Add( ruleElement );
                }

                report_root.Add( runElement );
            }

            _storage.SaveRunHistory( report, _args.History );
        }

        public RunHistoryEntry GenerateEntry( DateTime runDateTime )
        {
            // TODO: when I make this work, have a failing test first.  Maybe then I won't need to rewrite it again, eh?
            var entry = new RunHistoryEntry {
                Passed = (_failures.Count == 0),
                Number = _runHistory.NextRunNumber, 
                Date = runDateTime 
            };

            foreach (var keyRule in _filesPerRule.Keys)
            {
                int violations = countViolations(_filesPerRule[keyRule]);
                entry.RuleResults[keyRule.ID] = GetRuleResult( keyRule, violations, _latestPassingRun );
            }

            return entry;
        }

        public RuleResult GetRuleResult( Rule ruleUnderTest, int violations, RunHistoryEntry priorSuccess )
        {
            int priorViolations = 0;

            if (ruleUnderTest.FailOn == RuleFailOn.Over)
                priorViolations = ruleUnderTest.RunFailOverLimit;

            if (ruleUnderTest.FailOn == RuleFailOn.Increase)
                priorViolations = violations;

            if (priorSuccess != null)
            {
                if (priorSuccess.RuleResults.ContainsKey( ruleUnderTest.ID ))
                    priorViolations = priorSuccess.RuleResults[ruleUnderTest.ID].Violations;
            }

            bool breaking = false;

            if (ruleUnderTest.FailOn == RuleFailOn.Any)
                breaking = violations > 0;

            if (ruleUnderTest.FailOn == RuleFailOn.Over)
                breaking = violations > ruleUnderTest.RunFailOverLimit;

            if (ruleUnderTest.FailOn == RuleFailOn.Increase)
                breaking = violations > priorViolations;

            return new RuleResult
            {
                ID = ruleUnderTest.ID,
                FailOn = ruleUnderTest.FailOn,
                Violations = violations,
                Prior = priorViolations,
                Breaking = breaking
            };
        }

        public List<string> ListRunFailures( Dictionary<Rule, FileProblems> runDetails, RunHistory history )
        {
            var failures = new List<string>();

            foreach (var rule in runDetails.Keys)
            {
                int count = countViolations( runDetails[rule] );

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
                    threshold = history.WaterlineFor( rule.ID );
                    break;

                case RuleFailOn.None:
                    threshold = int.MaxValue;
                    break;

                default:
                    System.Reflection.MethodBase thisMethod = new StackTrace().GetFrame(0).GetMethod();
                    throw new Exception(String.Format("I do not know how to check a failure mode of [{0}].  Please extend {1}.{2}.", 
                        rule.FailOn, thisMethod.ReflectedType, thisMethod.Name));
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
