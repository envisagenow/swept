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
        private FailChecker _checker;
        private Dictionary<Change, Dictionary<SourceFile, ClauseMatch>> _changeViolations;

        [SetUp]
        public void Setup()
        {
            _checker = new FailChecker();
            _changeViolations = new Dictionary<Change, Dictionary<SourceFile, ClauseMatch>>();
        }

        [Test]
        public void Zero_Violations_do_not_fail()
        {
            var problems = _checker.Check( _changeViolations );
            Assert.That( problems.Count(), Is.EqualTo( 0 ) );
        }

        [Test]
        public void When_we_violate_a_FailAny_Change_we_fail()
        {
            Change change = new Change() { ID = "644", Description = "Major problem!", RunFail = RunFailMode.Any };
            Dictionary<SourceFile, ClauseMatch> sourceClauseMatch = new Dictionary<SourceFile, ClauseMatch>();

            SourceFile failedSource = new SourceFile( "some_file.cs" );
            ClauseMatch failedClause = new LineMatch( new List<int> { 1, 44 } );

            sourceClauseMatch[failedSource] = failedClause;

            _changeViolations[change] = sourceClauseMatch;

            var failures = _checker.Check( _changeViolations );

            Assert.That( failures.Count(), Is.EqualTo( 1 ) );
            Assert.That( failures[0], Is.EqualTo( "Rule [644] has been violated, and it breaks the build if there are any violations." ) );
        }

        [Test]
        public void When_we_have_no_tasks_in_a_FailAny_Change_we_do_not_fail()
        {
            Change change = new Change() { ID = "644", Description = "Major problem!", RunFail = RunFailMode.Any };
            Dictionary<SourceFile, ClauseMatch> sourceClauseMatch = new Dictionary<SourceFile, ClauseMatch>();

            _changeViolations[change] = sourceClauseMatch;

            var failures = _checker.Check( _changeViolations );

            Assert.That( failures.Count(), Is.EqualTo( 0 ) );
        }

        [Test]
        public void When_we_violate_a_BuildFail_None_Change_we_do_not_fail()
        {
            Change change = new Change() { ID = "644", Description="Not a problem.", RunFail = RunFailMode.None };
            Dictionary<SourceFile, ClauseMatch> sourceClauseMatch = new Dictionary<SourceFile, ClauseMatch>();

            SourceFile failedSource = new SourceFile( "some_file.cs" );
            ClauseMatch failedClause = new LineMatch( new List<int> { 1, 44 } );

            sourceClauseMatch[failedSource] = failedClause;

            var changes = new Dictionary< Change, Dictionary<SourceFile, ClauseMatch> >();
            changes[change] = sourceClauseMatch;

            var failures = _checker.Check( changes );

            Assert.That( failures.Count(), Is.EqualTo( 0 ) );
        }

        [Test]
        public void When_we_violate_multiple_BuildFail_Any_Changes_we_report_all_failures()
        {
            var change = new Change() { ID = "191", Description = "Major problem!", RunFail = RunFailMode.Any };
            var change2 = new Change() { ID = "200", Description = "Major problem!", RunFail = RunFailMode.Any };
            var sourceClauseMatch = new Dictionary<SourceFile, ClauseMatch>();
            var sourceClauseMatch2 = new Dictionary<SourceFile, ClauseMatch>();

            var failedSource = new SourceFile( "some_file.cs" );
            ClauseMatch failedClause = new LineMatch( new List<int> { 1, 44 } );
            sourceClauseMatch[failedSource] = failedClause;

            var failedSource2 = new SourceFile( "some_other_file.cs" );
            ClauseMatch failedClause2 = new LineMatch( new List<int> { 23, 65, 81 } );
            sourceClauseMatch2[failedSource2] = failedClause2;

            _changeViolations[change] = sourceClauseMatch;
            _changeViolations[change2] = sourceClauseMatch2;

            var failures = _checker.Check( _changeViolations );

            Assert.That( failures.Count(), Is.EqualTo( 2 ) );
            Assert.That( failures[0], Is.EqualTo( "Rule [191] has been violated, and it breaks the build if there are any violations." ) );
            Assert.That( failures[1], Is.EqualTo( "Rule [200] has been violated, and it breaks the build if there are any violations." ) );
        }

        [Test]
        public void When_we_exceed_BuildFail_Over_limit_we_fail()
        {
            var change = new Change() {
                ID = "200",
                Description = "Major problem!",
                RunFail = RunFailMode.Over,
                RunFailOverLimit = 2
            };
            var sourceClauseMatch = new Dictionary<SourceFile, ClauseMatch>();

            var failedSource = new SourceFile( "some_other_file.cs" );
            ClauseMatch failedClause = new LineMatch( new List<int> { 23, 65, 81 } );
            sourceClauseMatch[failedSource] = failedClause;

            _changeViolations[change] = sourceClauseMatch;

            var failures = _checker.Check( _changeViolations );

            Assert.That( failures.Count(), Is.EqualTo( 1 ) );
            Assert.That( failures[0], Is.EqualTo( "Rule [200] has been violated [3] times, and it breaks the build if there are over [2] violations." ) );
        }

        [Test]
        public void When_we_violate_BuildFail_Over_but_do_not_exceed_limit_we_do_not_fail()
        {
            var change = new Change() {
                ID = "191",
                Description = "Major problem!",
                RunFail = RunFailMode.Over,
                RunFailOverLimit = 2
            };
            var sourceClauseMatch = new Dictionary<SourceFile, ClauseMatch>();

            var failedSource = new SourceFile( "some_file.cs" );
            ClauseMatch failedClause = new LineMatch( new List<int> { 1, 44 } );
            sourceClauseMatch[failedSource] = failedClause;

            _changeViolations[change] = sourceClauseMatch;

            var failures = _checker.Check( _changeViolations );

            Assert.That( failures.Count(), Is.EqualTo( 0 ) );
        }
    }
}
