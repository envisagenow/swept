using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using System.Xml.Linq;

namespace swept.Tests
{
    [TestFixture]
    public class RunDelta_tests
    {
        private RunHistory _runHistory;
        private RunInspector _inspector;

        [SetUp]
        public void Setup()
        {
            _runHistory = new RunHistory();
            _inspector = new RunInspector( _runHistory );
        }

        [Test]
        public void Empty_RunHistory_causes_empty_delta()
        {
            XElement delta = _inspector.GenerateDeltaXml( new RunHistoryEntry() );

            Assert.That( delta.Descendants().Count(), Is.EqualTo( 0 ) );
        }

        [Test]
        public void Run_with_no_prior_passing_run_shows_empty_delta()
        {
            RunHistoryEntry priorButFailingEntry = new RunHistoryEntry { Passed = false };

            //  This rule result causes this run to fail.  Deltas only apply between the
            //  RunHistory.PriorPassingRun and the current run in hand.
            priorButFailingEntry.AddResult( "917", true, RuleFailOn.Any, 0, 3 );

            //  So even though this rule passes, it will not lead to a delta showing with
            //  next run's improvement on this rule.
            priorButFailingEntry.AddResult( "644", true, RuleFailOn.Increase, 40, 40 );

            _runHistory.AddEntry( priorButFailingEntry );
            Assert.That( _runHistory.LatestPassingRun, Is.Null );

            RunHistoryEntry entry = new RunHistoryEntry { Passed = true };
            entry.AddResult( "644", true, RuleFailOn.Increase, 10, 10 );

            XElement delta = _inspector.GenerateDeltaXml( entry );

            Assert.That( delta.Descendants().Count(), Is.EqualTo( 0 ) );
        }

        [Test]
        public void Delta_shows_Failures()
        {
            RunHistoryEntry entry = new RunHistoryEntry { Passed = false };
            entry.AddResult( "644", true, RuleFailOn.Any, 0, 2 );
            entry.AddResult( "432", true, RuleFailOn.Increase, 10, 23 );

            XElement delta = _inspector.GenerateDeltaXml( entry );

            Assert.That( delta.Descendants().Count(), Is.EqualTo( 2 ) );
            var failElement = delta.Descendants().Single( x => x.Attribute( "ID" ).Value == "644" );
            Assert_IsXElementMatching( failElement, "<DeltaItem ID='644' Limit='0' Current='2' Outcome='Fail' />" );

            failElement = delta.Descendants().Single( x => x.Attribute( "ID" ).Value == "432" );
            Assert_IsXElementMatching( failElement, "<DeltaItem ID='432' Limit='10' Current='23' Outcome='Fail' />" );
        }

        [Test]
        public void Delta_shows_Missing_rules()
        {
            RunHistoryEntry priorPassingEntry = new RunHistoryEntry { Passed = true };
            priorPassingEntry.AddResult( "644", true, RuleFailOn.Increase, 10, 2 );
            priorPassingEntry.AddResult( "411", true, RuleFailOn.Increase, 20, 7 );
            _runHistory.AddEntry( priorPassingEntry );

            XElement delta = _inspector.GenerateDeltaXml( new RunHistoryEntry { Passed = true } );

            Assert.That( delta.Descendants().Count(), Is.EqualTo( 2 ) );
            var goneElement = delta.Descendants().Single( x => x.Attribute( "ID" ).Value == "644" );
            Assert_IsXElementMatching( goneElement, "<DeltaItem ID='644' Limit='2' Current='0' Outcome='Gone' />" );

            goneElement = delta.Descendants().Single( x => x.Attribute( "ID" ).Value == "411" );
            Assert_IsXElementMatching( goneElement, "<DeltaItem ID='411' Limit='7' Current='0' Outcome='Gone' />" );
        }

        [Test]
        public void Delta_shows_Fixes()
        {
            RunHistoryEntry priorPassingEntry = new RunHistoryEntry { Passed = true };
            priorPassingEntry.AddResult( "644", true, RuleFailOn.Increase, 10, 2 );
            priorPassingEntry.AddResult( "411", true, RuleFailOn.Increase, 20, 7 );
            _runHistory.AddEntry( priorPassingEntry );

            RunHistoryEntry newRun = new RunHistoryEntry { Passed = true };
            newRun.AddResult( "644", true, RuleFailOn.Increase, 2, 1 );
            newRun.AddResult( "411", true, RuleFailOn.Increase, 7, 4 );

            XElement delta = _inspector.GenerateDeltaXml( newRun );

            Assert.That( delta.Descendants().Count(), Is.EqualTo( 2 ) );
            var fixElement = delta.Descendants().Single( x => x.Attribute( "ID" ).Value == "644" );
            Assert_IsXElementMatching( fixElement, "<DeltaItem ID='644' Limit='2' Current='1' Outcome='Fix' />" );

            fixElement = delta.Descendants().Single( x => x.Attribute( "ID" ).Value == "411" );
            Assert_IsXElementMatching( fixElement, "<DeltaItem ID='411' Limit='7' Current='4' Outcome='Fix' />" );
        }

        [Test]
        public void Broken_rules_will_suppress_fixed_or_gone_rule_reporting()
        {
            RunHistoryEntry priorPassingEntry = new RunHistoryEntry { Passed = true };
            priorPassingEntry.AddResult( "800", true, RuleFailOn.Increase, 8, 5 );
            priorPassingEntry.AddResult( "644", true, RuleFailOn.Increase, 10, 2 );
            priorPassingEntry.AddResult( "411", true, RuleFailOn.Increase, 20, 7 );
            _runHistory.AddEntry( priorPassingEntry );

            RunHistoryEntry newRun = new RunHistoryEntry { Passed = false };
            newRun.AddResult( "800", true, RuleFailOn.Increase, 5, 15 );
            //  A rule 644 result is missing from this run...
            newRun.AddResult( "411", true, RuleFailOn.Increase, 7, 4 );

            XElement delta = _inspector.GenerateDeltaXml( newRun );

            Assert.That( delta.Descendants().Count(), Is.EqualTo( 1 ) );
            Assert.That( delta.Descendants().Single().Attribute( "ID" ).Value, Is.EqualTo( "800" ) );
        }

        private void Assert_IsXElementMatching( XElement actualElement, string expectedElementText )
        {
            string actualText = actualElement.ToString( SaveOptions.DisableFormatting );

            var expectedElement = XElement.Parse( expectedElementText );
            var normalizedExpectedText = expectedElement.ToString( SaveOptions.DisableFormatting );

            Assert.That( actualText, Is.EqualTo( normalizedExpectedText ) );
        }
    }
}
