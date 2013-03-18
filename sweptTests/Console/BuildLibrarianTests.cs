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
            _args = new Arguments( new string[] { "library:foo.library", "history:foo.history" }, _storage );
            _librarian = new BuildLibrarian( _args, _storage );
        }


        [Test]
        public void we_can_read_run_history_from_disk()
        {
            _storage.RunHistory = XDocument.Parse(
@"<RunHistory>
  <Run Number=""22"" DateTime=""4/4/2012 10:25:02 AM"" Passed=""True"">
    <Rule ID=""foo"" Violations=""2"" Prior=""2"" FailOn=""None"" Breaking=""false"" />
  </Run>
</RunHistory>" );
            
            var runHistory = _librarian.ReadRunHistory();

            Assert.That( runHistory.Runs.Count, Is.EqualTo(1) );
        }


        [Test]
        public void Can_generate_run_entry_from_results()
        {
            var rule = new Rule()
            {
                ID = "basic entry",
                Description = "simple",
                FailOn = RuleFailOn.Over,
                RunFailOverLimit = 20
            };
            var sourceClauseMatch = new FileProblems();

            var failedSource = new SourceFile( "some_file.cs" );

            List<int> violationLines = new List<int>();
            for (int i = 0; i < 7; i++)
            {
                violationLines.Add( (i * 7) + 22 );  //arbitrary lines throughout the source file had this problem.
            }
            ClauseMatch failedClause = new LineMatch( violationLines );
            sourceClauseMatch[failedSource] = failedClause;

            var ruleViolations = new Dictionary<Rule, FileProblems>();
            ruleViolations[rule] = sourceClauseMatch;


            var noMatches = new FileProblems();
            var happyRule = new Rule
            {
                ID = "no problem",
                Description = "the app reports rules with no issues",
                FailOn = RuleFailOn.Any,
            };

            ruleViolations[happyRule] = noMatches;

            DateTime nowish = DateTime.Now;

            RunHistory runHistory = new RunHistory();
            RunHistoryEntry entryOld = new RunHistoryEntry { Number = 776, Passed = true };
            //ABEND entry.RuleResults.Add()
            runHistory.AddRun( entryOld );

            _librarian.ReportOn( ruleViolations, runHistory );
            RunHistoryEntry entry = _librarian.GenerateEntry( nowish );

            Assert.That( entry.Number, Is.EqualTo( 777 ) );
            Assert.That( entry.Date, Is.EqualTo( nowish ) );
            Assert.That( entry.RuleResults.Count, Is.EqualTo( 2 ) );
            Assert.That( entry.RuleResults[rule.ID].Violations, Is.EqualTo( 7 ) );
            Assert.That( entry.RuleResults[happyRule.ID].Violations, Is.EqualTo( 0 ) );
            Assert.That( entry.Passed, Is.True );

            Assert.Fail( "Need real values from history." );
        }

        //GenEntry with
        //no prior success
        //latest success contains rule
        //latest success does not contain rule
        //and test Prior and Breaking values for
        //Any, Over (when met and when exceeded)
        //Increase (when exceeded)

        // TODO: get rid of RunResult.FailOn, Breaking makes it redundant

        [Test]
        public void GenEntry_Increase_noPrior()
        {
            string id = "PE6-5000";
            RunHistoryEntry priorSuccess = null;
            Rule rut = new Rule { ID = id, FailOn = RuleFailOn.Increase };

            RuleResult result = _librarian.GetRuleResult( rut, 7, priorSuccess );

            Assert.That( result.ID, Is.EqualTo( id ) );
            Assert.That( result.FailOn, Is.EqualTo( RuleFailOn.Increase ) );
            Assert.That( result.Violations, Is.EqualTo( 7 ) );
            Assert.That( result.Prior, Is.EqualTo( 7 ) );
            Assert.That( result.Breaking, Is.False );
        }

        [Test]
        public void GenEntry_Increase_PriorWasBetter()
        {
            string id = "PE6-5000";
            RunHistoryEntry priorSuccess = new RunHistoryEntry();
            priorSuccess.RuleResults[id] = new RuleResult { ID = id, Violations = 2 };
            Rule rut = new Rule { ID = id, FailOn = RuleFailOn.Increase };

            RuleResult result = _librarian.GetRuleResult( rut, 7, priorSuccess );

            Assert.That( result.ID, Is.EqualTo( id ) );
            Assert.That( result.FailOn, Is.EqualTo( RuleFailOn.Increase ) );
            Assert.That( result.Violations, Is.EqualTo( 7 ) );
            Assert.That( result.Prior, Is.EqualTo( 2 ) );
            Assert.That( result.Breaking, Is.True );
        }

        [Test]
        public void GenEntry_Increase_PriorHasNoEntryForThisRule()
        {
            string id = "PE6-5000";
            RunHistoryEntry priorSuccess = new RunHistoryEntry();
            priorSuccess.RuleResults["by a different name"] = new RuleResult { ID = "by a different name", Violations = 2 };
            Rule rut = new Rule { ID = id, FailOn = RuleFailOn.Increase };

            RuleResult result = _librarian.GetRuleResult( rut, 7, priorSuccess );

            Assert.That( result.ID, Is.EqualTo( id ) );
            Assert.That( result.FailOn, Is.EqualTo( RuleFailOn.Increase ) );
            Assert.That( result.Violations, Is.EqualTo( 7 ) );
            Assert.That( result.Prior, Is.EqualTo( 7 ) );
            Assert.That( result.Breaking, Is.False );
        }


        [Test]
        public void GenEntry_Any_PriorHasNoEntryForThisRule()
        {
            string id = "PE6-5000";
            RunHistoryEntry priorSuccess = new RunHistoryEntry();
            priorSuccess.RuleResults["by a different name"] = new RuleResult { ID = "by a different name", Violations = 2 };
            Rule rut = new Rule { ID = id, FailOn = RuleFailOn.Any };

            RuleResult result = _librarian.GetRuleResult( rut, 7, priorSuccess );

            Assert.That( result.ID, Is.EqualTo( id ) );
            Assert.That( result.FailOn, Is.EqualTo( RuleFailOn.Any ) );
            Assert.That( result.Violations, Is.EqualTo( 7 ) );
            Assert.That( result.Prior, Is.EqualTo( 0 ) );  // an argument could be made for 7.
            Assert.That( result.Breaking, Is.True );
        }

        [Test]
        public void GenEntry_Over_PriorHasNoEntryForThisRule()
        {
            string id = "PE6-5000";
            RunHistoryEntry priorSuccess = new RunHistoryEntry();
            priorSuccess.RuleResults["by a different name"] = new RuleResult { ID = "by a different name", Violations = 2 };
            Rule rut = new Rule { ID = id, FailOn = RuleFailOn.Over, RunFailOverLimit = 5 };

            RuleResult result = _librarian.GetRuleResult( rut, 7, priorSuccess );

            Assert.That( result.ID, Is.EqualTo( id ) );
            Assert.That( result.FailOn, Is.EqualTo( RuleFailOn.Over ) );
            Assert.That( result.Violations, Is.EqualTo( 7 ) );
            Assert.That( result.Prior, Is.EqualTo( 5 ) );  // an argument could be made for 7.
            Assert.That( result.Breaking, Is.True );
        }

        [Test]
        public void Can_generate_failing_run_entry_from_results()
        {
            var rule = new Rule()
            {
                ID = "basic entry",
                Description = "simple",
                FailOn = RuleFailOn.Over,
                RunFailOverLimit = 2
            };
            var sourceClauseMatch = new FileProblems();

            var failedSource = new SourceFile("some_file.cs");

            List<int> violationLines = new List<int>();
            for (int i = 0; i < 7; i++)
            {
                violationLines.Add((i * 7) + 22);  //arbitrary lines throughout the source file had this problem.
            }
            ClauseMatch failedClause = new LineMatch(violationLines);
            sourceClauseMatch[failedSource] = failedClause;

            var ruleViolations = new Dictionary<Rule, FileProblems>();
            ruleViolations[rule] = sourceClauseMatch;

            DateTime nowish = DateTime.Now;

            RunHistory runHistory = new RunHistory();
            runHistory.AddRun(new RunHistoryEntry { Number = 887, Passed = true });

            _librarian.ReportOn( ruleViolations, runHistory );
            RunHistoryEntry entry = _librarian.GenerateEntry(nowish);

            Assert.That(entry.Number, Is.EqualTo(888));
            Assert.That(entry.Date, Is.EqualTo(nowish));
            Assert.That(entry.RuleResults.Count, Is.EqualTo(1));
            Assert.That(entry.RuleResults[rule.ID].Violations, Is.EqualTo(7));
            Assert.That(entry.Passed, Is.False);
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
            var results = new Dictionary<string, RuleResult>();
            results.Add( "foo", new RuleResult { ID = "foo", Violations = 2, Prior = 1, FailOn = RuleFailOn.Increase, Breaking = true } );
            runHistory.AddRun( new RunHistoryEntry
            {
                Number = 22,
                Date = DateTime.Parse( "4/4/2012 10:25:02 AM" ),
                RuleResults = results,
                Passed = false
            } );

            var resultsNext = new Dictionary<string, RuleResult>();
            resultsNext.Add( "bar", new RuleResult { ID = "bar", Violations = 0, Prior = 2, FailOn = RuleFailOn.None, Breaking = false } );
            runHistory.AddRun( new RunHistoryEntry
            {
                Number = 23,
                Date = DateTime.Parse( "4/7/2012 10:25:03 AM" ),
                RuleResults = resultsNext,
                Passed = true
            } );

            _librarian.ReportOn( new Dictionary<Rule, FileProblems>(), runHistory);
            _librarian.WriteRunHistory();


            var expectedHistory =
@"<RunHistory>
  <Run Number=""22"" DateTime=""4/4/2012 10:25:02 AM"" Passed=""False"">
    <Rule ID=""foo"" Violations=""2"" Prior=""1"" FailOn=""Increase"" Breaking=""true"" />
  </Run>
  <Run Number=""23"" DateTime=""4/7/2012 10:25:03 AM"" Passed=""True"">
    <Rule ID=""bar"" Violations=""0"" Prior=""2"" FailOn=""None"" Breaking=""false"" />
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
