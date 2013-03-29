//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using System.Xml.Linq;
using swept.DSL;

namespace swept.Tests
{
    [TestFixture]
    public class Reporting_tests
    {
        private BuildReporter _reporter;

        [SetUp]
        public void Setup()
        {
            _reporter = new BuildReporter();
        }

        #region Command line build fail messages
        [Test]
        public void Zero_Problems_produces_no_failure_text()
        {
            string failureText = _reporter.ReportBuildFailures( new List<string>() );
            
            Assert.AreEqual( string.Empty, failureText );
        }

        [Test]
        public void When_one_failure_occurs_text_is_correct()
        {
            string problemText = "fooblah";
            var failures = new List<string> { problemText };
            string failureText = _reporter.ReportBuildFailures( failures );

            var expectedFailureMessage = String.Format( "Swept failed due to build breaking rule failure:\n{0}\n", problemText );
            Assert.AreEqual( expectedFailureMessage, failureText );
        }

        [Test]
        public void When_multiple_failures_occur_text_is_correct()
        {
            var failures = new List<string>();
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
        #endregion

        [Test]
        public void We_see_failure_list_when_we_Check()
        {
            var runHistory = new RunHistory();
            RunHistoryEntry entry = new RunHistoryEntry { Passed = true, Number = 1 };
            entry.RuleResults["NET-001"] = new HistoricRuleResult { Violations = 4 };
            runHistory.AddEntry( entry );

            var net_001 = new Rule { ID = "NET-001", FailOn = RuleFailOn.Increase };

            FileTasks net_001_problems = new FileTasks();
            var file = new SourceFile( "troubled.cs" );
            var lines = new List<int>( new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 } );
            var match = new LineMatch( lines );
            net_001_problems[file] = match;

            var RuleTasks = new RuleTasks();
            RuleTasks[net_001] = net_001_problems;

            // I think this is moot? _librarian.ReportOn( RuleTasks, runHistory );
            var inspector = new RunInspector( runHistory );
            var failures = inspector.ListRunFailures( RuleTasks );
            string message = _reporter.ReportFailures( failures );

            string expectedMessage = "Error:  Rule [NET-001] has been violated [9] times, and it breaks the build if there are over [4] violations.\r\n";
            Assert.That( message, Is.EqualTo( expectedMessage ) );
        }

        // What's to be done about these?
        //[Test]
        //public void We_see_expected_header_when_we_Check()
        //{
        //    var args = new Arguments( new string[] { "library:foo.library", "history:foo.history", "check" }, _storage );
        //    _librarian = new BuildLibrarian( args, _storage );

        //    var nowish = DateTime.Parse( "6/26/2012 10:58 AM" );
        //    string header = _reporter.GetConsoleHeader( nowish );

        //    Assert.That( header, Is.EqualTo( "Swept checking [r:\\somefolder] with rules in [r:\\somefolder\\foo.library] on 6/26/2012 10:58:00 AM...\r\n" ) );
        //}

        //[Test]
        //public void We_see_no_header_for_default_run()
        //{
        //    var nowish = DateTime.Parse( "6/26/2012 10:58 AM" );
        //    string header = _reporter.GetConsoleHeader( nowish );

        //    Assert.That( header, Is.Empty );
        //}

        [Test]
        public void With_no_violations_the_check_passes()
        {
            string message = _reporter.ReportCheckResult( new List<string>() );

            Assert.That( message, Is.EqualTo( "Swept check passed!" + Environment.NewLine ) );
        }

        [Test]
        public void With_one_violation_the_check_fails()
        {
            string message = _reporter.ReportCheckResult( new List<string> { "I'm agonized about this." } );

            string expectedMessage = "Swept check failed!" + Environment.NewLine;
            Assert.That( message, Is.EqualTo( expectedMessage ) );
        }

        [Test]
        public void With_violations_the_check_fails()
        {
            List<string> failures = new List<string>();
            string problem = "Rule [NET-001] has been violated [22] times, and it breaks the build if there are over [18] violations.";
            string anotherProblem = "Rule [ETC-002] has been violated [7] times, and it breaks the build if there are over [6] violations.";
            failures.Add( problem );
            failures.Add( anotherProblem );
            string message = _reporter.ReportCheckResult( failures );

            string expectedMessage = "Swept check failed!" + Environment.NewLine;
            Assert.That( message, Is.EqualTo( expectedMessage ) );
        }

        [Test]
        public void Zero_failures_and_zero_fixes_produces_empty_delta_XML()
        {
            var delta = new BreakageDelta { Failures = new List<string>(), Fixes = new List<string>() };
            var deltaXml = _reporter.GenerateBuildDeltaXml( delta );

            Assert.AreEqual( "<SweptBuildDeltas />", deltaXml.ToString() );
        }

        [Test]
        public void When_one_failure_occurs_delta_XML_is_correct()
        {
            var failures = new List<string> { "fooblah" };
            var delta = new BreakageDelta { Failures = failures, Fixes = new List<string>() };
            XElement deltaXml = _reporter.GenerateBuildDeltaXml( delta );

            var expectedDeltaText =
@"<SweptBuildDeltas>
  <SweptBuildFailure>fooblah</SweptBuildFailure>
</SweptBuildDeltas>";

            Assert.AreEqual( expectedDeltaText, deltaXml.ToString() );
        }

        [Test]
        public void When_multiple_failures_occur_XML_is_correct()
        {
            var failures = new List<string> { "fail1", "fail2" };
            var delta = new BreakageDelta { Failures = failures, Fixes = new List<string>() };
            XElement deltaXml = _reporter.GenerateBuildDeltaXml( delta );

            var expectedDeltaText =
@"<SweptBuildDeltas>
  <SweptBuildFailure>fail1</SweptBuildFailure>
  <SweptBuildFailure>fail2</SweptBuildFailure>
</SweptBuildDeltas>";

            Assert.AreEqual( expectedDeltaText, deltaXml.ToString() );
        }

//        [Test]
//        public void When_one_fix_occurs_XML_is_correct()
//        {
//            var failures = new List<string>();

//            var priorRun = new RunHistoryEntry { Passed = false, Number = 41 };
//            priorRun.RuleResults["fail1"] = new RuleResult { Violations = 6, Breaking = true };
//            var history = new RunHistory();
//            history.AddRun( priorRun );

//            XElement deltaXml = _librarian.GenerateBuildDeltaXml( failures, history );

//            var expectedDeltaText =
//@"<SweptBuildDeltas>
//  <SweptBuildFix>fail1</SweptBuildFix>
//</SweptBuildDeltas>";

//            Assert.AreEqual( expectedDeltaText, deltaXml.ToString() );
//        }

        [Test]
        public void When_one_fix_occurs_XML_is_correct()
        {
            var fixes = new List<string> { "Fix1!!1" };
            var delta = new BreakageDelta { Failures = new List<string>(), Fixes = fixes };

            XElement deltaXml = _reporter.GenerateBuildDeltaXml( delta );

            var expectedDeltaText =
@"<SweptBuildDeltas>
  <SweptBuildFix>Fix1!!1</SweptBuildFix>
</SweptBuildDeltas>";

            Assert.AreEqual( expectedDeltaText, deltaXml.ToString() );
        }


        [Test]
        public void No_task_data_creates_empty_report()
        {
            string empty_report = "<SweptBuildReport TotalTasks=\"0\" />";

            string report = new BuildReporter().ReportDetailsXml( new RuleTasks() );

            Assert.That( report, Is.EqualTo( empty_report ) );
        }

        [Test]
        public void single_Rule_single_SourceFile_report()
        {
            var expectedReport = XDocument.Parse(
@"<SweptBuildReport TotalTasks='4'>
    <Rule ID='HTML 01' Description='Improve browser compatibility' TotalTasks='4'>
        <SourceFile Name='bar.htm' TaskCount='4' />
    </Rule>
</SweptBuildReport>"
            );

            var ruleTasks = new RuleTasks();

            var rule = new Rule
            {
                ID = "HTML 01",
                Subquery = new QueryLanguageNode { Language = FileLanguage.HTML },
                Description = "Improve browser compatibility"
            };

            var bar = new SourceFile( "bar.htm" );

            var fileMatches = new FileTasks();
            fileMatches[bar] = new LineMatch( new List<int> { 1, 12, 123, 1234 } );
            ruleTasks.Add( rule, fileMatches );

            string report = _reporter.ReportDetailsXml( ruleTasks );

            Assert.That( report, Is.EqualTo( expectedReport.ToString() ) );
        }

        [Test]
        public void multiple_Rule_multiple_SourceFile_report()
        {
            var expectedReport = XDocument.Parse(
@"
<SweptBuildReport TotalTasks='10'>
    <Rule 
        ID='DomainEvents 01' 
        Description='Use DomainEvents instead of AcadisUserPersister and AuditRecordPersister'
        TotalTasks='4'>
        
        <SourceFile Name='foo.cs' TaskCount='1' />
        <SourceFile Name='goo.cs' TaskCount='3' />
    </Rule>
    <Rule 
        ID='HTML 01' 
        Description='Improve browser compatibility across IE versions'
        TotalTasks='6'>

        <SourceFile Name='bar.htm' TaskCount='4' />
        <SourceFile Name='shmoo.aspx' TaskCount='2' />
    </Rule>
</SweptBuildReport>
"
            );

            var csharpRule = new Rule
            {
                ID = "DomainEvents 01",
                Description = "Use DomainEvents instead of AcadisUserPersister and AuditRecordPersister"
            };

            SourceFile foo = new SourceFile( "foo.cs" );
            SourceFile goo = new SourceFile( "goo.cs" );

            var csharpFiles = new FileTasks();
            csharpFiles[foo] = new FileMatch( true );
            csharpFiles[goo] = new LineMatch( new List<int> { 1, 2, 3 } );

            var htmlRule = new Rule
            {
                ID = "HTML 01",
                Subquery = new QueryLanguageNode { Language = FileLanguage.HTML },
                Description = "Improve browser compatibility across IE versions"
            };

            SourceFile bar = new SourceFile( "bar.htm" );
            SourceFile shmoo = new SourceFile( "shmoo.aspx" );

            var htmlFiles = new FileTasks();
            htmlFiles[bar] = new LineMatch( new List<int> { 1, 2, 3, 4 } );
            htmlFiles[shmoo] = new LineMatch( new List<int> { 8, 12 } );

            var rules = new RuleTasks();
            rules[csharpRule] = csharpFiles;
            rules[htmlRule] = htmlFiles;

            string report = _reporter.ReportDetailsXml( rules );

            Assert.That( report, Is.EqualTo( expectedReport.ToString() ) );
        }

        [Test]
        public void Files_with_false_FileMatch_not_added()
        {
            var expectedReport = XDocument.Parse(
@"
<SweptBuildReport TotalTasks='1'>
    <Rule 
        ID='DomainEvents 01' 
        Description='Use DomainEvents instead of AcadisUserPersister and AuditRecordPersister'
        TotalTasks='1'>
        
        <SourceFile Name='foo.cs' TaskCount='1' />
    </Rule>
</SweptBuildReport>
"
            );

            var csharpRule = new Rule
            {
                ID = "DomainEvents 01",
                Description = "Use DomainEvents instead of AcadisUserPersister and AuditRecordPersister"
            };

            SourceFile foo = new SourceFile( "foo.cs" );
            SourceFile goo = new SourceFile( "goo.cs" );
            SourceFile boo = new SourceFile( "boo.cs" );

            var csharpFiles = new FileTasks();
            csharpFiles[foo] = new FileMatch( true );
            csharpFiles[goo] = new FileMatch( false );
            csharpFiles[boo] = new LineMatch( new int[] { } );

            var ruleTasks = new RuleTasks();
            ruleTasks[csharpRule] = csharpFiles;

            string report = _reporter.ReportDetailsXml( ruleTasks );

            Assert.That( report, Is.EqualTo( expectedReport.ToString() ) );
        }


    }
}
