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
            var run = new RunHistoryEntry();
            run.Number = 27;
            _history.AddEntry( run );

            Assert.That( _history.Runs.Count(), Is.EqualTo( 1 ) );
            Assert.That( _history.NextRunNumber, Is.EqualTo( 28 ) );
        }

        [Test]
        public void Can_get_waterline_for_a_Rule()
        {
            _history.AddEntry( new RunHistoryEntry() );

            var run = new RunHistoryEntry { Passed = true };
            run.Number = 401;
            run.RuleResults["Eliminate Crystal Reports"] = new HistoricRuleResult { Violations = 6 };
            run.RuleResults["Update widgets to widgets 2.0"] = new HistoricRuleResult { Violations = 45 };
            _history.AddEntry( run );

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
            var run = new RunHistoryEntry();
            run.Number = 401;
            run.RuleResults["Eliminate Crystal Reports"] = new HistoricRuleResult { Violations = 6 };
            _history.AddEntry( run );

            Assert.That( _history.WaterlineFor( "Update widgets to widgets 2.0" ), Is.EqualTo( RunHistory.HighWaterLine ) );
        }

        [Test]
        public void Failed_runs_do_not_alter_waterline()
        {
            var run = new RunHistoryEntry() { Number = 401, Passed = true };
            run.RuleResults["Eliminate Crystal Reports"] = new HistoricRuleResult { Violations = 6 };
            _history.AddEntry( run );

            var failingRun = new RunHistoryEntry() { Number = 402, Passed = false };
            failingRun.RuleResults["Eliminate Crystal Reports"] = new HistoricRuleResult { Violations = 718 };
            _history.AddEntry( failingRun );

            Assert.That( _history.WaterlineFor( "Eliminate Crystal Reports" ), Is.EqualTo( 6 ) );
        }

    }
}
