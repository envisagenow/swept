using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace swept.Tests
{
    [TestFixture]
    public class Foresight_tests
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
        public void Empty_RunChanges_foretells_no_change()
        {
            var changes = new RunChanges();

            var report = changes.ForesightReport();

            Assert.That(report, Is.EqualTo("No changes from baseline.  You're free to commit, boss!"));
        }

        [TestCase(8, 7, "2 improvements.")] //  One improvement in another rule plus one here = two.
        [TestCase(3, 3, "1 improvement.")]  //  Swept is not Gollum.
        public void Improvements_accumulate_across_all_rules(int was, int isNow, string expectedMessage)
        {
            var changes = new RunChanges();

            FileChange file = new FileChange { Changed = true };
            file.Rules.Add(new RuleChange { ID = "first", Was = 1, Is = 0 });
            file.Rules.Add(new RuleChange { ID = "second", Was = was, Is = isNow });
            changes.Files.Add(file);

            var report = changes.ForesightReport();

            Assert.That(report, Is.StringStarting(expectedMessage));
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

            var report = changes.ForesightReport();

            string expectedMessage = "28 improvements.\n\tfoo.cs: 27 improvements.\r\n\tfla.cs: 1 improvement.\r\n";

            Assert.That(report, Is.EqualTo(expectedMessage));
        }


        [TestCase(13, 14, "1 regression.")]
        [TestCase(10, 14, "4 regressions.")]
        [TestCase(10, 10, "72 improvements.")]  //  No regressions text when no regressions exist.
        public void AnyRegression_Produces_warning_even_when_Improvements_exist(int was, int isNow, string regressionCountMessage)
        {
            var changes = new RunChanges();

            FileChange file = new FileChange { Changed = true };
            file.Rules.Add(new RuleChange { ID = "ARR", Was = was, Is = isNow });
            file.Rules.Add(new RuleChange { ID = "ADU", Was = 28, Is = 22 });   //  Some improvements inside the regressing file
            changes.Files.Add(file);
            changes.Rules.Add(new RuleDescription { ID = "ARR", Description = "Arr, matey." });
            changes.Rules.Add(new RuleDescription { ID = "ADU", Description = "Adumbrations!" });

            FileChange improvedFile = new FileChange { Name = "improved.cs", Changed = true };
            improvedFile.Rules.Add(new RuleChange { Was = 88, Is = 22 });   //  Some improvements in another file
            changes.Files.Add(improvedFile);

            var report = changes.ForesightReport();

            Assert.That(report, Is.StringStarting(regressionCountMessage));
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

            changes.Rules.Add(new RuleDescription { ID = "first", Description = "The most important thing" });
            changes.Rules.Add(new RuleDescription { ID = "second", Description = "The next most important thing" });

            var report = changes.ForesightReport();

            string expectedMessage = "6 regressions.\n\tfoo.cs:  5 regressions.\r\nfirst (The most important thing):  1 regression.\r\nsecond (The next most important thing):  4 regressions.\r\n\tfla.cs:  1 regression.\r\nfirst (The most important thing):  1 regression.\r\n";

            Assert.That(report, Is.EqualTo(expectedMessage));
        }


        [Test]
        public void File_regression_details()
        {
            var changes = new RunChanges();

            FileChange file = new FileChange { Name = "foo.cs", Changed = true };
            file.Rules.Add(new RuleChange { ID = "rule12", Was = 10, Is = 14 });
            changes.Files.Add(file);
            changes.Rules.Add(new RuleDescription { ID = "rule12", Description = "Don't use that code." });

            StringBuilder sb = new StringBuilder();
            int regressions = changes.ForesightFileRegression(file, sb);

            Assert.That(regressions, Is.EqualTo(4));
            Assert.That(sb.ToString(), Is.EqualTo("\tfoo.cs:  4 regressions.\r\nrule12 (Don't use that code.):  4 regressions.\r\n"));
        }


        [TestCase("second", "The next most important thing", 18, 22, "second (The next most important thing):  4 regressions.\r\n")]
        [TestCase("impRule66", "IMPORTANT!", 18, 3, "impRule66 (IMPORTANT!):  15 improvements.\r\n")]
        [TestCase("dull", "Blah blah blah", 4, 4, "")]
        public void Rule_reports_its_delta(string id, string description, int wasCount, int isCount, string expected)
        {
            var changes = new RunChanges();
            var regressingRule = new RuleChange { ID = id, Was = wasCount, Is = isCount };
            changes.Rules.Add(new RuleDescription { Description = description, ID = id });

            var foresight = changes.ForesightRuleDelta(regressingRule);

            Assert.That(foresight, Is.EqualTo(expected));
        }
    }
}
