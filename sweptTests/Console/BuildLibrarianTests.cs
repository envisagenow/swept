//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.Linq;
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
            var runHistory = new RunHistory();
            var results = new Dictionary<string, HistoricRuleResult>();
            results.Add( "foo", new HistoricRuleResult { ID = "foo", Violations = 2, Prior = 1, FailOn = RuleFailOn.Increase, Breaking = true } );
            runHistory.AddEntry( new RunHistoryEntry
            {
                Number = 22,
                Date = DateTime.Parse( "4/4/2012 10:25:02 AM" ),
                RuleResults = results,
                Passed = false
            } );

            var resultsNext = new Dictionary<string, HistoricRuleResult>();
            resultsNext.Add( "bar", new HistoricRuleResult { ID = "bar", Violations = 0, Prior = 2, FailOn = RuleFailOn.None, Breaking = false } );
            runHistory.AddEntry( new RunHistoryEntry
            {
                Number = 23,
                Date = DateTime.Parse( "4/7/2012 10:25:03 AM" ),
                RuleResults = resultsNext,
                Passed = true
            } );

            _librarian.WriteRunHistory( runHistory );

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

    }
}
