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
        public void GenEntry_Increase_noPrior()
        {
            string id = "PE6-5000";
            RunHistoryEntry priorSuccess = null;
            Rule rut = new Rule { ID = id, FailOn = RuleFailOn.Increase };

            HistoricRuleResult result = _inspector.GetRuleResult( rut, 7, priorSuccess );

            Assert.That( result.ID, Is.EqualTo( id ) );
            Assert.That( result.FailOn, Is.EqualTo( RuleFailOn.Increase ) );
            Assert.That( result.TaskCount, Is.EqualTo( 7 ) );
            Assert.That( result.Threshold, Is.EqualTo( 7 ) );
            Assert.That( result.Breaking, Is.False );
        }

        [Test]
        public void RuleResult_fails_when_RuleFailOn_Increase_and_PriorWasBetter()
        {
            string id = "PE6-5000";
            RunHistoryEntry priorSuccess = new RunHistoryEntry();

            priorSuccess.AddResult( id, false, RuleFailOn.Increase, 2, 2 );
            Rule rut = new Rule { ID = id, FailOn = RuleFailOn.Increase };

            var inspector = new RunInspector( null );
            HistoricRuleResult result = inspector.GetRuleResult( rut, 7, priorSuccess );

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
            RunHistoryEntry priorSuccess = new RunHistoryEntry();
            priorSuccess.AddResult( "by a different name", false, RuleFailOn.Increase, 2, 2 );
            Rule rut = new Rule { ID = id, FailOn = RuleFailOn.Increase };

            HistoricRuleResult result = _inspector.GetRuleResult( rut, 7, priorSuccess );

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
            RunHistoryEntry priorSuccess = new RunHistoryEntry();
            priorSuccess.AddResult( "by a different name", false, RuleFailOn.Any, 0, 0 );
            Rule rut = new Rule { ID = id, FailOn = RuleFailOn.Any };

            HistoricRuleResult result = _inspector.GetRuleResult( rut, 7, priorSuccess );

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
            runHistory.AddEntry( new RunHistoryEntry { Number = 776, Passed = true } );

            var inspector = new RunInspector( runHistory );
            DateTime nowish = DateTime.Now;
            RunHistoryEntry entry = inspector.GenerateEntry( nowish, ruleTasks );

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
            runHistory.AddEntry( new RunHistoryEntry { Number = 887, Passed = true } );

            var inspector = new RunInspector( runHistory );
            RunHistoryEntry entry = inspector.GenerateEntry( nowish, ruleTasks );

            Assert.That( entry.Number, Is.EqualTo( 888 ) );
            Assert.That( entry.Date, Is.EqualTo( nowish ) );
            Assert.That( entry.RuleResults.Count, Is.EqualTo( 1 ) );
            Assert.That( entry.RuleResults[rule.ID].TaskCount, Is.EqualTo( 7 ) );
            Assert.That( entry.Passed, Is.False );
        }
    }
}
