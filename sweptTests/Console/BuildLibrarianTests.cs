//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using NUnit.Framework;
using swept.DSL;
using System.IO;

namespace swept.Tests
{
    [TestFixture]
    public class BuildLibrarianTests
    {
        private BuildLibrarian _librarian;
        private MockStorageAdapter _storage;
        private Arguments _args;

        [SetUp]
        public void SetUp()
        {
            _storage = new MockStorageAdapter();
            _args = new Arguments( new string[] { "library:foo.library", "history:foo.history" }, _storage, Console.Out );
            _librarian = new BuildLibrarian( _args, _storage );
        }


        [Test]
        public void we_can_read_run_history_from_disk()
        {
            _storage.RunHistory = XDocument.Parse(
@"<RunHistory>
  <Run Number=""22"" DateTime=""4/4/2012 10:25:02 AM"" Passed=""True"">
    <Rule ID=""foo"" Violations=""2"" />
  </Run>
</RunHistory>" );
            
            var runHistory = _librarian.ReadRunHistory();

            Assert.That( runHistory.Runs.Count, Is.EqualTo(1) );
        }

        [Test]
        public void With_no_violations_the_check_report_is_cheerful()
        {
            Dictionary<Rule, FileProblems> problems = new Dictionary<Rule, FileProblems>();
            string message = _librarian.ReportCheckResult( problems, null );

            Assert.That( message, Is.EqualTo( "Swept check passed!" + Environment.NewLine ) );
        }

        [Test, Ignore("new setup needed")]
        public void With_a_violation_the_check_report_complains()
        {
            var history = new RunHistory();
            RunHistoryEntry entry = new RunHistoryEntry { Passed = true, Number = 1 };
            entry.Violations["NET-001"] = 18;
            history.AddRun( entry );


            var net_001 = new Rule { ID = "NET-001", RunFail = RunFailMode.Increase };


            Dictionary<Rule, FileProblems> problems = new Dictionary<Rule, FileProblems>();
            FileProblems net_001_problems = new FileProblems();

            problems[net_001] = net_001_problems;
            string message = _librarian.ReportCheckResult( problems, null );


            //string problem = "Rule [NET-001] has been violated [22] times, and it breaks the build if there are over [18] violations.";

            //List<string> problemLines = new List<string>();
            //problemLines.Add( problem );
            //string message = _librarian.ReportCheckResult( problemLines );

            //Assert.That( message, Is.EqualTo( problem + Environment.NewLine ) );
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
        public void When_run_history_is_missing_a_new_one_is_created()
        {
            _storage.RunHistoryNotFoundException = new FileNotFoundException();

            var runHistory = _librarian.ReadRunHistory();
            Assert.That( runHistory.Runs.Count, Is.EqualTo( 0 ) );
        }



        [Test]
        public void When_we_write_run_history_it_is_stored_to_disk()
        {
            var runHistory = new RunHistory();
            var violations = new Dictionary<string, int>();
            violations.Add( "foo", 2 );
            runHistory.AddRun( new RunHistoryEntry
            {
                Number = 22,
                Date = DateTime.Parse( "4/4/2012 10:25:02 AM" ),
                Violations = violations,
                Passed = false
            } );

            _librarian.WriteRunHistory( runHistory );

            Dictionary<string, int> violationsNext = new Dictionary<string, int>();
            violationsNext.Add( "bar", 0 );
            runHistory.AddRun( new RunHistoryEntry
            {
                Number = 23,
                Date = DateTime.Parse( "4/7/2012 10:25:03 AM" ),
                Violations = violationsNext,
                Passed = true
            } );

            _librarian.WriteRunHistory( runHistory );


            var expectedHistory =
@"<RunHistory>
  <Run Number=""22"" DateTime=""4/4/2012 10:25:02 AM"" Passed=""False"">
    <Rule ID=""foo"" Violations=""2"" />
  </Run>
  <Run Number=""23"" DateTime=""4/7/2012 10:25:03 AM"" Passed=""True"">
    <Rule ID=""bar"" Violations=""0"" />
  </Run>
</RunHistory>";

            Assert.That( _storage.RunHistory.ToString(), Is.EqualTo( expectedHistory ) );
        }

        [Test]
        public void No_task_data_creates_empty_report()
        {
            string empty_report = "<SweptBuildReport TotalTasks=\"0\" />";

            string report = _librarian.ReportOn( new Dictionary<Rule, FileProblems>(), null );

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

            var rules = new Dictionary<Rule, FileProblems>();

            var rule = new Rule
            {
                ID = "HTML 01",
                Subquery = new QueryLanguageNode { Language = FileLanguage.HTML },
                Description = "Improve browser compatibility"
            };

            var bar = new SourceFile( "bar.htm" );

            var fileMatches = new FileProblems();
            fileMatches[bar] = new LineMatch( new List<int> { 1, 12, 123, 1234 } );
            rules.Add( rule, fileMatches );

            string report = _librarian.ReportOn( rules, null );

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

            SourceFile foo = new SourceFile("foo.cs");
            SourceFile goo = new SourceFile("goo.cs");

            var csharpFiles = new FileProblems();
            csharpFiles[foo] = new FileMatch(true);
            csharpFiles[goo] = new LineMatch(new List<int> { 1, 2, 3 });

            var htmlRule = new Rule
            {
                ID = "HTML 01",
                Subquery = new QueryLanguageNode { Language = FileLanguage.HTML },
                Description = "Improve browser compatibility across IE versions"
            };

            SourceFile bar = new SourceFile("bar.htm");
            SourceFile shmoo = new SourceFile("shmoo.aspx");

            var htmlFiles = new FileProblems();
            htmlFiles[bar] = new LineMatch(new List<int> { 1, 2, 3, 4 });
            htmlFiles[shmoo] = new LineMatch(new List<int> { 8, 12 });

            var rules = new Dictionary<Rule, FileProblems>();
            rules[csharpRule] = csharpFiles;
            rules[htmlRule] = htmlFiles;

            string report = _librarian.ReportOn(rules, null);

            Assert.That(report, Is.EqualTo(expectedReport.ToString()));
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

            SourceFile foo = new SourceFile("foo.cs");
            SourceFile goo = new SourceFile("goo.cs");
            SourceFile boo = new SourceFile("boo.cs");

            var csharpFiles = new FileProblems();
            csharpFiles[foo] = new FileMatch(true);
            csharpFiles[goo] = new FileMatch(false);
            csharpFiles[boo] = new LineMatch( new int[] {} );

            var rules = new Dictionary<Rule, FileProblems>();
            rules[csharpRule] = csharpFiles;

            string report = _librarian.ReportOn(rules, null);

            Assert.That(report, Is.EqualTo(expectedReport.ToString()));
        }


    }
}
