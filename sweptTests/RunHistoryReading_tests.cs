//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using System.Xml.Linq;

namespace swept.Tests
{
    [TestFixture]
    public class RunHistoryReading_tests
    {
        private BuildLibrarian _librarian;

        [SetUp]
        public void Setup()
        {
            var storage = new MockStorageAdapter();
            var args = new Arguments( new string[] { "library:foo.library", "history:foo.history" }, storage );
            _librarian = new BuildLibrarian( args, storage );
        }

        [Test]
        public void Empty_run_history_causes_no_problem()
        {
            var historyXml = XDocument.Parse( "<RunHistory />" );

            RunHistory history = _librarian.ReadRunHistory( historyXml );

            Assert.That( history.Runs.Count(), Is.EqualTo( 0 ) );
        }

        [TestCase( 12, "9/14/2012 2:44:02 AM", 60 )]
        [TestCase( 14, "5/11/2012 7:28:02 AM", 54 )]
        public void We_can_read_history_from_XML_into_domain( int runNumber, string dateString, int taskCount )
        {
            //  Don't overanalyze this history expecting it to make domain sense
            var historyXml = XDocument.Parse( string.Format(
@"<RunHistory>
  <Run Number=""{3}"" DateTime=""{2}"" Passed=""false"">
    <Rule ID=""{0}"" TaskCount=""{1}"" FailOn=""Increase"" Threshold=""38"" Breaking=""true"" Description=""Optimism!"" />
    <Rule ID=""always the same"" TaskCount=""44"" FailOn=""None"" Threshold=""44"" Breaking=""false"" Description=""Same as before"" />
  </Run>
  <Run Number=""1100"" DateTime=""1/1/2022 3:20:14 PM"" Passed=""true"">
    <Rule ID=""always the same"" TaskCount=""44"" FailOn=""None"" Threshold=""44"" Breaking=""false"" Description=""Same as before"" />
  </Run>

</RunHistory>", "silly problem", taskCount, dateString, runNumber ) );

            RunHistory history = _librarian.ReadRunHistory( historyXml );

            RunHistoryEntry firstRun = history.Runs.ElementAt( 0 );
            Assert.That( firstRun.Date, Is.EqualTo( DateTime.Parse( dateString ) ) );
            Assert.That( firstRun.Number, Is.EqualTo( runNumber ) );

            HistoricRuleResult sillyResult = firstRun.RuleResults["silly problem"];
            Assert.That( sillyResult.TaskCount, Is.EqualTo( taskCount ) );
            Assert.That( sillyResult.ID, Is.EqualTo( "silly problem" ) );
            Assert.That( sillyResult.Threshold, Is.EqualTo( 38 ) );
            Assert.That( sillyResult.FailOn, Is.EqualTo( RuleFailOn.Increase ) );
            Assert.That( sillyResult.Breaking );
            Assert.That( sillyResult.Description, Is.EqualTo( "Optimism!" ) );

            HistoricRuleResult sameResult = firstRun.RuleResults["always the same"];
            Assert.That( sameResult.TaskCount, Is.EqualTo( 44 ) );
            Assert.That( firstRun.Passed, Is.False );
            Assert.That( sameResult.Breaking, Is.False );
            Assert.That( sameResult.Description, Is.EqualTo( "Same as before" ) );

            RunHistoryEntry secondRun = history.Runs.ElementAt( 1 );
            Assert.That( secondRun.Date, Is.EqualTo( DateTime.Parse( "1/1/2022 3:20:14 PM" ) ) );
            Assert.That( secondRun.Number, Is.EqualTo( 1100 ) );
            Assert.That( secondRun.RuleResults.Count(), Is.EqualTo( 1 ) );
            Assert.That( secondRun.Passed );
        }

        [Test]
        public void When_no_Flags_in_Run_XML_then_Flags_collection_empty()
        {
            RunHistory history = _librarian.ReadRunHistory( XDocument.Parse( @"<RunHistory><Run Number='1' DateTime='10/4/2013 10:52 AM' Passed='False' /></RunHistory>" ) );

            var run = history.Runs.ElementAt( 0 );

            var flags = run.Flags;
            Assert.That( flags.Count(), Is.EqualTo( 0 ) );
        }

        [Test]
        public void Flags_in_History_XML_parsed_into_Flags()
        {
            // TODO: extract flags to another section, later,
            // leaving only references living in the Run elements.
            string historyXML = @"<RunHistory>

    <Run Number='1' DateTime='10/4/2013 10:52 AM' Passed='False'>
        <Rule ID='MID-011' TaskCount='0' Threshold='0' FailOn='Increase' Breaking='false' Description='Align widgets to the left' />
        <Rule ID='MID-033' TaskCount='14' Threshold='10' FailOn='Increase' Breaking='true' Description='Gadgets should be properly wired' />

        <Flag RuleID='MID-033' TaskCount='14' Threshold='10'>
            <Commit ID='r10323' Person='bren.coppick' Time='10/4/2013 10:50 AM' />
            <Commit ID='r10324' Person='will.meary' Time='10/4/2013 10:51 AM' />
        </Flag>
    </Run>
    <Run Number='2' DateTime='10/4/2013 1:23 PM' Passed='False'>
        <Rule ID='MID-011' TaskCount='1' Threshold='0' FailOn='Increase' Breaking='true' Description='Align widgets to the left' />
        <Rule ID='MID-033' TaskCount='19' Threshold='10' FailOn='Increase' Breaking='true' Description='Gadgets should be properly wired' />

        <Flag RuleID='MID-033' TaskCount='14' Threshold='10'>
            <Commit ID='r10323' Person='bren.coppick' Time='10/4/2013 10:50 AM' />
            <Commit ID='r10324' Person='will.meary' Time='10/4/2013 10:51 AM' />
        </Flag>
        <Flag RuleID='MID-033' TaskCount='19' Threshold='10'>
            <Commit ID='r10338' Person='bren.coppick' Time='10/4/2013 1:22 PM' />
        </Flag>
        <Flag RuleID='MID-011' TaskCount='1' Threshold='0'>
            <Commit ID='r10339' Person='mal.govinnia' Time='10/4/2013 1:22 PM' />
        </Flag>
    </Run>

</RunHistory>";
            RunHistory history = _librarian.ReadRunHistory( XDocument.Parse( historyXML ) );

            var run = history.Runs.ElementAt( 0 );
            var flags = run.Flags;

            Assert.That( flags.Count(), Is.EqualTo( 1 ) );

            Flag flag = flags[0];
            Assert.That( flag.RuleID, Is.EqualTo( "MID-033" ) );
            Assert.That( flag.TaskCount, Is.EqualTo( 14 ) );
            Assert.That( flag.Threshold, Is.EqualTo( 10 ) );

            Assert.That( flag.Commits.Count(), Is.EqualTo( 2 ) );
            Commit commit = flag.Commits[0];
            Assert.That( commit.ID, Is.EqualTo( "r10323" ) );
            Assert.That( commit.Person, Is.EqualTo( "bren.coppick" ) );
            Assert.That( commit.Time, Is.EqualTo( "10/4/2013 10:50 AM" ) );
        }

        [Test]
        public void ParseRun_parses_direct_attributes()
        {
            RunHistoryEntry run;
            XElement runXml = XElement.Parse( "<Run Number='1' DateTime='10/4/2013 10:52 AM' Passed='False'/>" );
            run = _librarian.ParseRun( runXml );
            
            Assert.That( run.Number, Is.EqualTo( 1 ) );
            Assert.That( run.Date, Is.EqualTo( DateTime.Parse( "10/4/2013 10:52 AM" ) ) );
            Assert.That( run.Passed, Is.False );
        }

        [Test]
        public void ParseRun_collects_Rules()
        {
            string runXml = @"
    <Run Number='1' DateTime='10/4/2013 10:52 AM' Passed='False'>
        <Rule ID='MID-011' TaskCount='0' Threshold='0' FailOn='Increase' Breaking='false' Description='Align widgets to the left' />
        <Rule ID='MID-033' TaskCount='14' Threshold='10' FailOn='Increase' Breaking='true' Description='Gadgets should be properly wired' />

        <Flag RuleID='MID-033' TaskCount='14' Threshold='10'>
            <Commit ID='r10323' Person='bren.coppick' Time='10/4/2013 10:50 AM' />
            <Commit ID='r10324' Person='will.meary' Time='10/4/2013 10:51 AM' />
        </Flag>
    </Run>
";
            var run = _librarian.ParseRun( XElement.Parse( runXml ) );

            Assert.That( run.RuleResults.Keys.Count(), Is.EqualTo( 2 ) );
        }

        [Test]
        public void ParseRun_collects_Flags()
        {
            string runXml = @"
            <Run Number='1' DateTime='10/4/2013 10:52 AM' Passed='False'>
                <Rule ID='MID-011' TaskCount='0' Threshold='0' FailOn='Increase' Breaking='false' Description='Align widgets to the left' />
                <Rule ID='MID-033' TaskCount='14' Threshold='10' FailOn='Increase' Breaking='true' Description='Gadgets should be properly wired' />

                <Flag RuleID='MID-033' TaskCount='14' Threshold='10'>
                    <Commit ID='r10323' Person='bren.coppick' Time='10/4/2013 10:50 AM' />
                    <Commit ID='r10324' Person='will.meary' Time='10/4/2013 10:51 AM' />
                </Flag>

                <Flag RuleID='MID-034' TaskCount='8' Threshold='5'>
                    <Commit ID='r10325' Person='bren.coppick' Time='10/4/2013 11:50 AM' />
                </Flag>
            </Run>
        ";

            var run = _librarian.ParseRun( XElement.Parse( runXml ) );

            Assert.That( run.Flags.Count, Is.EqualTo( 2 ) );
        }

        [Test]
        public void ParseFlag_collects_Commits()
        {
            string flagXml = @"
                <Flag RuleID='MID-033' TaskCount='14' Threshold='10'>
                    <Commit ID='r10323' Person='bren.coppick' Time='10/4/2013 10:50 AM' />
                    <Commit ID='r10324' Person='will.meary' Time='10/4/2013 10:51 AM' />
                </Flag>
        ";

            var flag = _librarian.ParseFlag( XElement.Parse( flagXml ) );

            Assert.That( flag.Commits.Count, Is.EqualTo( 2 ) );
        }

        [Test]
        public void ParseRule_parses_direct_attributes()
        {
            HistoricRuleResult rule;
            XElement ruleXml = XElement.Parse( "<Rule ID='MID-011' TaskCount='5' Threshold='7' FailOn='Increase' Breaking='false' Description='Align widgets to the left' />" );

            rule = _librarian.ParseRule( ruleXml );
            Assert.That( rule.ID, Is.EqualTo( "MID-011" ) );
            Assert.That( rule.TaskCount, Is.EqualTo( 5 ) );
            Assert.That( rule.Threshold, Is.EqualTo( 7 ) );
            Assert.That( rule.FailOn, Is.EqualTo( RuleFailOn.Increase ) );
            Assert.That( rule.Breaking, Is.EqualTo( false ) );
            Assert.That( rule.Description, Is.EqualTo( "Align widgets to the left" ) );
        }

        [Test]
        public void ParseFlag_parses_direct_attributes()
        {
            XElement flagXml = XElement.Parse( "<Flag RuleID='MID-033' TaskCount='14' Threshold='10' />" );

            Flag flag = _librarian.ParseFlag( flagXml );
            Assert.That( flag.RuleID, Is.EqualTo( "MID-033" ) );
            Assert.That( flag.TaskCount, Is.EqualTo( 14 ) );
            Assert.That( flag.Threshold, Is.EqualTo( 10 ) );
        }

        [Test]
        public void ParseCommit_parses_direct_attributes()
        {
            XElement commitXml = XElement.Parse( "<Commit ID='r10323' Person='bren.coppick' Time='10/4/2013 10:50 AM'/>" );

            Commit commit = _librarian.ParseCommit( commitXml );

            Assert.That( commit.ID, Is.EqualTo( "r10323" ) );
            Assert.That( commit.Person, Is.EqualTo( "bren.coppick" ) );
            Assert.That( commit.Time, Is.EqualTo( "10/4/2013 10:50 AM" ) );
        }
    }
}
