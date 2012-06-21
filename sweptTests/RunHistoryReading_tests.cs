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
            var args = new Arguments( new string[] { "library:foo.library", "history:foo.history" }, storage, Console.Out );
            _librarian = new BuildLibrarian( args, storage );
        }

        [Test]
        public void Empty_run_history_causes_no_problem()
        {
            var historyXml = XDocument.Parse( "<RunHistory />" );

            RunHistory history = _librarian.ReadRunHistory( historyXml );

            Assert.That( history.Runs.Count, Is.EqualTo( 0 ) );
        }

        [TestCase( 12, "9/14/2012 2:44:02 AM", 60 )]
        [TestCase( 14, "5/11/2012 7:28:02 AM", 54 )]
        public void We_can_read_history_from_XML_into_domain( int runNumber, string dateString, int violationsCount )
        {

            var historyXml = XDocument.Parse( string.Format(
@"<RunHistory>
  <Run Number=""{3}"" DateTime=""{2}"" Passed=""false"">
    <Change ID=""{0}"" Violations=""{1}"" />
    <Change ID=""always the same"" Violations=""44"" />
  </Run>
  <Run Number=""1100"" DateTime=""1/1/2022 3:20:14 PM"" Passed=""true"">
    <Change ID=""always the same"" Violations=""44"" />
  </Run>

</RunHistory>", "silly problem", violationsCount, dateString, runNumber ) );


            RunHistory history = _librarian.ReadRunHistory( historyXml );

            RunHistoryEntry firstRun = history.Runs[0];
            Assert.That( firstRun.Date, Is.EqualTo( DateTime.Parse( dateString ) ) );
            Assert.That( firstRun.Number, Is.EqualTo( runNumber ) );

            Assert.That( firstRun.Violations["silly problem"], Is.EqualTo( violationsCount ) );
            Assert.That( firstRun.Violations["always the same"], Is.EqualTo( 44 ) );
            Assert.That( firstRun.Passed, Is.False );

            RunHistoryEntry secondRun = history.Runs[1];
            Assert.That( secondRun.Date, Is.EqualTo( DateTime.Parse( "1/1/2022 3:20:14 PM" ) ) );
            Assert.That( secondRun.Number, Is.EqualTo( 1100 ) );
            Assert.That( secondRun.Violations.Count(), Is.EqualTo( 1 ) );
            Assert.That( secondRun.Passed );
        }
    }
}
