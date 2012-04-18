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

        [TestCase( "Copyright update", 22, "4/4/2012 10:25:02 AM", 3403 )]
        [TestCase( "Silly problem", 46, "5/11/2012 7:28:02 AM", 1)]
        public void Checking_for_build_failures_updates_history_file( string changeID, int violationCount, string buildTimeString, int buildNumber )
        {
            DateTime buildDateTime = DateTime.Parse( buildTimeString );
            var expectedHistory = XDocument.Parse( string.Format(
@"<BuildHistory>
  <Build Number=""{3}"" DateTime=""{2}"">
    <Change ID=""{0}"" Violations=""{1}"" />
  </Build>
</BuildHistory>", changeID, violationCount, buildDateTime, buildNumber ) );

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

            string writtenHistory = _checker.GetBuildHistory( _changeViolations, buildDateTime, buildNumber );

            Assert.That( writtenHistory, Is.EqualTo( expectedHistory.ToString() ) );
        }

        [TestCase( 12, "9/14/2012 2:44:02 AM", 60)]
        [TestCase( 14, "5/11/2012 7:28:02 AM", 54)]
        public void We_can_read_a_history_from_XML_to_a_domain_object( int buildNumber, string dateString, int violationsCount )
        {
                       
            var history = XDocument.Parse( string.Format(
@"<BuildHistory>
  <Build Number=""{3}"" DateTime=""{2}"">
    <Change ID=""{0}"" Violations=""{1}"" />
    <Change ID=""always the same"" Violations=""44"" />
  </Build>
  <Build Number=""1100"" DateTime=""1/1/2022 3:20:14 PM"">
    <Change ID=""always the same"" Violations=""44"" />
  </Build>

</BuildHistory>", "silly problem", violationsCount, dateString, buildNumber  ) );


            BuildHistory buildHistory = _checker.ReadBuildHistory( history );

            BuildRun firstRun = buildHistory.BuildRuns[0];
            Assert.That( firstRun.BuildDate, Is.EqualTo( DateTime.Parse( dateString ) ) );
            Assert.That( firstRun.BuildNumber, Is.EqualTo( buildNumber ) );

            Assert.That( firstRun.ChangeViolations["silly problem"], Is.EqualTo( violationsCount ) );
            Assert.That( firstRun.ChangeViolations["always the same"], Is.EqualTo( 44 ) );


            BuildRun secondRun = buildHistory.BuildRuns[1];
            Assert.That( secondRun.BuildDate, Is.EqualTo( DateTime.Parse( "1/1/2022 3:20:14 PM" ) ) );
            Assert.That( secondRun.BuildNumber, Is.EqualTo( 1100 ) );
            Assert.That( secondRun.ChangeViolations.Count(), Is.EqualTo( 1 ) );
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
