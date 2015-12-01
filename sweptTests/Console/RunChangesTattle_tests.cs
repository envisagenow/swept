using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace swept.Tests
{
    [TestFixture]
    public class RunChangesTattle_tests
    {
        private BuildLibrarian _librarian;
        private MockStorageAdapter _storage;
        private Arguments _args;

        [SetUp]
        public void SetUp()
        {
            _storage = new MockStorageAdapter();
            _args = new Arguments(new string[] { "library:foo.library", "changes:foo.changes" }, _storage);
            _librarian = new BuildLibrarian(_args, _storage);
        }

        [Test]
        public void Empty_RunChanges_produces_no_change_tattle()
        {
            var changes = new RunChanges();

            var tattle = changes.TattleReport();

            Assert.That(tattle, Is.EqualTo("No changes from baseline.  You're free to commit, boss!"));
        }

        [TestCase(2, 2, "1 improvement.")]
        [TestCase(22, 20, "3 improvements.")]
        public void Improvements_accumulate_across_all_rules(int was, int isNow, string expectedMessage)
        {
            var changes = new RunChanges();

            FileChange file = new FileChange { Changed = true };
            file.Rules.Add(new RuleChange { ID = "first", Was = 1, Is = 0 });
            file.Rules.Add(new RuleChange { ID = "second", Was = was, Is = isNow });
            changes.Files.Add(file);

            var tattle = changes.TattleReport();

            Assert.That(tattle, Is.StringStarting(expectedMessage));
        }

        [Test]
        public void Improvements_report_improved_file_names_and_counts()
        {
            var changes = new RunChanges();

            FileChange file = new FileChange { Name = "foo.cs", Changed = true };
            file.Rules.Add(new RuleChange { ID = "first", Was = 14, Is = 3 });
            file.Rules.Add(new RuleChange { ID = "second", Was = 18, Is = 2 });
            changes.Files.Add(file);

            FileChange another = new FileChange { Name = "fla.cs", Changed = true };
            another.Rules.Add(new RuleChange { ID = "first", Was = 1, Is = 0 });
            changes.Files.Add(another);

            FileChange boring = new FileChange { Name = "oga.cs", Changed = true };
            boring.Rules.Add(new RuleChange { ID = "first", Was = 1, Is = 1 });
            changes.Files.Add(boring);

            var tattle = changes.TattleReport();

            string expectedMessage = "28 improvements.\n\tfoo.cs: 27 improvements.\r\n\tfla.cs: 1 improvement.\r\n";

            Assert.That(tattle, Is.EqualTo(expectedMessage));
        }


        [TestCase(13, 14, "1 regression.")]
        [TestCase(10, 14, "4 regressions.")]
        public void AnyRegression_Produces_warning_even_when_Improvements_exist(int was, int isNow, string regressionCountMessage)
        {
            var changes = new RunChanges();

            FileChange file = new FileChange { Changed = true };
            file.Rules.Add(new RuleChange { Was = was, Is = isNow });
            file.Rules.Add(new RuleChange { Was = 88, Is = 22 });
            changes.Files.Add(file);

            FileChange improvedFile = new FileChange { Name = "improved.cs", Changed = true };
            improvedFile.Rules.Add(new RuleChange { Was = 88, Is = 22 });
            changes.Files.Add(improvedFile);

            var tattle = changes.TattleReport();

            Assert.That(tattle, Is.StringStarting(regressionCountMessage));
        }


        [Test]
        public void Regressions_report_regresssed_file_names_and_counts()
        {
            var changes = new RunChanges();

            FileChange file = new FileChange { Name = "foo.cs", Changed = true };
            file.Rules.Add(new RuleChange { ID = "first", Was = 14, Is = 15 });
            file.Rules.Add(new RuleChange { ID = "second", Was = 18, Is = 22 });
            changes.Files.Add(file);

            FileChange another = new FileChange { Name = "fla.cs", Changed = true };
            another.Rules.Add(new RuleChange { ID = "first", Was = 1, Is = 2 });
            changes.Files.Add(another);

            FileChange boring = new FileChange { Name = "oga.cs", Changed = true };
            boring.Rules.Add(new RuleChange { ID = "first", Was = 1, Is = 1 });
            changes.Files.Add(boring);

            var tattle = changes.TattleReport();

            string expectedMessage = "6 regressions.\n\tfoo.cs:  5 regressions.\r\n\tfla.cs:  1 regression.\r\n";

            Assert.That(tattle, Is.EqualTo(expectedMessage));
        }


        [Test]
        public void File_regression_details()
        {
            var changes = new RunChanges();

            FileChange file = new FileChange { Name = "foo.cs", Changed = true };
            file.Rules.Add(new RuleChange { ID = "rule12", Was = 10, Is = 14 });
            changes.Files.Add(file);

            StringBuilder sb = new StringBuilder();
            int regressions = changes.TattleFileRegression(file, sb);

            Assert.That(regressions, Is.EqualTo(4));
            Assert.That(sb.ToString(), Is.EqualTo("\tfoo.cs:  4 regressions.\r\nrule12:  4 regressions.\r\n"));
 
        }

        [TestCase("second", 18, 22, "second:  4 regressions.\r\n")]
        [TestCase("impRule66", 18, 3, "impRule66:  15 improvements.\r\n")]
        [TestCase("dull", 4, 4, "")]
        public void Rule_reports_its_delta(string id, int wasCount, int isCount, string expected)
        {
            var changes = new RunChanges();
            var regressingRule = new RuleChange { ID = id, Was = wasCount, Is = isCount };

            var tattle = changes.TattleRuleDelta(regressingRule);

            Assert.That(tattle, Is.EqualTo(expected));
        }

        //  Details are coming in, so the expected result is growing rapidly.  Fix test after stabilizing.
        //[Test]
        //public void Mixed_results_report_regresssions_first()
        //{
        //    var changes = new RunChanges();

        //    FileChange mixed = new FileChange { Name = "foo.cs", Changed = true };
        //    mixed.Rules.Add(new RuleChange { ID = "first", Was = 14, Is = 15 });
        //    mixed.Rules.Add(new RuleChange { ID = "second", Was = 18, Is = 12 });
        //    changes.Files.Add(mixed);

        //    FileChange worse = new FileChange { Name = "fla.cs", Changed = true };
        //    worse.Rules.Add(new RuleChange { ID = "first", Was = 1, Is = 2 });
        //    changes.Files.Add(worse);

        //    FileChange boring = new FileChange { Name = "oga.cs", Changed = true };
        //    boring.Rules.Add(new RuleChange { ID = "first", Was = 1, Is = 1 });
        //    changes.Files.Add(boring);

        //    FileChange better = new FileChange { Name = "lol.cs", Changed = true };
        //    better.Rules.Add(new RuleChange { ID = "second", Was = 31, Is = 6 });
        //    changes.Files.Add(better);

        //    var tattle = changes.TattleReport();

        //    string expectedMessage = "2 regressions.\n\tfoo.cs:  1 regression.\r\n\tfla.cs:  1 regression.\r\n\r\n31 improvements.\n\tfoo.cs: 6 improvements.\r\n\tlol.cs: 25 improvements.\r\n";

        //    Assert.That(tattle, Is.EqualTo(expectedMessage));
        //}

    }
}
