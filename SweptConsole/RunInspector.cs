//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Linq;
using System.Collections.Generic;

namespace swept
{
    public class RunInspector
    {
        private RunHistory _runHistory;
        public RunInspector( RunHistory runHistory )
        {
            _runHistory = runHistory;
        }

        public RunHistoryEntry GenerateEntry( DateTime runDateTime, RuleTasks ruleTasks )
        {
            List<string> failures = ListRunFailures( ruleTasks );

            var entry = new RunHistoryEntry
            {
                Passed = (failures.Count == 0),
                Number = _runHistory.NextRunNumber,
                Date = runDateTime
            };

            foreach (var keyRule in ruleTasks.Keys)
            {
                int violations = countViolations( ruleTasks[keyRule] );
                entry.RuleResults[keyRule.ID] = GetRuleResult( keyRule, violations, _runHistory.LatestPassingRun );
            }

            return entry;
        }

        public List<string> ListRunFailures( RuleTasks ruleTasks )
        {
            var failures = new List<string>();

            foreach (var rule in ruleTasks.Keys)
            {
                int count = countViolations( ruleTasks[rule] );

                int threshold = _runHistory.GetThreshold( rule );
                if (count > threshold)
                {
                    failures.Add( string.Format(
                        "Rule [{0}] has been violated [{1}] {2}, and it breaks the build if there are {3} violations.",
                        rule.ID, count, "time".Plurs( count ),
                        (threshold == 0) ? "any" : string.Concat( "over [", threshold, "]" )
                    ) );
                }
            }

            return failures;
        }

        public List<string> ListRunFixes( RuleTasks ruleTasks )
        {
            //!!!
            var fixes = new List<string>();
            return fixes;
        }


        private int countViolations( FileTasks fileTasks )
        {
            return fileTasks.Keys.Sum( file => fileTasks[file].Count );
        }

        public HistoricRuleResult GetRuleResult( Rule ruleUnderTest, int violations, RunHistoryEntry priorSuccess )
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

            return new HistoricRuleResult
            {
                ID = ruleUnderTest.ID,
                FailOn = ruleUnderTest.FailOn,
                Violations = violations,
                Prior = priorViolations,
                Breaking = breaking
            };
        }

    }
}
