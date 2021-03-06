﻿//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2013 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
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
    <Rule ID=""foo"" TaskCount=""2"" Threshold=""2"" FailOn=""None"" Breaking=""false"" Description=""I want to keep my eyes on my level of foo."" />
  </Run>
</RunHistory>" );
            
            var runHistory = _librarian.ReadRunHistory();

            Assert.That( runHistory.Runs.Count(), Is.EqualTo(1) );
        }


        [Test]
        public void When_RunHistory_file_is_missing_a_new_one_is_created()
        {
            _storage.RunHistoryNotFoundException = new FileNotFoundException();

            var runHistory = _librarian.ReadRunHistory();
            Assert.That( runHistory.Runs.Count(), Is.EqualTo( 0 ) );
        }

        [Test]
        public void When_we_write_run_history_it_is_stored_to_disk()
        {
            var entry = new RunEntry {
                Number = 22,
                Date = DateTime.Parse( "4/4/2012 10:25:02 AM" ),
                Passed = false
            };
            entry.AddResult( "foo", true, RuleFailOn.Increase, 1, 2, "Upgrade from old stylesheets" );

            var nextEntry = new RunEntry {
                Number = 23,
                Date = DateTime.Parse( "4/7/2012 10:25:03 AM" ),
                Passed = true
            };
            nextEntry.AddResult( "bar", false, RuleFailOn.None, 2, 0, "XML islands no longer supported" );

            var runHistory = new RunHistory();
            runHistory.AddEntry( entry );
            runHistory.AddEntry( nextEntry );

            var librarian = new BuildLibrarian( new Arguments( new string[] { "trackhistory", "library:foo.library", "history:foo.history" }, _storage ), _storage );
            librarian.WriteRunHistory( runHistory );

            var expectedHistory =
@"<RunHistory>
  <Run Number=""22"" DateTime=""4/4/2012 10:25:02 AM"" Passed=""False"">
    <Rule ID=""foo"" TaskCount=""2"" Threshold=""1"" FailOn=""Increase"" Breaking=""true"" Description=""Upgrade from old stylesheets"" />
  </Run>
  <Run Number=""23"" DateTime=""4/7/2012 10:25:03 AM"" Passed=""True"">
    <Rule ID=""bar"" TaskCount=""0"" Threshold=""2"" FailOn=""None"" Breaking=""false"" Description=""XML islands no longer supported"" />
  </Run>
</RunHistory>";

            Assert.That( _storage.RunHistory.ToString(), Is.EqualTo( expectedHistory ) );
        }

        [Test]
        public void RunHistory_Flags_are_stored()
        {
            var entry = new RunEntry {
                Number = 22,
                Date = DateTime.Parse( "4/4/2012 10:25:02 AM" ),
                Passed = false
            };
            entry.AddResult( "foo", true, RuleFailOn.Increase, 1, 2, "Upgrade from old stylesheets" );

            var flag = new Flag {
                RuleID = "foo",
                TaskCount = 2,
                Threshold = 1,
            };

            var commit = new Commit {
                ID = "r10324",
                Person = "will.meary",
                Time = "4/4/2012 10:24:32 AM"
            };
            flag.Commits.Add(commit);
            var runHistory = new RunHistory();
            runHistory.AddEntry( entry );
            entry.Flags.Add( flag );  //entry.AddFlag( flag );

            var librarian = new BuildLibrarian( new Arguments( new string[] { "trackhistory", "library:foo.library", "history:foo.history" }, _storage ), _storage );
            librarian.WriteRunHistory( runHistory );

            var expectedHistory =
@"<RunHistory>
  <Run Number=""22"" DateTime=""4/4/2012 10:25:02 AM"" Passed=""False"">
    <Rule ID=""foo"" TaskCount=""2"" Threshold=""1"" FailOn=""Increase"" Breaking=""true"" Description=""Upgrade from old stylesheets"" />
    <Flag RuleID='foo' TaskCount='2' Threshold='1'>
        <Commit ID='r10324' Person='will.meary' Time='4/4/2012 10:24:32 AM' />
    </Flag>
  </Run>
</RunHistory>";

            Assert.That( _storage.RunHistory.ToString(), Is.EqualTo( XDocument.Parse(expectedHistory).ToString()) );
        }

        [Test]
        public void When_xml_contains_flag_elements_then_Flags_are_in_RunHistory()
        {
            _storage.RunHistory = XDocument.Parse(
@"<RunHistory>
  <Run Number=""22"" DateTime=""4/4/2012 10:25:02 AM"" Passed=""False"">
    <Rule ID=""foo"" TaskCount=""2"" Threshold=""1"" FailOn=""Increase"" Breaking=""true"" Description=""Upgrade from old stylesheets"" />

    <Flag RuleID='foo' TaskCount='2' Threshold='1'>
        <Commit ID='r10324' Person='will.meary' Time='4/4/2012 10:24:32 AM' />
    </Flag>
  </Run>
</RunHistory>" );

            var runHistory = _librarian.ReadRunHistory();

            Assert.That( runHistory.Runs.ElementAt(0).Flags.Count(), Is.EqualTo( 1 ) );
        }

        [Test]
        public void Run_history_is_not_stored_to_disk_when_trackhistory_arg_not_set()
        {
            var entry = new RunEntry
            {
                Number = 22,
                Date = DateTime.Parse( "4/4/2012 10:25:02 AM" ),
                Passed = false
            };
            entry.AddResult( "foo", true, RuleFailOn.Increase, 1, 2, "Upgrade from old stylesheets" );

            var nextEntry = new RunEntry
            {
                Number = 23,
                Date = DateTime.Parse( "4/7/2012 10:25:03 AM" ),
                Passed = true
            };
            nextEntry.AddResult( "bar", false, RuleFailOn.None, 2, 0, "XML islands no longer supported" );

            var runHistory = new RunHistory();
            runHistory.AddEntry( entry );
            runHistory.AddEntry( nextEntry );

            _librarian.WriteRunHistory( runHistory );

            Assert.That( _storage.RunHistory, Is.Null  );
        }

        [Test]
        public void Empty_ChangeSet_XML_emits_empty_change_collection()
        {
            _storage.ChangeDoc = XDocument.Parse( "<new_commits></new_commits>" );
            List<Commit> changes = _librarian.ReadChangeSet();
            Assert.That( changes, Is.Empty );
        }

        private string commitXml = @"
<new_commits version='1.0'>
    <commit id='r46816' person='walter.punchline' time='2013-08-07 11:24:34 -0400 (Wed, 07 Aug 2013)' />
    <commit id='r46817' person='carla.gargunza' time='2013-08-07 11:28:08 -0400 (Wed, 07 Aug 2013)' />
    <commit id='r46818' person='ewige.quaston' time='2013-08-07 11:48:11 -0400 (Wed, 07 Aug 2013)' />
</new_commits>
";

        [Test]
        public void ChangeSet_XML_entries_are_populated()
        {
            _storage.ChangeDoc = XDocument.Parse( commitXml );
            List<Commit> changes = _librarian.ReadChangeSet();

            Assert.That( changes.Count, Is.EqualTo( 3 ) );
            
            var r46816 = changes[0];
            Assert.That( r46816.ID, Is.EqualTo( "r46816" ) );
            Assert.That( r46816.Person, Is.EqualTo( "walter.punchline" ) );
            Assert.That( r46816.Time, Is.EqualTo( "2013-08-07 11:24:34 -0400 (Wed, 07 Aug 2013)" ) );

            var r46818 = changes[2];
            Assert.That( r46818.ID, Is.EqualTo( "r46818" ) );
            Assert.That( r46818.Person, Is.EqualTo( "ewige.quaston" ) );
            Assert.That( r46818.Time, Is.EqualTo( "2013-08-07 11:48:11 -0400 (Wed, 07 Aug 2013)" ) );
        }

        [Test]
        public void Missing_ChangeSet_file_emits_empty_change_collection()
        {
            _storage.ChangeSetNotFoundException = new FileNotFoundException();

            var changes = _librarian.ReadChangeSet();
            Assert.That( changes.Count(), Is.EqualTo( 0 ) );
        }


    }
}
