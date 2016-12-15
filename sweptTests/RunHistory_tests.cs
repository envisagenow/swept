//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2016 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
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
            Assert.That(_history.Runs.Count(), Is.EqualTo(0));
            Assert.That(_history.NextRunNumber, Is.EqualTo(1));
        }

        [Test]
        public void Can_add_a_Run()
        {
            var run = new RunEntry() { Number = 27 };
            _history.AddEntry(run);

            Assert.That(_history.Runs.Count(), Is.EqualTo(1));
            Assert.That(_history.NextRunNumber, Is.EqualTo(28));
        }

        [Test]
        public void Can_get_waterline_for_a_Rule()
        {
            _history.AddEntry(new RunEntry());

            var entry = new RunEntry { Passed = true, Number = 401 };
            entry.RuleResults["Eliminate Crystal Reports"] = new RuleResult { TaskCount = 6 };
            entry.RuleResults["Update widgets to widgets 2.0"] = new RuleResult { TaskCount = 45 };
            _history.AddEntry(entry);

            Assert.That(_history.WaterlineFor("Eliminate Crystal Reports"), Is.EqualTo(6));
            Assert.That(_history.WaterlineFor("Update widgets to widgets 2.0"), Is.EqualTo(45));
        }

        [Test]
        public void Waterline_on_an_empty_history_starts_high()
        {
            Assert.That(_history.WaterlineFor("Update widgets to widgets 2.0"), Is.EqualTo(RunHistory.HighWaterLine));
        }

        [Test]
        public void Waterline_of_an_unfound_Rule_starts_high()
        {
            var entry = new RunEntry() { Number = 401 };
            entry.RuleResults["Eliminate Crystal Reports"] = new RuleResult { TaskCount = 6 };
            _history.AddEntry(entry);

            Assert.That(_history.WaterlineFor("Update widgets to widgets 2.0"), Is.EqualTo(RunHistory.HighWaterLine));
        }

        [Test]
        public void Waterline_tracks_progress_made_by_failed_runs()
        {
            var entry = new RunEntry() { Number = 401, Passed = true };
            entry.RuleResults["Eliminate Crystal Reports"] = new RuleResult { TaskCount = 6 };
            _history.AddEntry(entry);

            // run 402 failed as a whole...
            var failingEntry = new RunEntry() { Number = 402, Passed = false };
            //  ...though we did away with five of our legacy reports
            failingEntry.RuleResults["Eliminate Crystal Reports"] = new RuleResult { TaskCount = 1 };
            _history.AddEntry(failingEntry);

            //  Our water line should mark progress made even during failed runs
            Assert.That(_history.WaterlineFor("Eliminate Crystal Reports"), Is.EqualTo(1));
        }

        [Test]
        public void Waterline_does_not_regress()
        {
            var entry = new RunEntry() { Number = 401, Passed = true };
            entry.RuleResults["Eliminate Crystal Reports"] = new RuleResult { TaskCount = 6 };
            _history.AddEntry(entry);

            var failingEntry = new RunEntry() { Number = 402, Passed = false };
            failingEntry.RuleResults["Eliminate Crystal Reports"] = new RuleResult { TaskCount = 22, Threshold = 6 };
            _history.AddEntry(failingEntry);

            //  failed runs do not regress our waterline.
            Assert.That(_history.WaterlineFor("Eliminate Crystal Reports"), Is.EqualTo(6));
        }

        [Test]
        public void LatestPassingRun_is_null_when_history_lacks_passing_runs()
        {
            Assert.That(_history.Runs.Count(), Is.EqualTo(0));
            Assert.Null(_history.LatestPassingRun);

            _history.AddEntry(new RunEntry {
                Date = DateTime.Now,
                Number = 1,
                Passed = false
            });

            Assert.That(_history.Runs.Count(), Is.GreaterThan(0));
            Assert.Null(_history.LatestPassingRun);
        }

        [Test]
        public void LatestPassingRun_decides_latest_based_on_run_Number()
        {
            RunEntry shouldBeLatest = new RunEntry {
                Date = DateTime.Now,
                Number = 2,
                Passed = true
            };

            RunEntry shouldNotBeLatest = new RunEntry {
                Date = DateTime.Now.AddDays(2),
                Number = 1,
                Passed = true
            };

            //  this entry has the highest run number
            _history.AddEntry(shouldBeLatest);
            
            //  this entry ran most recently, and it's physically latest in the history
            _history.AddEntry(shouldNotBeLatest);

            Assert.That(_history.LatestPassingRun, Is.SameAs(shouldBeLatest));

            //  This test documents the decision, but there's no downstream dependency (as
            //  of December 2016) on this implementation.  It could as well be by date.
        }

        [Test]
        public void GetThreshold_of_unrecognized_type_complains_clearly()
        {
            var ex = Assert.Throws<Exception>(() => _history.GetThreshold(new Rule { FailOn = (RuleFailOn)(-1) }));
            Assert.That(ex.Message, Is.EqualTo("Don't know the case [-1]."));
        }
    }
}
