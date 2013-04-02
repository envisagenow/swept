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
        public void Failures_show_in_delta()
        {
            RunHistoryEntry entry = new RunHistoryEntry { Passed = false };
            entry.RuleResults["644"] = new HistoricRuleResult
            {
                ID = "644",
                Breaking = true,
                FailOn = RuleFailOn.Any,
                Threshold = 0,
                TaskCount = 2
            };
            entry.RuleResults["432"] = new HistoricRuleResult
            {
                ID = "432",
                Breaking = true,
                FailOn = RuleFailOn.Increase,
                Threshold = 10,
                TaskCount = 23
            };


            XElement delta = _inspector.GenerateDeltaXml( entry );

            Assert.That( delta.Descendants().Count(), Is.EqualTo( 2 ) );
            var failElement = delta.Descendants().Single( x => x.Attribute( "ID" ).Value == "644" );
            Assert.That( failElement.Attribute( "Limit" ).Value, Is.EqualTo( "0" ) );
            Assert.That( failElement.Attribute( "Current" ).Value, Is.EqualTo( "2" ) );
            Assert.That( failElement.Attribute( "Outcome" ).Value, Is.EqualTo( "Fail" ) );

            failElement = delta.Descendants().Single( x => x.Attribute( "ID" ).Value == "432" );
            Assert.That( failElement.Attribute( "Limit" ).Value, Is.EqualTo( "10" ) );
            Assert.That( failElement.Attribute( "Current" ).Value, Is.EqualTo( "23" ) );
            Assert.That( failElement.Attribute( "Outcome" ).Value, Is.EqualTo( "Fail" ) );

        }

        [Test]
        public void Missing_rule_shows_in_delta()
        {
            RunHistoryEntry priorPassingEntry = new RunHistoryEntry { Passed = true };
            priorPassingEntry.RuleResults["644"] = new HistoricRuleResult
            {
                ID = "644",
                Breaking = true,
                FailOn = RuleFailOn.Increase,
                Threshold = 10,
                TaskCount = 2
            };
            _runHistory.AddEntry( priorPassingEntry );
            XElement delta = _inspector.GenerateDeltaXml( new RunHistoryEntry() );

            Assert.That( delta.Descendants().Count(), Is.EqualTo( 1 ) );
            var failElement = delta.Descendants().Single( x => x.Attribute( "ID" ).Value == "644" );
            // this is overkill after that selector, right?  Assert.That( failElement.Attribute( "ID" ).Value, Is.EqualTo( "644" ) );
            Assert.That( failElement.Attribute( "Limit" ).Value, Is.EqualTo( "2" ) );
            Assert.That( failElement.Attribute( "Current" ).Value, Is.EqualTo( "0" ) );
            Assert.That( failElement.Attribute( "Outcome" ).Value, Is.EqualTo( "Fix" ) );
        }

        [Test]
        public void Passing_run_with_no_prior_passing_run_shows_empty_delta()
        {
            RunHistoryEntry entry = new RunHistoryEntry { Passed = true };
            entry.RuleResults["644"] = new HistoricRuleResult
            {
                ID = "644",
                Breaking = true,
                FailOn = RuleFailOn.Increase,
                Threshold = 10,
                TaskCount = 10
            };
            XElement delta = _inspector.GenerateDeltaXml( entry );

            Assert.That( delta.Descendants().Count(), Is.EqualTo( 0 ) );
        }
    }
}
