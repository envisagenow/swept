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
    public class RunFailReporting_tests
    {
        private BuildLibrarian _librarian;
        private MockStorageAdapter _storage;

        [SetUp]
        public void Setup()
        {
            _storage = new MockStorageAdapter();
            _storage.CWD = "r:\\somefolder";
            var args = new Arguments( new string[] { "library:foo.library", "history:foo.history" }, _storage, Console.Out );
            _librarian = new BuildLibrarian( args, _storage );
        }

        #region Command line build fail messages
        [Test]
        public void Zero_Problems_produces_no_failure_text()
        {
            string failureText = _librarian.ReportBuildFailures();

            Assert.AreEqual( string.Empty, failureText );
        }

        [Test]
        public void When_multiple_failures_occur_text_is_correct()
        {
            List<string> failures = new List<string>();
            failures.Add( "fail1" );
            failures.Add( "fail2" );

            _librarian._failures = failures;

            string problemText = "";
            foreach (string fail in failures)
            {
                problemText += fail + "\n";
            }
            var expectedFailureMessage = String.Format( "Swept failed due to build breaking rule failures:\n{0}", problemText );

            string failureText = _librarian.ReportBuildFailures();

            Assert.AreEqual( expectedFailureMessage, failureText );
        }

        [Test]
        public void When_one_failure_occurs_text_is_correct()
        {
            string problemText = "fooblah";
            var failures = new List<string> { problemText };
            _librarian._failures = failures;
            string failureText = _librarian.ReportBuildFailures();

            var expectedFailureMessage = String.Format( "Swept failed due to build breaking rule failure:\n{0}\n", problemText );
            Assert.AreEqual( expectedFailureMessage, failureText );
        }
        #endregion

        [Test]
        public void We_see_failure_list_when_we_Check()
        {
            var args = new Arguments( new string[] { "library:foo.library", "history:foo.history", "check" }, _storage, Console.Out );
            _librarian = new BuildLibrarian( args, _storage );

            var history = new RunHistory();
            RunHistoryEntry entry = new RunHistoryEntry { Passed = true, Number = 1 };
            entry.Violations["NET-001"] = 4;
            history.AddRun( entry );

            var net_001 = new Rule { ID = "NET-001", RunFail = RunFailMode.Increase };

            FileProblems net_001_problems = new FileProblems();
            var file = new SourceFile( "troubled.cs" );
            var lines = new List<int>( new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 } );
            var match = new LineMatch( lines );
            net_001_problems[file] = match;

            Dictionary<Rule, FileProblems> problems = new Dictionary<Rule, FileProblems>();
            problems[net_001] = net_001_problems;

            _librarian.ReportOn( problems, history );
            string message = _librarian.ReportFailures();

            string expectedMessage = "Rule [NET-001] has been violated [9] times, and it breaks the build if there are over [4] violations.\r\n";
            Assert.That( message, Is.EqualTo( expectedMessage ) );
        }

        [Test]
        public void We_see_expected_header_when_we_Check()
        {
            var args = new Arguments( new string[] { "library:foo.library", "history:foo.history", "check" }, _storage, Console.Out );
            _librarian = new BuildLibrarian( args, _storage );

            var nowish = DateTime.Parse( "6/26/2012 10:58 AM" );
            string header = _librarian.GetConsoleHeader( nowish );

            string library = System.IO.Path.Combine(_storage.CWD, "foo.library");
            string expectedMessage = String.Format( "Swept checking [{0}] with rules in [{1}] on {2}...{3}", _storage.CWD, library, nowish.ToString( "G" ), Environment.NewLine );
            Assert.That( header, Is.EqualTo( expectedMessage ) );
        }

        [Test]
        public void We_see_no_header_for_default_run()
        {
            var nowish = DateTime.Parse( "6/26/2012 10:58 AM" );
            string header = _librarian.GetConsoleHeader( nowish );

            Assert.That( header, Is.Empty );
        }

        [Test]
        public void With_no_violations_the_check_report_is_cheerful()
        {
            Dictionary<Rule, FileProblems> problems = new Dictionary<Rule, FileProblems>();
            string message = _librarian.ReportCheckResult();

            Assert.That( message, Is.EqualTo( "Swept check passed!" + Environment.NewLine ) );
        }

        [Test]
        public void With_a_violation_the_check_report_complains()
        {
            var history = new RunHistory();
            RunHistoryEntry entry = new RunHistoryEntry { Passed = true, Number = 1 };
            entry.Violations["NET-001"] = 4;
            history.AddRun( entry );

            var net_001 = new Rule { ID = "NET-001", RunFail = RunFailMode.Increase, Description = "Good exception messages, please" };

            FileProblems net_001_problems = new FileProblems();
            var file = new SourceFile( "troubled.cs" );
            var lines = new List<int>( new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 } );
            var match = new LineMatch( lines );
            net_001_problems[file] = match;

            Dictionary<Rule, FileProblems> problems = new Dictionary<Rule, FileProblems>();
            problems[net_001] = net_001_problems;
            _librarian.ReportOn( problems, history );
            string message = _librarian.ReportCheckResult();

            //string expectedMessage = "Rule [NET-001] has been violated [9] times, and it breaks the build if there are over [4] violations.\r\n";
            string expectedMessage = "Swept check failed!" + Environment.NewLine;
            Assert.That( message, Is.EqualTo( expectedMessage ) );
        }

        [Test, Ignore()]
        public void With_violations_the_check_report_complains()
        {
            //List<string> problemLines = new List<string>();
            //string problem = "Rule [NET-001] has been violated [22] times, and it breaks the build if there are over [18] violations.";
            //string anotherProblem = "Rule [ETC-002] has been violated [7] times, and it breaks the build if there are over [6] violations.";
            //problemLines.Add( problem );
            //problemLines.Add( anotherProblem );
            //string message = _librarian.ReportCheckResult( problemLines );

            //string expectedMessage = problem + Environment.NewLine + anotherProblem + Environment.NewLine;
            //Assert.That( message, Is.EqualTo( expectedMessage ) );
        }


        [Test]
        public void Zero_Problems_produces_empty_failure_XML()
        {
            _librarian._failures = new List<string>();
            var failureXML = _librarian.GenerateBuildFailureXML();

            Assert.AreEqual( "<SweptBuildFailures />", failureXML.ToString() );
        }

        [Test]
        public void When_one_failure_occurs_failure_XML_is_correct()
        {
            List<string> failures = new List<string> { "fooblah" };
            _librarian._failures = failures;
            XElement failureXML = _librarian.GenerateBuildFailureXML();

            var expectedFailureXML =
@"<SweptBuildFailures>
  <SweptBuildFailure>fooblah</SweptBuildFailure>
</SweptBuildFailures>";

            Assert.AreEqual( expectedFailureXML, failureXML.ToString() );
        }

        [Test]
        public void When_multiple_failures_occur_XML_is_correct()
        {
            var failures = new List<string> { "fail1", "fail2" };
            _librarian._failures = failures;
            XElement failureXML = _librarian.GenerateBuildFailureXML();

            var expectedFailureXML =
@"<SweptBuildFailures>
  <SweptBuildFailure>fail1</SweptBuildFailure>
  <SweptBuildFailure>fail2</SweptBuildFailure>
</SweptBuildFailures>";

            Assert.AreEqual( expectedFailureXML, failureXML.ToString() );
        }
    }
}
