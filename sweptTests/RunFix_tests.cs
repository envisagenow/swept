//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using System.Xml.Linq;

namespace swept.Tests
{

    [TestFixture]
    public class RunFix_tests
    {
        private RunHistory _runHistory;
        private RuleTasks _ruleTasks;
        private RunInspector _inspector;

        [SetUp]
        public void Setup()
        {
            _runHistory = new RunHistory();
            _ruleTasks = new RuleTasks();
            _inspector = new RunInspector( _runHistory );
        }

        [Test]
        public void Nothing_fixed_if_history_empty()
        {
            var entry = _inspector.GenerateEntry( DateTime.Now, _ruleTasks );
            var fixes = _inspector.ListRunFixIDs( entry );
            Assert.That( fixes.Count(), Is.EqualTo( 0 ) );
        }

        [Test]
        public void One_fix_when_this_run_has_fewer_tasks_than_LatestPassingRun()
        {
            var oldEntry = _inspector.GenerateEntry( DateTime.Now.AddDays( -2 ), _ruleTasks );
            var fooResult = new RuleResult {
                FailOn = RuleFailOn.Increase,
                ID = "No more Foo!",
                Threshold = 221,
                TaskCount = 221,
                Breaking = false
            };
            oldEntry.RuleResults.Add( "No more Foo!", fooResult );
            _runHistory.AddEntry( oldEntry );
            Assert.That( _runHistory.LatestPassingRun, Is.SameAs( oldEntry ) );


            var newEntry = _inspector.GenerateEntry( DateTime.Now, new RuleTasks() );
            var newFooResult = new RuleResult {
                FailOn = RuleFailOn.Increase,
                ID = "No more Foo!",
                Threshold = 221,
                TaskCount = 201,
                Breaking = false
            };
            newEntry.RuleResults.Add( "No more Foo!", newFooResult );

            var fixes = _inspector.ListRunFixIDs( newEntry );
            Assert.That( fixes.Count(), Is.EqualTo( 1 ) );
        }

        [Test]
        public void No_fix_from_equal_or_higher_task_count()
        {
            var oldEntry = _inspector.GenerateEntry( DateTime.Now.AddDays( -2 ), _ruleTasks );
            var fooResult = new RuleResult {
                FailOn = RuleFailOn.Increase,
                ID = "No more Foo!",
                Threshold = 221,
                TaskCount = 221,
                Breaking = false
            };
            var barResult = new RuleResult {
                FailOn = RuleFailOn.Increase,
                ID = "Less bar!",
                Threshold = 41,
                TaskCount = 41,
                Breaking = false
            };
            oldEntry.RuleResults.Add( "No more Foo!", fooResult );
            oldEntry.RuleResults.Add( "Less bar!", barResult );
            _runHistory.AddEntry( oldEntry );
            Assert.That( _runHistory.LatestPassingRun, Is.SameAs( oldEntry ) );


            var newEntry = _inspector.GenerateEntry( DateTime.Now, new RuleTasks() );
            var newFooResult = new RuleResult {
                FailOn = RuleFailOn.Increase,
                ID = "No more Foo!",
                Threshold = 221,
                TaskCount = 291,
                Breaking = false
            };
            var newBarResult = new RuleResult {
                FailOn = RuleFailOn.Increase,
                ID = "Less bar!",
                Threshold = 41,
                TaskCount = 41,
                Breaking = false
            };
            newEntry.RuleResults.Add( "No more Foo!", newFooResult );
            newEntry.RuleResults.Add( "Less bar!", newBarResult );

            var fixes = _inspector.ListRunFixIDs( newEntry );
            Assert.That( fixes.Count(), Is.EqualTo( 0 ) );
        }

        [Test]
        public void Fixes_scored_even_if_no_passing_run_exists()
        {
            var oldEntry = _inspector.GenerateEntry( DateTime.Now.AddDays( -2 ), _ruleTasks );
            oldEntry.Passed = false;
            var fooResult = new RuleResult {
                FailOn = RuleFailOn.Increase,
                ID = "No more Foo!",
                Threshold = 221,
                TaskCount = 250,
                Breaking = false
            };
            var barResult = new RuleResult {
                FailOn = RuleFailOn.Increase,
                ID = "Less bar!",
                Threshold = 41,
                TaskCount = 41,
                Breaking = false
            };
            oldEntry.RuleResults.Add( "No more Foo!", fooResult );
            oldEntry.RuleResults.Add( "Less bar!", barResult );
            _runHistory.AddEntry( oldEntry );
            Assert.That( _runHistory.LatestPassingRun, Is.Null );


            var newEntry = _inspector.GenerateEntry( DateTime.Now, new RuleTasks() );
            var newFooResult = new RuleResult {
                FailOn = RuleFailOn.Increase,
                ID = "No more Foo!",
                Threshold = 221,
                TaskCount = 200,
                Breaking = false
            };
            var newBarResult = new RuleResult {
                FailOn = RuleFailOn.Increase,
                ID = "Less bar!",
                Threshold = 41,
                TaskCount = 41,
                Breaking = false
            };
            newEntry.RuleResults.Add( "No more Foo!", newFooResult );
            newEntry.RuleResults.Add( "Less bar!", newBarResult );

            var fixes = _inspector.ListRunFixIDs( newEntry );
            Assert.That( fixes.Count(), Is.EqualTo( 1 ) );
        }


    }
}
