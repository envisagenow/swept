using System.IO;
using NUnit.Framework;

namespace swept.Tests
{
    [TestFixture]
    public class TeamCityDelta_tests
    {
        private RunHistory _runHistory;
        private RunInspector _inspector;

        private StringWriter _stdOut;

        [SetUp]
        public void Setup()
        {
            _runHistory = new RunHistory();
            _inspector = new RunInspector(_runHistory);

            _stdOut = new StringWriter();
        }

        [Test]
        public void Empty_RunHistory_causes_empty_delta()
        {
            _inspector.GenerateDeltaTeamCityOutput(_stdOut, new RunEntry());
            Assert.That(_stdOut.ToString(), Is.EqualTo(string.Empty));
        }

        [Test]
        public void Run_with_no_prior_passing_run_shows_valid_delta()
        {
            RunEntry priorButFailingEntry = new RunEntry { Passed = false };

            //  This rule result causes this run to fail.  Delta still shows up!
            priorButFailingEntry.AddResult("917", true, RuleFailOn.Any, 0, 3, "No document.all() allowed");

            //  This rule passes, so it will lead to a delta showing with
            //  next run's improvement on this rule.
            priorButFailingEntry.AddResult("644", true, RuleFailOn.Increase, 40, 40, "Replace WR framework with DF");

            _runHistory.AddEntry(priorButFailingEntry);
            Assert.That(_runHistory.LatestPassingRun, Is.Null);

            RunEntry entry = new RunEntry { Passed = true };
            entry.AddResult("644", true, RuleFailOn.Increase, 10, 10, "Replace old stylesheets");

            _inspector.GenerateDeltaTeamCityOutput(_stdOut, entry);

            var actual = _stdOut.ToString();

            Assert.That(actual, Is.EqualTo("Swept Fix [917] No document.all() allowed: has 0 tasks, decreased from 0\r\nSwept Fix [644] Replace old stylesheets: has 10 task(s), decreased from 10\r\n"));
        }

        [Test]
        public void Delta_shows_Failures()
        {
            RunEntry entry = new RunEntry { Passed = false };
            entry.AddResult("644", true, RuleFailOn.Any, 0, 2, "Absolutely no document.all.");
            entry.AddResult("432", true, RuleFailOn.Increase, 10, 23, "Eliminate references to behavior files");

            _inspector.GenerateDeltaTeamCityOutput(_stdOut, entry);

            var actual = _stdOut.ToString();

            Assert.That(actual, Is.EqualTo("Swept Failure [644] Absolutely no document.all.: has 2 task(s), increased from 0\r\nSwept Failure [432] Eliminate references to behavior files: has 23 task(s), increased from 10\r\n"));
        }

        [Test]
        public void Delta_shows_Missing_rules()
        {
            RunEntry priorPassingEntry = new RunEntry { Passed = true };
            priorPassingEntry.AddResult("644", true, RuleFailOn.Increase, 10, 2, "Replace AjaxToolkit with JQuery");
            priorPassingEntry.AddResult("411", true, RuleFailOn.Increase, 20, 7, "Less of foo, please.");
            _runHistory.AddEntry(priorPassingEntry);

            _inspector.GenerateDeltaTeamCityOutput(_stdOut, new RunEntry { Passed = true });

            var actual = _stdOut.ToString();

            Assert.That(actual, Is.EqualTo("Swept Fix [644] Replace AjaxToolkit with JQuery: has 0 tasks, decreased from 10\r\nSwept Fix [411] Less of foo, please.: has 0 tasks, decreased from 20\r\n"));
        }

        [Test]
        public void Delta_shows_Fixes()
        {
            RunEntry priorPassingEntry = new RunEntry { Passed = true };
            priorPassingEntry.AddResult("644", true, RuleFailOn.Increase, 10, 2, "Descrip");
            priorPassingEntry.AddResult("411", true, RuleFailOn.Increase, 20, 7, "Less foo now!");
            _runHistory.AddEntry(priorPassingEntry);

            RunEntry newRun = new RunEntry { Passed = true };
            newRun.AddResult("644", true, RuleFailOn.Increase, 2, 1, "Descrip");
            newRun.AddResult("411", true, RuleFailOn.Increase, 7, 4, "Less foo now!");

            _inspector.GenerateDeltaTeamCityOutput(_stdOut, newRun);

            var actual = _stdOut.ToString();

            Assert.That(actual, Is.EqualTo("Swept Fix [644] Descrip: has 1 task(s), decreased from 2\r\nSwept Fix [411] Less foo now!: has 4 task(s), decreased from 7\r\n"));
        }
        
        [Test]
        public void Broken_rules_will_suppress_fixed_or_gone_rule_reporting()
        {
            RunEntry priorPassingEntry = new RunEntry { Passed = true };
            priorPassingEntry.AddResult("800", true, RuleFailOn.Increase, 8, 5, "foo");
            priorPassingEntry.AddResult("644", true, RuleFailOn.Increase, 10, 2, "bar");
            priorPassingEntry.AddResult("411", true, RuleFailOn.Increase, 20, 7, "baz");
            _runHistory.AddEntry(priorPassingEntry);

            RunEntry newRun = new RunEntry { Passed = false };
            newRun.AddResult("800", true, RuleFailOn.Increase, 5, 15, "foo");
            //  A rule 644 result is missing from this run...
            newRun.AddResult("411", true, RuleFailOn.Increase, 7, 4, "baz");

            _inspector.GenerateDeltaTeamCityOutput(_stdOut, newRun);

            var actual = _stdOut.ToString();

            Assert.That(actual, Is.EqualTo("Swept Failure [800] foo: has 15 task(s), increased from 5\r\n"));
        }
    }
}