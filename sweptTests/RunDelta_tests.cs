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
            XElement delta = _inspector.GenerateDeltaXml( new RunEntry() );

            Assert.That( delta.Descendants().Count(), Is.EqualTo( 0 ) );
        }

        [Test]
        public void Run_with_no_prior_passing_run_shows_empty_delta()
        {
            RunEntry priorButFailingEntry = new RunEntry { Passed = false };

            //  This rule result causes this run to fail.  Deltas only apply between the
            //  RunHistory.PriorPassingRun and the current run in hand.
            priorButFailingEntry.AddResult( "917", true, RuleFailOn.Any, 0, 3, "No document.all() allowed" );

            //  So even though this rule passes, it will not lead to a delta showing with
            //  next run's improvement on this rule.
            priorButFailingEntry.AddResult( "644", true, RuleFailOn.Increase, 40, 40, "Replace WR framework with DF" );

            _runHistory.AddEntry( priorButFailingEntry );
            Assert.That( _runHistory.LatestPassingRun, Is.Null );

            RunEntry entry = new RunEntry { Passed = true };
            entry.AddResult( "644", true, RuleFailOn.Increase, 10, 10, "Replace old stylesheets" );

            XElement delta = _inspector.GenerateDeltaXml( entry );

            Assert.That( delta.Descendants().Count(), Is.EqualTo( 0 ) );
        }

        [Test]
        public void Delta_shows_Failures()
        {
            RunEntry entry = new RunEntry { Passed = false };
            entry.AddResult( "644", true, RuleFailOn.Any, 0, 2, "Absolutely no document.all." );
            entry.AddResult( "432", true, RuleFailOn.Increase, 10, 23, "Eliminate references to behavior files" );

            XElement delta = _inspector.GenerateDeltaXml( entry );

            Assert.That( delta.Descendants().Count(), Is.EqualTo( 2 ) );
            var failElement = delta.Descendants().Single( x => x.Attribute( "ID" ).Value == "644" );
            Assert_IsXElementMatching( failElement, "<DeltaItem ID='644' Threshold='0' TaskCount='2' Outcome='Fail' Description='Absolutely no document.all.' />" );

            failElement = delta.Descendants().Single( x => x.Attribute( "ID" ).Value == "432" );
            Assert_IsXElementMatching( failElement, "<DeltaItem ID='432' Threshold='10' TaskCount='23' Outcome='Fail' Description='Eliminate references to behavior files' />" );
        }

        [Test]
        public void Delta_shows_Missing_rules()
        {
            RunEntry priorPassingEntry = new RunEntry { Passed = true };
            priorPassingEntry.AddResult( "644", true, RuleFailOn.Increase, 10, 2, "Replace AjaxToolkit with JQuery" );
            priorPassingEntry.AddResult( "411", true, RuleFailOn.Increase, 20, 7, "Less of foo, please." );
            _runHistory.AddEntry( priorPassingEntry );

            XElement delta = _inspector.GenerateDeltaXml( new RunEntry { Passed = true } );

            Assert.That( delta.Descendants().Count(), Is.EqualTo( 2 ) );
            var goneElement = delta.Descendants().Single( x => x.Attribute( "ID" ).Value == "644" );
            Assert_IsXElementMatching( goneElement, "<DeltaItem ID='644' Threshold='10' TaskCount='0' Outcome='Gone' Description='Replace AjaxToolkit with JQuery' />" );

            goneElement = delta.Descendants().Single( x => x.Attribute( "ID" ).Value == "411" );
            Assert_IsXElementMatching( goneElement, "<DeltaItem ID='411' Threshold='20' TaskCount='0' Outcome='Gone' Description='Less of foo, please.'/>" );
        }

        [Test]
        public void Delta_shows_Fixes()
        {
            RunEntry priorPassingEntry = new RunEntry { Passed = true };
            priorPassingEntry.AddResult( "644", true, RuleFailOn.Increase, 10, 2, "Descrip" );
            priorPassingEntry.AddResult( "411", true, RuleFailOn.Increase, 20, 7, "Less foo now!" );
            _runHistory.AddEntry( priorPassingEntry );

            RunEntry newRun = new RunEntry { Passed = true };
            newRun.AddResult( "644", true, RuleFailOn.Increase, 2, 1, "Descrip" );
            newRun.AddResult( "411", true, RuleFailOn.Increase, 7, 4, "Less foo now!" );

            XElement delta = _inspector.GenerateDeltaXml( newRun );

            Assert.That( delta.Descendants().Count(), Is.EqualTo( 2 ) );
            var fixElement = delta.Descendants().Single( x => x.Attribute( "ID" ).Value == "644" );
            Assert_IsXElementMatching( fixElement, "<DeltaItem ID='644' Threshold='2' TaskCount='1' Outcome='Fix' Description='Descrip' />" );

            fixElement = delta.Descendants().Single( x => x.Attribute( "ID" ).Value == "411" );
            Assert_IsXElementMatching( fixElement, "<DeltaItem ID='411' Threshold='7' TaskCount='4' Outcome='Fix' Description='Less foo now!' />" );
        }

        [Test, Ignore("Until it's got some didactic value")]
        public void Delta_shows_descriptions_from_new_run()
        {
            Assert.Fail();
        }


        [Test]
        public void Broken_rules_will_suppress_fixed_or_gone_rule_reporting()
        {
            RunEntry priorPassingEntry = new RunEntry { Passed = true };
            priorPassingEntry.AddResult( "800", true, RuleFailOn.Increase, 8, 5, "foo" );
            priorPassingEntry.AddResult( "644", true, RuleFailOn.Increase, 10, 2, "bar" );
            priorPassingEntry.AddResult( "411", true, RuleFailOn.Increase, 20, 7, "baz" );
            _runHistory.AddEntry( priorPassingEntry );

            RunEntry newRun = new RunEntry { Passed = false };
            newRun.AddResult( "800", true, RuleFailOn.Increase, 5, 15, "foo" );
            //  A rule 644 result is missing from this run...
            newRun.AddResult( "411", true, RuleFailOn.Increase, 7, 4, "baz" );

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
