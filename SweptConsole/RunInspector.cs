//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2015 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace swept
{
    public class DetailDelta
    {
        public List<DetailDeltaFile> Files = new List<DetailDeltaFile>();
    }

    public class DetailDeltaFile
    {
        public string Name = string.Empty;
        public List<DetailDeltaRule> Rules = new List<DetailDeltaRule>();
    }

    public class DetailDeltaRule
    {

    }

    public class RunInspector
    {
        private readonly RunHistory _runHistory;
        public RunInspector( RunHistory runHistory )
        {
            _runHistory = runHistory;
        }

        public RunEntry GenerateEntry( DateTime runDateTime, RuleTasks ruleTasks )
        {
            var entry = new RunEntry
            {
                Passed = (CountRunFailures( ruleTasks ) == 0),
                Number = _runHistory.NextRunNumber,
                Date = runDateTime
            };

            foreach (var keyRule in ruleTasks.Keys)
            {
                int violations = ruleTasks[keyRule].CountTasks();
                entry.RuleResults[keyRule.ID] = GetRuleResult( keyRule, violations, _runHistory.Runs.LastOrDefault(r => r.Passed) );
            }

            return entry;
        }

        public DetailDelta GetDetailDelta(RuleTasks ruleTasks, RunChanges runDetails)
        {

            var result = new DetailDelta();

            foreach (var file in runDetails.Files)
            {
                var detailDeltaFile = new DetailDeltaFile();
                detailDeltaFile.Name = file.Name;

                foreach (var rule in file.Rules)
                {
                     detailDeltaFile.Rules.Add (new DetailDeltaRule ());
                }
               
                result.Files.Add(detailDeltaFile);
            }

            return result;
        }

        public void GenerateDeltaTeamCityOutput(TextWriter standardOutput, RunEntry entry)
        {
            var failIDs = ListRunFailureIDs( entry );

            foreach (var failID in failIDs)
            {
                var result = entry.RuleResults[failID];
                standardOutput.WriteLine($"Swept Failure [{result.ID}] {result.Description}: has {result.TaskCount} task(s), increased from {result.Threshold}");
            }

            if (failIDs.Count == 0)
            {
                var fixIDs = ListRunFixIDs( entry );
                var latestRun = _runHistory.Runs.LastOrDefault();
                foreach (var fixID in fixIDs)
                {
                    if (entry.RuleResults.ContainsKey( fixID ))
                    {
                        var result = entry.RuleResults[fixID];
                        standardOutput.WriteLine($"Swept Fix [{result.ID}] {result.Description}: has {result.TaskCount} task(s), decreased from {result.Threshold}");
                    }
                    else
                    {
                        var result = latestRun.RuleResults[fixID];
                        standardOutput.WriteLine($"Swept Fix [{result.ID}] {result.Description}: has 0 tasks, decreased from {result.Threshold}");
                    }
                }
            }
        }

        public XElement GenerateDeltaXml( RunEntry entry )
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
                var latestRun = _runHistory.Runs.LastOrDefault();
                foreach (var fixID in fixIDs)
                {
                    if (entry.RuleResults.ContainsKey( fixID ))
                    {
                        var result = entry.RuleResults[fixID];
                        doc.Add( NewDeltaItem( fixID, result.Threshold, result.TaskCount, "Fix", result.Description ) );
                    }
                    else
                    {
                        var result = latestRun.RuleResults[fixID];
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

        public List<string> ListRunFailureIDs( RunEntry entry )
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

        public List<string> ListRunFailureMessages( RunEntry entry )
        {
            return ListRunFailureIDs( entry ).Select( 
                id => reportFailLine( entry.RuleResults[id] )
            ).ToList();
        }

        private string reportFailLine( RuleResult result )
        {
            return string.Format(
                "Rule [{0}] has [{1}] {2}, and it breaks the build if there are {3} tasks.",
                result.ID, result.TaskCount, "task".Plurs( result.TaskCount ),
                (result.Threshold == 0) ? "any" : string.Concat( "over [", result.Threshold, "]" )
            );
        }

        public List<string> ListRunFixIDs( RunEntry currentEntry )
        {
            var fixes = new List<string>();
            var latestRun = _runHistory.Runs.LastOrDefault();
            if (latestRun == null)
                return fixes;

            foreach (string priorRuleID in latestRun.RuleResults.Keys)
            {
                int waterline = latestRun.WaterlineFor( priorRuleID );
                if (currentEntry.RuleResults.ContainsKey( priorRuleID ))
                {
                    RuleResult result = currentEntry.RuleResults[priorRuleID];
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

        public RuleResult GetRuleResult( Rule ruleUnderTest, int violations, RunEntry priorRun )
        {
            int priorViolations = 0;

            if (ruleUnderTest.FailOn == RuleFailOn.Increase)
                priorViolations = violations;

            if (priorRun != null)
            {
                if (priorRun.RuleResults.ContainsKey( ruleUnderTest.ID ))
                    priorViolations = priorRun.RuleResults[ruleUnderTest.ID].TaskCount;
            }

            bool breaking = false;

            if (ruleUnderTest.FailOn == RuleFailOn.Any)
                breaking = violations > 0;

            if (ruleUnderTest.FailOn == RuleFailOn.Increase)
                breaking = violations > priorViolations;

            return new RuleResult
            {
                ID = ruleUnderTest.ID,
                FailOn = ruleUnderTest.FailOn,
                TaskCount = violations,
                Threshold = priorViolations,
                Breaking = breaking,
                Description = ruleUnderTest.Description
            };
        }

        public List<Flag> ReportUpdatedFlags( List<Flag> existingFlags, RunEntry runResult, List<Commit> changeSet )
        {
            var flags = new List<Flag>();

            foreach (var existingFlag in existingFlags)
            {
                flags.Add(existingFlag);
            }

            foreach (var key in runResult.RuleResults.Keys)
            {
                var result = runResult.RuleResults[key];

                if (result.Threshold >= result.TaskCount)
                {
                    flags.RemoveAll(f => f.RuleID == result.ID);
                }
                else
                {
                    var priorFlag = flags.LastOrDefault(f => f.RuleID == result.ID);
                    if (priorFlag != null && priorFlag.TaskCount >= result.TaskCount) continue;

                    var flag = new Flag
                    {
                        Threshold = result.Threshold,
                        TaskCount = result.TaskCount,
                    };
                    flag.Commits.AddRange(changeSet);
                    flags.Add(flag);
                }
            }

            return flags;
        }

    }
}
