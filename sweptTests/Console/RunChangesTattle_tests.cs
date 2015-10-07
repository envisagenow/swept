using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using System.IO;

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

        [Test]
        public void OneImprovement_Produces_Happy_Message()
        {
            var changes = new RunChanges();

            FileChange file = new FileChange { Changed = true };
            file.Rules.Add(new RuleChange { Was = 4, Is = 3 });
            changes.Files.Add(file);

            var tattle = changes.TattleReport();

            Assert.That(tattle, Is.EqualTo("1 improvement. Way to go!"));
        }

        [Test]
        public void Improvements_accumulate_across_all_rules()
        {
            var changes = new RunChanges();

            FileChange file = new FileChange { Changed = true };
            file.Rules.Add(new RuleChange { ID = "first", Was = 14, Is = 3 });
            file.Rules.Add(new RuleChange { ID = "second", Was = 18, Is = 2 });
            changes.Files.Add(file);

            var tattle = changes.TattleReport();

            Assert.That(file.Rules, Has.Count.EqualTo(2));
            Assert.That(tattle, Is.EqualTo("27 improvements. Way to go!"));
        }

        [Test]
        public void OneWorsening_Produces_Sad_Message()
        {
            var changes = new RunChanges();

            FileChange file = new FileChange { Changed = true };
            file.Rules.Add(new RuleChange { Was = 13, Is = 14 });
            changes.Files.Add(file);

            var tattle = changes.TattleReport();

            Assert.That(tattle, Is.EqualTo("This is no good at all. Please fix this."));
        }
    }
}
