﻿//  Swept:  Software Enhancement Progress Tracking.
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
        private RuleTasks _ruleTasks;
        private RunInspector _inspector;

        [SetUp]
        public void Setup()
        {
            _history = new RunHistory();
            _checker = new BuildLibrarian( null, null );
            _ruleTasks = new RuleTasks();
            _inspector = new RunInspector( _history );
        }

        [Test]
        public void Zero_Tasks_do_not_fail()
        {
            var failures = _inspector.CountRunFailures( _ruleTasks );
            Assert.That( failures, Is.EqualTo( 0 ) );
        }

        [Test]
        public void Any_transgression_of_a_FailOnAny_Rule_causes_failure()
        {
            Rule rule = new Rule() { ID = "644", Description = "Major problem!", FailOn = RuleFailOn.Any };
            var sourceClauseMatch = new FileTasks();

            SourceFile failedSource = new SourceFile( "some_file.cs" );
            ClauseMatch failedClause = new LineMatch( new List<int> { 1, 44 } );

            sourceClauseMatch[failedSource] = failedClause;

            _ruleTasks[rule] = sourceClauseMatch;

            var failures = _inspector.CountRunFailures( _ruleTasks );

            Assert.That( failures, Is.EqualTo( 1 ) );
            //Assert.That( failures[0], Is.EqualTo( "Rule [644] has [2] tasks, and it breaks the build if there are any tasks." ) );
        }

        [Test]
        public void Can_ListRunFailureMessages_with_RunHistoryEntry()
        {
            var oldEntry = _inspector.GenerateEntry( DateTime.Now.AddDays( -2 ), _ruleTasks );
            var fooResult = new RuleResult
            {
                ID = "No more Foo!",
                Breaking = true,
                FailOn = RuleFailOn.Increase,
                Threshold = 221,
                TaskCount = 222,
            };
            oldEntry.RuleResults.Add( "No more Foo!", fooResult );
            //oldEntry.Passed = false;

            var failures = _inspector.ListRunFailureMessages( oldEntry );

            Assert.That( failures.Count(), Is.EqualTo( 1 ) );
            Assert.That( failures[0], Is.EqualTo( "Rule [No more Foo!] has [222] tasks, and it breaks the build if there are over [221] tasks." ) );
        }

        [Test]
        public void When_we_have_no_tasks_in_a_FailOn_Any_Rule_we_do_not_fail()
        {
            Rule rule = new Rule() { ID = "644", Description = "Major problem!", FailOn = RuleFailOn.Any };
            var sourceClauseMatch = new FileTasks();

            _ruleTasks[rule] = sourceClauseMatch;

            var failures = _inspector.CountRunFailures( _ruleTasks );

            Assert.That( failures, Is.EqualTo( 0 ) );
        }

        [Test]
        public void When_we_have_tasks_in_a_FailOn_None_Rule_we_do_not_fail()
        {
            Rule rule = new Rule() { ID = "644", Description="Not a problem.", FailOn = RuleFailOn.None };
            var sourceClauseMatch = new FileTasks();

            SourceFile failedSource = new SourceFile( "some_file.cs" );
            ClauseMatch failedClause = new LineMatch( new List<int> { 1, 44 } );

            sourceClauseMatch[failedSource] = failedClause;

            var rules = new RuleTasks();
            rules[rule] = sourceClauseMatch;

            var failures = _inspector.CountRunFailures( rules );

            Assert.That( failures, Is.EqualTo( 0 ) );
        }

        [Test]
        public void When_we_have_tasks_in_multiple_FailOn_Any_Rules_we_report_all_failures()
        {
            var rule = new Rule() { ID = "191", Description = "Major problem!", FailOn = RuleFailOn.Any };
            var rule2 = new Rule() { ID = "200", Description = "Major problem!", FailOn = RuleFailOn.Any };
            var sourceClauseMatch = new FileTasks();
            var sourceClauseMatch2 = new FileTasks();

            var failedSource = new SourceFile( "some_file.cs" );
            ClauseMatch failedClause = new LineMatch( new List<int> { 1, 44 } );
            sourceClauseMatch[failedSource] = failedClause;

            var failedSource2 = new SourceFile( "some_other_file.cs" );
            ClauseMatch failedClause2 = new LineMatch( new List<int> { 23, 65, 81 } );
            sourceClauseMatch2[failedSource2] = failedClause2;

            _ruleTasks[rule] = sourceClauseMatch;
            _ruleTasks[rule2] = sourceClauseMatch2;

            var failures = _inspector.CountRunFailures( _ruleTasks );

            Assert.That( failures, Is.EqualTo( 2 ) );
        }

        [Test]
        public void When_tasks_increase_on_a_FailOn_Increase_rule_we_break()
        {
            var rule = new Rule()
            {
                ID = "300",
                Description = "Major problem!",
                FailOn = RuleFailOn.Increase,
            };
            var sourceClauseMatch = new FileTasks();

            var failedSource = new SourceFile( "some_file.cs" );
            ClauseMatch failedClause = new LineMatch( new List<int> { 1, 44, 68, 70, 79, 102, 111, 194, 198, 292, 321, 334, 345, 367 } );
            sourceClauseMatch[failedSource] = failedClause;

            _ruleTasks[rule] = sourceClauseMatch;

            var results = new Dictionary<string,RuleResult>();
            results["300"] = new RuleResult { TaskCount = 10 };

            _history.AddEntry( new RunEntry { 
                Date = DateTime.Now.AddDays( -7 ), 
                Number = 1704, 
                RuleResults = results, 
                Passed = true 
            } );

            var failures = _inspector.CountRunFailures( _ruleTasks );

            Assert.That( failures, Is.EqualTo( 1 ) );
        }
    }
}
