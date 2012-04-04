//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using System.Xml.Linq;

namespace swept.Tests
{
    [TestFixture]
    public class BuildFailReporting_tests
    {
        private BuildReporter _reporter;
        private Dictionary<Change, Dictionary<SourceFile, ClauseMatch>> _changeViolations;
        private FailChecker _checker;

        [SetUp]
        public void Setup()
        {
            _reporter = new BuildReporter();
            _changeViolations = new Dictionary<Change, Dictionary<SourceFile, ClauseMatch>>();
            _checker = new FailChecker();
        }

        #region Command line build fail messages
        [Test]
        public void Zero_Problems_produces_no_failure_text()
        {
            string failureText = _reporter.ReportBuildFailures( new List<string>() );

            Assert.AreEqual( string.Empty, failureText );
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

            string failureText = _reporter.ReportBuildFailures( failures );

            Assert.AreEqual( expectedFailureMessage, failureText );
        }

        [Test]
        public void When_one_failure_occurs_text_is_correct()
        {
            List<string> failures = new List<string>();
            string problemText = "fooblah";
            failures.Add( problemText );
            string failureText = _reporter.ReportBuildFailures( failures );

            var expectedFailureMessage = String.Format( "Swept failed due to build breaking rule failure:\n{0}\n", problemText );
            Assert.AreEqual( expectedFailureMessage, failureText );
        }
        #endregion

        [TestCase( "Copyright update", 22 )]
        [TestCase( "Silly problem", 46 )]
        public void Checking_for_build_failures_updates_history_file( string changeID, int violationCount )
        {
            var expectedHistory = XDocument.Parse( string.Format( @"<BuildHistory>
  <Build Number=""3403"" DateTime=""4/4/2012 10:25 AM"">
    <Change ID=""{0}"" Violations=""{1}"" />
  </Build>
</BuildHistory>", changeID, violationCount ) );

            var change = new Change()
            {
                ID = changeID,
                Description = "Time marches on",
                BuildFail = BuildFailMode.Over,
                BuildFailOverLimit = 20
            };
            var sourceClauseMatch = new Dictionary<SourceFile, ClauseMatch>();

            var failedSource = new SourceFile( "some_file.cs" );

            List<int> violationLines = new List<int>();
            for (int i = 0; i < violationCount; i++)
            {
                violationLines.Add( (i * 7) + 22 );  //arbitrary lines throughout the source file had this problem.
            }
            ClauseMatch failedClause = new LineMatch( violationLines );
            sourceClauseMatch[failedSource] = failedClause;

            _changeViolations[change] = sourceClauseMatch;

            string writtenHistory = _checker.GetBuildHistory( _changeViolations );

            Assert.That( writtenHistory, Is.EqualTo( expectedHistory.ToString() ) );
        }

        [Test]
        public void Zero_Problems_produces_empty_failure_XML()
        {
            var failureXML = _reporter.GenerateBuildFailureXML( new List<string>() );

            Assert.AreEqual( "<SweptBuildFailures />", failureXML.ToString() );
        }

        [Test]
        public void When_one_failure_occurs_failure_XML_is_correct()
        {
            List<string> failures = new List<string>();
            string problemText = "fooblah";
            failures.Add( problemText );
            XElement failureXML = _reporter.GenerateBuildFailureXML( failures );

            var expectedFailureXML =
@"<SweptBuildFailures>
  <SweptBuildFailure>fooblah</SweptBuildFailure>
</SweptBuildFailures>";

            Assert.AreEqual( expectedFailureXML, failureXML.ToString() );
        }

        [Test]
        public void When_multiple_failures_occur_XML_is_correct()
        {
            List<string> failures = new List<string>();
            failures.Add( "fail1" );
            failures.Add( "fail2" );
            XElement failureXML = _reporter.GenerateBuildFailureXML( failures );

            var expectedFailureXML =
@"<SweptBuildFailures>
  <SweptBuildFailure>fail1</SweptBuildFailure>
  <SweptBuildFailure>fail2</SweptBuildFailure>
</SweptBuildFailures>";

            Assert.AreEqual( expectedFailureXML, failureXML.ToString() );
        }
    }
}
