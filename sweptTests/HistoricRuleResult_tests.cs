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

        //GenEntry with
        //no prior success
        //latest success contains rule
        //latest success does not contain rule
        //and test Prior and Breaking values for
        //Any, Over (when met and when exceeded)
        //Increase (when exceeded)

        [Test]
        public void GenEntry_Increase_noPrior()
        {
            string id = "PE6-5000";
            RunHistoryEntry priorSuccess = null;
            Rule rut = new Rule { ID = id, FailOn = RuleFailOn.Increase };

            HistoricRuleResult result = _inspector.GetRuleResult( rut, 7, priorSuccess );

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
            priorSuccess.RuleResults[id] = new HistoricRuleResult { ID = id, Violations = 2 };
            Rule rut = new Rule { ID = id, FailOn = RuleFailOn.Increase };

            var inspector = new RunInspector( null );
            HistoricRuleResult result = inspector.GetRuleResult( rut, 7, priorSuccess );

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
            priorSuccess.RuleResults["by a different name"] = new HistoricRuleResult { ID = "by a different name", Violations = 2 };
            Rule rut = new Rule { ID = id, FailOn = RuleFailOn.Increase };

            HistoricRuleResult result = _inspector.GetRuleResult( rut, 7, priorSuccess );

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
            priorSuccess.RuleResults["by a different name"] = new HistoricRuleResult { ID = "by a different name", Violations = 2 };
            Rule rut = new Rule { ID = id, FailOn = RuleFailOn.Any };

            HistoricRuleResult result = _inspector.GetRuleResult( rut, 7, priorSuccess );

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
            priorSuccess.RuleResults["by a different name"] = new HistoricRuleResult { ID = "by a different name", Violations = 2 };
            Rule rut = new Rule { ID = id, FailOn = RuleFailOn.Over, RunFailOverLimit = 5 };

            HistoricRuleResult result = _inspector.GetRuleResult( rut, 7, priorSuccess );

            Assert.That( result.ID, Is.EqualTo( id ) );
            Assert.That( result.FailOn, Is.EqualTo( RuleFailOn.Over ) );
            Assert.That( result.Violations, Is.EqualTo( 7 ) );
            Assert.That( result.Prior, Is.EqualTo( 5 ) );  // an argument could be made for 7.
            Assert.That( result.Breaking, Is.True );
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
            runHistory.AddEntry( new RunHistoryEntry { Number = 776, Passed = true } );

            var inspector = new RunInspector( runHistory );
            DateTime nowish = DateTime.Now;
            RunHistoryEntry entry = inspector.GenerateEntry( nowish, ruleTasks );

            Assert.That( entry.Number, Is.EqualTo( 777 ) );
            Assert.That( entry.Date, Is.EqualTo( nowish ) );
            Assert.That( entry.RuleResults.Count, Is.EqualTo( 2 ) );
            Assert.That( entry.RuleResults[rule.ID].Violations, Is.EqualTo( 7 ) );
            Assert.That( entry.RuleResults[happyRule.ID].Violations, Is.EqualTo( 0 ) );
            Assert.That( entry.Passed, Is.True );
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
            runHistory.AddEntry( new RunHistoryEntry { Number = 887, Passed = true } );

            var inspector = new RunInspector( runHistory );
            RunHistoryEntry entry = inspector.GenerateEntry( nowish, ruleTasks );

            Assert.That( entry.Number, Is.EqualTo( 888 ) );
            Assert.That( entry.Date, Is.EqualTo( nowish ) );
            Assert.That( entry.RuleResults.Count, Is.EqualTo( 1 ) );
            Assert.That( entry.RuleResults[rule.ID].Violations, Is.EqualTo( 7 ) );
            Assert.That( entry.Passed, Is.False );
        }
    }
}
