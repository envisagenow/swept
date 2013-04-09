//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;

namespace swept
{
    public class RunInspector
    {
        private readonly RunHistory _runHistory;
        public RunInspector( RunHistory runHistory )
        {
            _runHistory = runHistory;
        }

        public RunHistoryEntry GenerateEntry( DateTime runDateTime, RuleTasks ruleTasks )
        {
            var entry = new RunHistoryEntry
            {
                Passed = (CountRunFailures( ruleTasks ) == 0),
                Number = _runHistory.NextRunNumber,
                Date = runDateTime
            };

            foreach (var keyRule in ruleTasks.Keys)
            {
                int violations = ruleTasks[keyRule].CountTasks();
                entry.RuleResults[keyRule.ID] = GetRuleResult( keyRule, violations, _runHistory.LatestPassingRun );
            }

            return entry;
        }

        public XElement GenerateDeltaXml( RunHistoryEntry entry )
        {
            var doc = new XElement( "SweptBreakageDelta" );

            var failIDs = ListRunFailureIDs( entry );

            foreach (var failID in failIDs)
            {
                var result = entry.RuleResults[failID];
                doc.Add( NewDeltaItem( failID, result.Threshold, result.TaskCount, "Fail", result.Description ) );
            }

            if (failIDs.Count == 0)
            {
                var fixIDs = ListRunFixIDs( entry );
                foreach (var fixID in fixIDs)
                {
                    if (entry.RuleResults.ContainsKey( fixID ))
                    {
                        var result = entry.RuleResults[fixID];
                        doc.Add( NewDeltaItem( fixID, result.Threshold, result.TaskCount, "Fix", result.Description ) );
                    }
                    else
                    {
                        var result = _runHistory.LatestPassingRun.RuleResults[fixID];
                        doc.Add( NewDeltaItem( fixID, result.Threshold, 0, "Gone", result.Description ) );
                    }
                }
            }

            return doc;
        }

        private static XElement NewDeltaItem( string ruleID, int threshold, int taskCount, string outcome, string description )
        {
            return new XElement( "DeltaItem",
                new XAttribute( "ID", ruleID ),
                new XAttribute( "Threshold", threshold ),
                new XAttribute( "TaskCount", taskCount ),
                new XAttribute( "Outcome", outcome ),
                new XAttribute( "Description", description )
            );
        }
        public int CountRunFailures( RuleTasks ruleTasks )
        {
            int failures = 0;

            foreach (var rule in ruleTasks.Keys)
            {
                int taskCount = ruleTasks[rule].CountTasks();
                int threshold = _runHistory.GetThreshold( rule );

                if (taskCount > threshold)
                    failures++;
            }

            return failures;
        }

        public List<string> ListRunFailureIDs( RunHistoryEntry entry )
        {
            var failures = new List<string>();

            foreach (var ruleID in entry.RuleResults.Keys)
            {
                var result = entry.RuleResults[ruleID];
                if (result.Breaking && result.TaskCount > result.Threshold)
                {
                    failures.Add( ruleID );
                }
            }
            return failures;
        }

        public List<string> ListRunFailureMessages( RunHistoryEntry entry )
        {
            return ListRunFailureIDs( entry ).Select( 
                id => reportFailLine( entry.RuleResults[id] )
            ).ToList();
        }

        private string reportFailLine( HistoricRuleResult result )
        {
            return string.Format(
                "Rule [{0}] has [{1}] {2}, and it breaks the build if there are {3} tasks.",
                result.ID, result.TaskCount, "task".Plurs( result.TaskCount ),
                (result.Threshold == 0) ? "any" : string.Concat( "over [", result.Threshold, "]" )
            );
        }

        public List<string> ListRunFixIDs( RunHistoryEntry currentEntry )
        {
            var fixes = new List<string>();
            if (_runHistory.LatestPassingRun == null)
                return fixes;

            foreach (string priorRuleID in _runHistory.LatestPassingRun.RuleResults.Keys)
            {
                int waterline = _runHistory.WaterlineFor( priorRuleID );
                if (currentEntry.RuleResults.ContainsKey( priorRuleID ))
                {
                    HistoricRuleResult result = currentEntry.RuleResults[priorRuleID];
                    if (result.TaskCount < waterline)
                        fixes.Add( priorRuleID );
                }
                else
                {
                    fixes.Add( priorRuleID );
                }
            }

            return fixes;
        }

        public HistoricRuleResult GetRuleResult( Rule ruleUnderTest, int violations, RunHistoryEntry priorSuccess )
        {
            int priorViolations = 0;

            if (ruleUnderTest.FailOn == RuleFailOn.Increase)
                priorViolations = violations;

            if (priorSuccess != null)
            {
                if (priorSuccess.RuleResults.ContainsKey( ruleUnderTest.ID ))
                    priorViolations = priorSuccess.RuleResults[ruleUnderTest.ID].TaskCount;
            }

            bool breaking = false;

            if (ruleUnderTest.FailOn == RuleFailOn.Any)
                breaking = violations > 0;

            if (ruleUnderTest.FailOn == RuleFailOn.Increase)
                breaking = violations > priorViolations;

            return new HistoricRuleResult
            {
                ID = ruleUnderTest.ID,
                FailOn = ruleUnderTest.FailOn,
                TaskCount = violations,
                Threshold = priorViolations,
                Breaking = breaking,
                Description = ruleUnderTest.Description
            };
        }

    }
}
