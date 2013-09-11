//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2013 Jason Cole and Envisage Technologies Corp.
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

        const string PassedMessage = "Swept check passed.\r\n";
        const string FailedMessage = "Swept check failed!\r\n";
        const string EmptyReport = "<SweptBuildReport TotalTasks=\"0\" TotalFlags=\"0\" />";

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

        const string rawProblemText = "Rule [NET-001] has [9] tasks, and it breaks the build if there are over [4] tasks.";
        const string expectedMessage = "Error:  " + rawProblemText + "\r\n";
        [Test]
        public void We_see_failure_list_when_we_Check()
        {
            List<string> failures = new List<string> { rawProblemText };
            string message = _reporter.ReportFailures( failures );

            Assert.That( message, Is.EqualTo( expectedMessage ) );
        }

        [Test]
        public void With_no_violations_the_check_passes()
        {
            string message = _reporter.ReportCheckResult( new List<string>() );

            Assert.That( message, Is.EqualTo( PassedMessage ) );
        }

        [Test]
        public void With_one_violation_the_check_fails()
        {
            string message = _reporter.ReportCheckResult( new List<string> { "I'm agonized about this." } );

            Assert.That( message, Is.EqualTo( FailedMessage ) );
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

            Assert.That( message, Is.EqualTo( FailedMessage ) );
        }

        [Test]
        public void No_task_data_creates_empty_report()
        {
            string report = new BuildReporter().ReportDetailsXml( new RuleTasks() );

            Assert.That( report, Is.EqualTo( EmptyReport ) );
        }

        [Test]
        public void single_Rule_single_SourceFile_report()
        {
            var expectedReport = XDocument.Parse(
@"<SweptBuildReport TotalTasks='4' TotalFlags='0'>
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
<SweptBuildReport TotalTasks='10' TotalFlags='0'>
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
<SweptBuildReport TotalTasks='1' TotalFlags='0'>
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
