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
    public class RunFix_tests
    {
        private RunHistory _history;
        private RuleTasks _runResults;
        private RunInspector _inspector;

        [SetUp]
        public void Setup()
        {
            _history = new RunHistory();
            _runResults = new RuleTasks();
            _inspector = new RunInspector( _history );
        }

        [Test]
        public void Nothing_fixed_if_no_history()
        {
            var fixes = _inspector.ListRunFixes( _runResults );
            Assert.That( fixes.Count(), Is.EqualTo( 0 ) );
        }

        [Test]
        public void One_prior_failure_and_no_current_failures_means_one_fix()
        {
            var entry = _inspector.GenerateEntry( DateTime.Now, _runResults );
            HistoricRuleResult fooResult = new HistoricRuleResult {
                FailOn = RuleFailOn.Any,
                ID = "No more Foo!",
                Prior = 0,
                Violations = 1,
                Breaking = true
            };
            entry.RuleResults.Add( "No more Foo!", fooResult );
            var fixes = _inspector.ListRunFixes( _runResults );
            Assert.That( fixes.Count(), Is.EqualTo( 1 ) );
        }

    }
}
