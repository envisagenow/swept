//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace swept.Tests
{
    //  So what do I think about unit test classes describing their domain
    //  rather than corresponding 1:1-ishly to app classes?
    [TestFixture]
    public class RunFail_tests
    {
        private BuildLibrarian _checker;
        private RunHistory _history;
        private Dictionary<Rule, FileProblems> _runResults;

        [SetUp]
        public void Setup()
        {
            _history = new RunHistory();
            _checker = new BuildLibrarian( null, null );
            _runResults = new Dictionary<Rule, FileProblems>();
        }

        [Test]
        public void Zero_Violations_do_not_fail()
        {
            var problems = _checker.ListRunFailures( _runResults, _history );
            Assert.That( problems.Count(), Is.EqualTo( 0 ) );
        }

        [Test]
        public void When_we_violate_a_FailAny_Rule_we_fail()
        {
            Rule rule = new Rule() { ID = "644", Description = "Major problem!", FailOn = RuleFailOn.Any };
            var sourceClauseMatch = new FileProblems();

            SourceFile failedSource = new SourceFile( "some_file.cs" );
            ClauseMatch failedClause = new LineMatch( new List<int> { 1, 44 } );

            sourceClauseMatch[failedSource] = failedClause;

            _runResults[rule] = sourceClauseMatch;

            var failures = _checker.ListRunFailures( _runResults, _history );

            Assert.That( failures.Count(), Is.EqualTo( 1 ) );
            Assert.That( failures[0], Is.EqualTo( "Rule [644] has been violated [2] times, and it breaks the build if there are any violations." ) );
        }

        [Test]
        public void When_we_have_no_tasks_in_a_FailAny_Rule_we_do_not_fail()
        {
            Rule rule = new Rule() { ID = "644", Description = "Major problem!", FailOn = RuleFailOn.Any };
            var sourceClauseMatch = new FileProblems();

            _runResults[rule] = sourceClauseMatch;

            var failures = _checker.ListRunFailures( _runResults, _history );

            Assert.That( failures.Count(), Is.EqualTo( 0 ) );
        }

        [Test]
        public void When_we_violate_a_BuildFail_None_Rule_we_do_not_fail()
        {
            Rule rule = new Rule() { ID = "644", Description="Not a problem.", FailOn = RuleFailOn.None };
            var sourceClauseMatch = new FileProblems();

            SourceFile failedSource = new SourceFile( "some_file.cs" );
            ClauseMatch failedClause = new LineMatch( new List<int> { 1, 44 } );

            sourceClauseMatch[failedSource] = failedClause;

            var rules = new Dictionary<Rule, FileProblems>();
            rules[rule] = sourceClauseMatch;

            var failures = _checker.ListRunFailures( rules, _history );

            Assert.That( failures.Count(), Is.EqualTo( 0 ) );
        }

        [Test]
        public void When_we_violate_multiple_BuildFail_Any_Rules_we_report_all_failures()
        {
            var rule = new Rule() { ID = "191", Description = "Major problem!", FailOn = RuleFailOn.Any };
            var rule2 = new Rule() { ID = "200", Description = "Major problem!", FailOn = RuleFailOn.Any };
            var sourceClauseMatch = new FileProblems();
            var sourceClauseMatch2 = new FileProblems();

            var failedSource = new SourceFile( "some_file.cs" );
            ClauseMatch failedClause = new LineMatch( new List<int> { 1, 44 } );
            sourceClauseMatch[failedSource] = failedClause;

            var failedSource2 = new SourceFile( "some_other_file.cs" );
            ClauseMatch failedClause2 = new LineMatch( new List<int> { 23, 65, 81 } );
            sourceClauseMatch2[failedSource2] = failedClause2;

            _runResults[rule] = sourceClauseMatch;
            _runResults[rule2] = sourceClauseMatch2;

            var failures = _checker.ListRunFailures( _runResults, _history );

            Assert.That( failures.Count(), Is.EqualTo( 2 ) );
            Assert.That( failures[0], Is.EqualTo( "Rule [191] has been violated [2] times, and it breaks the build if there are any violations." ) );
            Assert.That( failures[1], Is.EqualTo( "Rule [200] has been violated [3] times, and it breaks the build if there are any violations." ) );
        }

        [Test]
        public void When_we_exceed_BuildFail_Over_limit_we_fail()
        {
            var rule = new Rule() {
                ID = "200",
                Description = "Major problem!",
                FailOn = RuleFailOn.Over,
                RunFailOverLimit = 2
            };
            var sourceClauseMatch = new FileProblems();

            var failedSource = new SourceFile( "some_other_file.cs" );
            ClauseMatch failedClause = new LineMatch( new List<int> { 23, 65, 81 } );
            sourceClauseMatch[failedSource] = failedClause;

            _runResults[rule] = sourceClauseMatch;

            var failures = _checker.ListRunFailures( _runResults, _history );

            Assert.That( failures.Count(), Is.EqualTo( 1 ) );
            Assert.That( failures[0], Is.EqualTo( "Rule [200] has been violated [3] times, and it breaks the build if there are over [2] violations." ) );
        }

        [Test]
        public void When_we_violate_BuildFail_Over_but_do_not_exceed_limit_we_do_not_fail()
        {
            var rule = new Rule() {
                ID = "191",
                Description = "Major problem!",
                FailOn = RuleFailOn.Over,
                RunFailOverLimit = 2
            };
            var sourceClauseMatch = new FileProblems();

            var failedSource = new SourceFile( "some_file.cs" );
            ClauseMatch failedClause = new LineMatch( new List<int> { 1, 44 } );
            sourceClauseMatch[failedSource] = failedClause;

            _runResults[rule] = sourceClauseMatch;

            var failures = _checker.ListRunFailures( _runResults, _history );

            Assert.That( failures.Count(), Is.EqualTo( 0 ) );
        }

        [Test]
        public void When_failures_increase_on_a_FailMode_Increase_rule_we_break()
        {
            var rule = new Rule()
            {
                ID = "300",
                Description = "Major problem!",
                FailOn = RuleFailOn.Increase,
            };
            var sourceClauseMatch = new FileProblems();

            var failedSource = new SourceFile( "some_file.cs" );
            ClauseMatch failedClause = new LineMatch( new List<int> { 1, 44, 68, 70, 79, 102, 111, 194, 198, 292, 321, 334, 345, 367 } );
            sourceClauseMatch[failedSource] = failedClause;

            _runResults[rule] = sourceClauseMatch;

            var results = new Dictionary<string,RuleResult>();
            results["300"] = new RuleResult { Violations = 10 };

            _history.AddRun( new RunHistoryEntry { 
                Date = DateTime.Now.AddDays( -7 ), 
                Number = 1704, 
                RuleResults = results, 
                Passed = true 
            } );

            var failures = _checker.ListRunFailures( _runResults, _history );

            Assert.That( failures.Count(), Is.EqualTo( 1 ) );
            Assert.That( failures[0], Is.EqualTo( "Rule [300] has been violated [14] times, and it breaks the build if there are over [10] violations." ) );
        }
    }
}
