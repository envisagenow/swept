﻿//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2012 Jason Cole and Envisage Technologies Corp.
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
    public class BuildFail_tests
    {
        private FailChecker _checker;
        private Dictionary<Change, Dictionary<swept.SourceFile, swept.ClauseMatch>> _changeViolations;

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
        public void Zero_Problems_produces_no_failure_text()
        {
            string failureText = _checker.ReportFailures( new List<string>() );

            Assert.AreEqual( string.Empty, failureText );
        }

        //var xml = checker.ReportFailureXML( failures );
        [Test]
        public void Zero_Problems_produces_no_failure_XML()
        {
            string failureXML = _checker.ReportFailureXML( new List<string>() );

            Assert.AreEqual( string.Empty, failureXML );
        }

        [Test]
        public void When_one_failure_occurs_failure_XML_is_correct()
        {
            List<string> failures = new List<string>();
            string problemText = "fooblah";
            failures.Add( problemText );
            string failureXML = _checker.ReportFailureXML( failures );

            var expectedFailureXML = String.Format( "<{0}s><{0}>{1}</{0}></{0}s>", "SweptRuleFailure", problemText );

            Assert.AreEqual( expectedFailureXML, failureXML );
        }



        [Test]
        public void When_one_failure_occurs_text_is_correct()
        {
            List<string> failures = new List<string>();
            string problemText = "fooblah";
            failures.Add( problemText );
            string failureText = _checker.ReportFailures( failures );

            var expectedFailureMessage = String.Format( "Swept failed due to build breaking rule failure:\n{0}\n", problemText );
            Assert.AreEqual( expectedFailureMessage, failureText );
        }

        [Test]
        public void When_multiple_failures_occur_text_is_correct()
        {
            List<string> failures = new List<string>();
            failures.Add( "fail1" );
            failures.Add( "fail2" );
            
            string problemText = "";
            foreach (string fail in failures)
            {
                problemText += fail + "\n";
            }
            var expectedFailureMessage = String.Format( "Swept failed due to build breaking rule failures:\n{0}", problemText );

            string failureText = _checker.ReportFailures( failures );

            Assert.AreEqual( expectedFailureMessage, failureText );
        }

        [Test]
        public void When_we_violate_a_FailAny_Change_we_fail()
        {
            Change change = new Change() { ID = "644", Description = "Major problem!", BuildFail = BuildFailMode.Any };
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
        public void When_we_violate_a_BuildFail_None_Change_we_do_not_fail()
        {
            Change change = new Change() { ID = "644", Description="Not a problem.", BuildFail = BuildFailMode.None };
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
            Change change = new Change() { ID = "191", Description = "Major problem!", BuildFail = BuildFailMode.Any };
            Change change2 = new Change() { ID = "200", Description = "Major problem!", BuildFail = BuildFailMode.Any };
            Dictionary<SourceFile, ClauseMatch> sourceClauseMatch = new Dictionary<SourceFile, ClauseMatch>();
            Dictionary<SourceFile, ClauseMatch> sourceClauseMatch2 = new Dictionary<SourceFile, ClauseMatch>();

            SourceFile failedSource = new SourceFile( "some_file.cs" );
            ClauseMatch failedClause = new LineMatch( new List<int> { 1, 44 } );
            sourceClauseMatch[failedSource] = failedClause;

            SourceFile failedSource2 = new SourceFile( "some_other_file.cs" );
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
            Change change = new Change()
            {
                ID = "200",
                Description = "Major problem!",
                BuildFail = BuildFailMode.Over,
                BuildFailOverLimit = 2
            };
            Dictionary<SourceFile, ClauseMatch> sourceClauseMatch = new Dictionary<SourceFile, ClauseMatch>();

            SourceFile failedSource = new SourceFile( "some_other_file.cs" );
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
            Change change = new Change()
            {
                ID = "191",
                Description = "Major problem!",
                BuildFail = BuildFailMode.Over,
                BuildFailOverLimit = 2
            };
            Dictionary<SourceFile, ClauseMatch> sourceClauseMatch = new Dictionary<SourceFile, ClauseMatch>();

            SourceFile failedSource = new SourceFile( "some_file.cs" );
            ClauseMatch failedClause = new LineMatch( new List<int> { 1, 44 } );
            sourceClauseMatch[failedSource] = failedClause;

            _changeViolations[change] = sourceClauseMatch;

            var failures = _checker.Check( _changeViolations );

            Assert.That( failures.Count(), Is.EqualTo( 0 ) );
        }
    }
}
