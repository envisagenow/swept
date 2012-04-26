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
        public void Starts_with_no_runs()
        {
            Assert.That( _history.Runs.Count, Is.EqualTo( 0 ) );
        }

        [Test]
        public void Knows_next_run_number()
        {
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
        public void Can_generate_entry()
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
