using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace swept.Tests
{
    [TestFixture]
    public class RunHistory_tests
    {
        private RunHistory _history;

        [SetUp]
        public void SetUp()
        {
            _history = new RunHistory();
        }

        [Test]
        public void Starts_with_no_runs_ready_for_first()
        {
            Assert.That( _history.Runs.Count, Is.EqualTo( 0 ) );
            Assert.That( _history.NextRunNumber, Is.EqualTo( 1 ) );
        }

        [Test]
        public void Can_add_a_Run()
        {
            var run = new RunHistoryEntry();
            run.Number = 27;
            _history.AddRun( run );

            Assert.That( _history.Runs.Count, Is.EqualTo( 1 ) );
            Assert.That( _history.NextRunNumber, Is.EqualTo( 28 ) );
        }

        [Test]
        public void Can_get_waterline_for_a_Change()
        {
            _history.AddRun( new RunHistoryEntry() );

            var run = new RunHistoryEntry();
            run.Number = 401;
            run.Violations["Eliminate Crystal Reports"] = 6;
            run.Violations["Update widgets to widgets 2.0"] = 45;
            _history.AddRun( run );

            Assert.That( _history.WaterlineFor( "Eliminate Crystal Reports" ), Is.EqualTo( 6 ) );
            Assert.That( _history.WaterlineFor( "Update widgets to widgets 2.0" ), Is.EqualTo( 45 ) );
        }

        [Test]
        public void Waterline_on_an_empty_history_starts_high()
        {
            Assert.That( _history.WaterlineFor( "Update widgets to widgets 2.0" ), Is.EqualTo( RunHistory.HighWaterLine ) );
        }

        [Test]
        public void Waterline_of_an_unfound_Change_starts_high()
        {
            var run = new RunHistoryEntry();
            run.Number = 401;
            run.Violations["Eliminate Crystal Reports"] = 6;
            _history.AddRun( run );

            Assert.That( _history.WaterlineFor( "Update widgets to widgets 2.0" ), Is.EqualTo( RunHistory.HighWaterLine ) );
        }


        [Test]
        public void Can_generate_run_entry_from_results()
        {
            var change = new Change()
            {
                ID = "basic entry",
                Description = "simple",
                RunFail = RunFailMode.Over,
                RunFailOverLimit = 20
            };
            var sourceClauseMatch = new Dictionary<SourceFile, ClauseMatch>();

            var failedSource = new SourceFile( "some_file.cs" );

            List<int> violationLines = new List<int>();
            for (int i = 0; i < 7; i++)
            {
                violationLines.Add( (i * 7) + 22 );  //arbitrary lines throughout the source file had this problem.
            }
            ClauseMatch failedClause = new LineMatch( violationLines );
            sourceClauseMatch[failedSource] = failedClause;

            var changeViolations = new Dictionary<Change, Dictionary<SourceFile, ClauseMatch>>();
            changeViolations[change] = sourceClauseMatch;


            var noMatches = new Dictionary<SourceFile, ClauseMatch>();
            var happyChange = new Change
            {
                ID = "no problem",
                Description = "the app reports changes with no issues",
                RunFail = RunFailMode.Any,
            };

            changeViolations[happyChange] = noMatches;

            DateTime nowish = DateTime.Now;

            RunHistoryEntry entry = _history.GenerateEntry( changeViolations, nowish );

            Assert.That( entry.Number, Is.EqualTo( 1 ) );
            Assert.That( entry.Date, Is.EqualTo( nowish ) );
            Assert.That( entry.Violations.Count, Is.EqualTo( 2 ) );
            Assert.That( entry.Violations[change.ID], Is.EqualTo( 7 ) );
            Assert.That( entry.Violations[happyChange.ID], Is.EqualTo( 0 ) );
        }

    }
}
