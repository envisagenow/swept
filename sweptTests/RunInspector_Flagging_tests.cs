//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2013 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace swept.Tests
{
    [TestFixture]
    public class RunInspector_Flagging_tests
    {
        private RunInspector _inspector;
        private List<Commit> _changeSet;
        private Commit _firstChange;

        [SetUp]
        public void SetUp()
        {
            _inspector = new RunInspector( new RunHistory() );
            _firstChange = new Commit { Person = "ewige.quaston" };
            _changeSet = new List<Commit> { _firstChange };
        }

        [Test]
        public void When_no_existing_flags_and_no_violations_in_current_run_then_no_new_flags()
        {
            var existingFlags = new List<Flag>();
            var runResult = new RunHistoryEntry();

            var flags = _inspector.ReportUpdatedFlags( existingFlags, runResult, _changeSet );

            Assert.That( flags, Is.Empty );
        }

        [Test]
        public void When_no_existing_flags_and_a_violation_in_current_run_then_a_new_flag()
        {
            var existingFlags = new List<Flag>();
            var resultsDictionary = new Dictionary<string, HistoricRuleResult>();
            resultsDictionary["foo"] = new HistoricRuleResult {
                FailOn = RuleFailOn.Increase,
                TaskCount = 12,
                Threshold = 9,
                Breaking = true,
            };

            var runResult = new RunHistoryEntry
            {
                Passed = false,
                RuleResults = resultsDictionary,
            };

            var flags = _inspector.ReportUpdatedFlags( existingFlags, runResult, _changeSet );

            Assert.That( flags.Count, Is.EqualTo( 1 ) );
            var flag = flags[0];

            Assert.That( flag.Threshold, Is.EqualTo( 9 ) );
            Assert.That( flag.TaskCount, Is.EqualTo( 12 ) );
            Assert.That( flag.Changes.Count, Is.EqualTo( 1 ) );
            Assert.That( flag.Changes[0], Is.SameAs( _firstChange ) );
        }

        [Test]
        public void Flag_includes_all_Changes_in_ChangeSet()
        {
            // prior run
            var existingFlags = new List<Flag>();

            var runResult = new RunHistoryEntry
            {
                Passed = false,
                RuleResults = new Dictionary<string, HistoricRuleResult>(),
            };
            runResult.RuleResults["foo"] = new HistoricRuleResult
            {
                FailOn = RuleFailOn.Increase,
                TaskCount = 12,
                Threshold = 9,
                Breaking = true,
            };

            var secondChange = new Commit { ID = "r24987", Person = "mob.barley", Time = "now-ish" };
            _changeSet.Add( secondChange );

            var flags = _inspector.ReportUpdatedFlags( existingFlags, runResult, _changeSet );
            var flag = flags[0];

            Assert.That( flag.Changes.Count, Is.EqualTo( 2 ) );
            Assert.That( flag.Changes[0], Is.SameAs( _firstChange ) );
            Assert.That( flag.Changes[1], Is.SameAs( secondChange ) );
        }

        [Test]
        public void When_current_run_still_over_threshold_then_existing_flags_carried_forward()
        {
            // prior run
            var existingFlags = new List<Flag> { new Flag { TaskCount = 12, Threshold = 9, RuleID = "INT-002" } };

            // current run
            var runResult = new RunHistoryEntry
            {
                Passed = false,
                RuleResults = new Dictionary<string, HistoricRuleResult>(),
            };
            runResult.RuleResults["foo"] = new HistoricRuleResult
            {
                ID = "INT-002",
                FailOn = RuleFailOn.Increase,
                Threshold = 9,
                TaskCount = 12,
                Breaking = true,
            };

            var flags = _inspector.ReportUpdatedFlags(existingFlags, runResult, _changeSet);

            Assert.That(flags.Count, Is.EqualTo(1));
        }

        [Test]
        public void When_current_run_eliminates_a_violation_then_all_flags_on_that_rule_are_cleared()
        {
            var existingFlags = new List<Flag>();
            existingFlags.Add(new Flag { TaskCount = 12, Threshold = 9, RuleID = "INT-002" });
            existingFlags.Add(new Flag { TaskCount = 412, Threshold = 9, RuleID = "INT-002" });
            var resultsDictionary = new Dictionary<string, HistoricRuleResult>();
            resultsDictionary["foo"] = new HistoricRuleResult
            {
                ID = "INT-002",
                FailOn = RuleFailOn.Increase,
                Threshold = 9,
                TaskCount = 8,
                Breaking = false,
            };

            var runResult = new RunHistoryEntry
            {
                Passed = false,
                RuleResults = resultsDictionary,
            };

            var flags = _inspector.ReportUpdatedFlags(existingFlags, runResult, _changeSet);

            Assert.That(flags.Count, Is.EqualTo(0));
        }

        [TestCase(12, 1)]
        [TestCase(10, 1)]
        [TestCase(44, 2)]
        public void When_current_run_continues_an_existing_violation_then_flag_added_if_TaskCount_increases(int newTaskCount, int expectedTaskCount)
        {
            var existingFlags = new List<Flag>();
            existingFlags.Add(new Flag { TaskCount = 12, Threshold = 9, RuleID = "INT-002" });
            var resultsDictionary = new Dictionary<string, HistoricRuleResult>();
            resultsDictionary["foo"] = new HistoricRuleResult
            {
                ID = "INT-002",
                FailOn = RuleFailOn.Increase,
                Threshold = 9,
                TaskCount = newTaskCount,
                Breaking = true,
            };

            var runResult = new RunHistoryEntry
            {
                Passed = false,
                RuleResults = resultsDictionary,
            };

            var flags = _inspector.ReportUpdatedFlags(existingFlags, runResult, _changeSet);

            Assert.That(flags.Count, Is.EqualTo(expectedTaskCount));
        }

        [Test]
        public void When_current_run_eliminates_a_violation_then_flags_on_other_rules_are_unchanged()
        {
            var existingFlags = new List<Flag>();
            existingFlags.Add(new Flag { TaskCount = 12, Threshold = 9, RuleID = "INT-002" });
            existingFlags.Add(new Flag { TaskCount = 33, Threshold = 7, RuleID = "WEB-044" });
            var resultsDictionary = new Dictionary<string, HistoricRuleResult>();
            resultsDictionary["foo"] = new HistoricRuleResult
            {
                ID = "INT-002",
                FailOn = RuleFailOn.Increase,
                Threshold = 9,
                TaskCount = 8,
                Breaking = false,
            };

            var runResult = new RunHistoryEntry
            {
                Passed = false,
                RuleResults = resultsDictionary,
            };

            var flags = _inspector.ReportUpdatedFlags(existingFlags, runResult, _changeSet);

            Assert.That(flags.Count, Is.EqualTo(1));
            var flag = flags[0];

            Assert.That(flag.RuleID, Is.EqualTo("WEB-044"));
        }

    }
}
