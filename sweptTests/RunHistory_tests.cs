//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace swept.Tests
{
    [TestFixture]
    public class RunHistory_tests
    {
        private RunHistory _history;

        [SetUp]
        public void SetUp()
        {
            _history = new RunHistory();
        }

        [Test]
        public void Starts_with_no_runs_ready_for_first()
        {
            Assert.That( _history.Runs.Count(), Is.EqualTo( 0 ) );
            Assert.That( _history.NextRunNumber, Is.EqualTo( 1 ) );
        }

        [Test]
        public void Can_add_a_Run()
        {
            var run = new RunHistoryEntry() { Number = 27 };
            _history.AddEntry( run );

            Assert.That( _history.Runs.Count(), Is.EqualTo( 1 ) );
            Assert.That( _history.NextRunNumber, Is.EqualTo( 28 ) );
        }

        [Test]
        public void Can_get_waterline_for_a_Rule()
        {
            _history.AddEntry( new RunHistoryEntry() );

            var entry = new RunHistoryEntry { Passed = true, Number = 401 };
            entry.RuleResults["Eliminate Crystal Reports"] = new HistoricRuleResult { TaskCount = 6 };
            entry.RuleResults["Update widgets to widgets 2.0"] = new HistoricRuleResult { TaskCount = 45 };
            _history.AddEntry( entry );

            Assert.That( _history.WaterlineFor( "Eliminate Crystal Reports" ), Is.EqualTo( 6 ) );
            Assert.That( _history.WaterlineFor( "Update widgets to widgets 2.0" ), Is.EqualTo( 45 ) );
        }

        [Test]
        public void Waterline_on_an_empty_history_starts_high()
        {
            Assert.That( _history.WaterlineFor( "Update widgets to widgets 2.0" ), Is.EqualTo( RunHistory.HighWaterLine ) );
        }

        [Test]
        public void Waterline_of_an_unfound_Rule_starts_high()
        {
            var entry = new RunHistoryEntry() { Number = 401 };
            entry.RuleResults["Eliminate Crystal Reports"] = new HistoricRuleResult { TaskCount = 6 };
            _history.AddEntry( entry );

            Assert.That( _history.WaterlineFor( "Update widgets to widgets 2.0" ), Is.EqualTo( RunHistory.HighWaterLine ) );
        }

        [Test]
        public void Waterline_unchanged_by_failed_runs()
        {
            var entry = new RunHistoryEntry() { Number = 401, Passed = true };
            entry.RuleResults["Eliminate Crystal Reports"] = new HistoricRuleResult { TaskCount = 6 };
            _history.AddEntry( entry );

            // even though the failed run has fewer tasks for this particular rule
            var failingEntry = new RunHistoryEntry() { Number = 402, Passed = false };
            failingEntry.RuleResults["Eliminate Crystal Reports"] = new HistoricRuleResult { TaskCount = 1 };
            _history.AddEntry( failingEntry );

            Assert.That( _history.WaterlineFor( "Eliminate Crystal Reports" ), Is.EqualTo( 6 ) );

            //  Rationale:  Someone may improve some rules and learn only later that it
            //  was at the expense of breaking other rules.  Backing out all those changes should
            //  lead back to a passing run (presuming we were passing when we started), not a failing run.
        }

        [Test]
        public void LatestPassingRun_is_null_when_history_lacks_passing_runs()
        {
            Assert.That( _history.Runs.Count(), Is.EqualTo( 0 ) );
            Assert.Null( _history.LatestPassingRun );

            _history.AddEntry( new RunHistoryEntry {
                Date = DateTime.Now,
                Number = 1,
                Passed = false
            } );

            Assert.That( _history.Runs.Count(), Is.GreaterThan( 0 ) );
            Assert.Null( _history.LatestPassingRun );
        }

        [Test]
        public void LatestPassingRun_decides_latest_based_on_run_Number()
        {
            RunHistoryEntry shouldBeLatest = new RunHistoryEntry {
                Date = DateTime.Now,
                Number = 2,
                Passed = true
            };

            RunHistoryEntry shouldNotBeLatest = new RunHistoryEntry {
                Date = DateTime.Now.AddDays( 2 ),  // not latest even though it ran most recently
                Number = 1,
                Passed = true
            };

            _history.AddEntry( shouldBeLatest );
            _history.AddEntry( shouldNotBeLatest );  // not latest even though it's added most recently

            Assert.That( _history.LatestPassingRun, Is.SameAs( shouldBeLatest ) );

            //  Can change if needed--though Date and Number sequences _should_ never diverge.
        }
    }
}
