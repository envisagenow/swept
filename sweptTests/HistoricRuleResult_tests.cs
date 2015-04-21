//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace swept.Tests
{
    [TestFixture]
    public class HistoricRuleResult_tests
    {
        private RunInspector _inspector;
        private RunHistory _history;

        [SetUp]
        public void SetUp()
        {
            _history = new RunHistory();
            _inspector = new RunInspector( _history );
        }


        [Test]
        public void Inspector_finds_no_delta_between_empty_sources()
        {
            DetailDelta result = _inspector.GetDetailDelta(new RuleTasks(), new RunChanges());


            Assert.That(result, Is.InstanceOf<DetailDelta>());
            Assert.That(result.Files.Count, Is.EqualTo(0));
        }

        [Test]
        public void Inspector_finds_all_RunDetails_for_delta_with_empty_RuleTasks()
        {
            RunChanges runDetails = new RunChanges();
            runDetails.RunNumber = 4;
            runDetails.DateTime = DateTime.Now;

            FileChange detailFile = new FileChange();
            detailFile.Name = "somefile.cs";

            RuleChange detailRule = new RuleChange();
            detailRule.ID = "INT-011";
            detailRule.Was = 3;
            detailRule.Is = 6;
            detailRule.Breaking = true;

            detailFile.Rules.Add(detailRule);
            runDetails.Files.Add(detailFile);

            DetailDelta result = _inspector.GetDetailDelta(new RuleTasks(), runDetails);


            Assert.That(result, Is.InstanceOf<DetailDelta>());
            Assert.That(result.Files.Count, Is.EqualTo(1));

            var firstFile = result.Files[0];

            Assert.That( firstFile.Name, Is.EqualTo("somefile.cs"));
            Assert.That(firstFile.Rules.Count, Is.EqualTo(1));
        }


        //[Test]
        //public void finding_deltas_later()
        //{
        //    Assert.Fail("I'll be back.");

        //    var csharpRule = new Rule {
        //        ID = "DomainEvents 01",
        //        Description = "Use DomainEvents instead of AcadisUserPersister and AuditRecordPersister"
        //    };

        //    SourceFile abc = new SourceFile("abc.cs");
        //    SourceFile foo = new SourceFile("foo.cs");
        //    SourceFile goo = new SourceFile("goo.cs");
        //    SourceFile google = new SourceFile("google_eyes.cs");

        //    var csharpFiles = new FileTasks();
        //    csharpFiles[foo] = new FileMatch(true);
        //    csharpFiles[abc] = new FileMatch(true);
        //    csharpFiles[goo] = new LineMatch(new List<int> { 1, 2, 3 });
        //    csharpFiles[google] = new LineMatch(new List<int> { 7, 77, 777 });

        //    var rules = new RuleTasks();
        //    rules[csharpRule] = csharpFiles;

        //}


        [Test]
        public void GenEntry_Increase_noPrior()
        {
            string id = "PE6-5000";
            RunEntry priorSuccess = null;
            Rule rut = new Rule { ID = id, FailOn = RuleFailOn.Increase, Description = "Really very important." };

            RuleResult result = _inspector.GetRuleResult( rut, 7, priorSuccess );

            Assert.That( result.ID, Is.EqualTo( id ) );
            Assert.That( result.FailOn, Is.EqualTo( RuleFailOn.Increase ) );
            Assert.That( result.TaskCount, Is.EqualTo( 7 ) );
            Assert.That( result.Threshold, Is.EqualTo( 7 ) );
            Assert.That( result.Breaking, Is.False );
            Assert.That( result.Description, Is.EqualTo( "Really very important." ) );
        }

        [Test]
        public void RuleResult_fails_when_RuleFailOn_Increase_and_PriorWasBetter()
        {
            string id = "PE6-5000";
            RunEntry priorSuccess = new RunEntry();

            priorSuccess.AddResult( id, false, RuleFailOn.Increase, 2, 2, "Update JQuery framework" );
            Rule rut = new Rule { ID = id, FailOn = RuleFailOn.Increase };

            var inspector = new RunInspector( null );
            RuleResult result = inspector.GetRuleResult( rut, 7, priorSuccess );

            Assert.That( result.ID, Is.EqualTo( id ) );
            Assert.That( result.FailOn, Is.EqualTo( RuleFailOn.Increase ) );
            Assert.That( result.TaskCount, Is.EqualTo( 7 ) );
            Assert.That( result.Threshold, Is.EqualTo( 2 ) );
            Assert.That( result.Breaking, Is.True );
        }

        [Test]
        public void RuleResult_passes_when_RuleFailOn_Increase_and_Prior_has_no_result_for_this_rule()
        {
            string id = "PE6-5000";
            RunEntry priorSuccess = new RunEntry();
            priorSuccess.AddResult( "PE7-1000", false, RuleFailOn.Increase, 2, 2, "Was PE6-5000" );
            Rule rut = new Rule { ID = id, FailOn = RuleFailOn.Increase };

            RuleResult result = _inspector.GetRuleResult( rut, 7, priorSuccess );

            Assert.That( result.ID, Is.EqualTo( id ) );
            Assert.That( result.FailOn, Is.EqualTo( RuleFailOn.Increase ) );
            Assert.That( result.TaskCount, Is.EqualTo( 7 ) );
            Assert.That( result.Threshold, Is.EqualTo( 7 ) );
            Assert.That( result.Breaking, Is.False );
        }

        [Test]
        public void RuleResult_fails_when_RuleFailOn_Any_and_Prior_has_no_result_for_this_rule()
        {
            string id = "PE6-5000";
            RunEntry priorSuccess = new RunEntry();
            priorSuccess.AddResult( "PE7-1000", false, RuleFailOn.Increase, 2, 2, "Was PE6-5000" );
            Rule rut = new Rule { ID = id, FailOn = RuleFailOn.Any };

            RuleResult result = _inspector.GetRuleResult( rut, 7, priorSuccess );

            Assert.That( result.ID, Is.EqualTo( id ) );
            Assert.That( result.FailOn, Is.EqualTo( RuleFailOn.Any ) );
            Assert.That( result.TaskCount, Is.EqualTo( 7 ) );
            Assert.That( result.Threshold, Is.EqualTo( 0 ) );  // an argument could be made for 7.
            Assert.That( result.Breaking, Is.True );
        }

        [Test]
        public void Can_generate_run_entry_from_results()
        {
            var rule = new Rule()
            {
                ID = "basic entry",
                Description = "simple",
                FailOn = RuleFailOn.Increase
            };
            var sourceClauseMatch = new FileTasks();

            var failedSource = new SourceFile( "some_file.cs" );

            List<int> violationLines = new List<int>();
            for (int i = 0; i < 7; i++)
            {
                violationLines.Add( (i * 7) + 22 );  //arbitrary lines throughout the source file had this problem.
            }
            ClauseMatch failedClause = new LineMatch( violationLines );
            sourceClauseMatch[failedSource] = failedClause;

            var ruleTasks = new RuleTasks();
            ruleTasks[rule] = sourceClauseMatch;

            var noMatches = new FileTasks();
            var happyRule = new Rule
            {
                ID = "no problem",
                Description = "the app reports rules with no issues",
                FailOn = RuleFailOn.Any,
            };

            ruleTasks[happyRule] = noMatches;

            RunHistory runHistory = new RunHistory();
            runHistory.AddEntry( new RunEntry { Number = 776, Passed = true } );

            var inspector = new RunInspector( runHistory );
            DateTime nowish = DateTime.Now;
            RunEntry entry = inspector.GenerateEntry( nowish, ruleTasks );

            Assert.That( entry.Number, Is.EqualTo( 777 ) );
            Assert.That( entry.Date, Is.EqualTo( nowish ) );
            Assert.That( entry.RuleResults.Count, Is.EqualTo( 2 ) );
            Assert.That( entry.RuleResults[rule.ID].TaskCount, Is.EqualTo( 7 ) );
            Assert.That( entry.RuleResults[happyRule.ID].TaskCount, Is.EqualTo( 0 ) );
            Assert.That( entry.Passed, Is.True );
        }

        [Test]
        public void Can_generate_failing_run_entry_from_results()
        {
            var rule = new Rule()
            {
                ID = "basic entry",
                Description = "simple",
                FailOn = RuleFailOn.Any
            };
            var sourceClauseMatch = new FileTasks();

            var failedSource = new SourceFile( "some_file.cs" );

            List<int> violationLines = new List<int>();
            for (int i = 0; i < 7; i++)
            {
                violationLines.Add( (i * 7) + 22 );  //arbitrary lines throughout the source file had this problem.
            }
            ClauseMatch failedClause = new LineMatch( violationLines );
            sourceClauseMatch[failedSource] = failedClause;

            var ruleTasks = new RuleTasks();
            ruleTasks[rule] = sourceClauseMatch;

            DateTime nowish = DateTime.Now;

            RunHistory runHistory = new RunHistory();
            runHistory.AddEntry( new RunEntry { Number = 887, Passed = true } );

            var inspector = new RunInspector( runHistory );
            RunEntry entry = inspector.GenerateEntry( nowish, ruleTasks );

            Assert.That( entry.Number, Is.EqualTo( 888 ) );
            Assert.That( entry.Date, Is.EqualTo( nowish ) );
            Assert.That( entry.RuleResults.Count, Is.EqualTo( 1 ) );
            Assert.That( entry.RuleResults[rule.ID].TaskCount, Is.EqualTo( 7 ) );
            Assert.That( entry.Passed, Is.False );
        }
    }
}
