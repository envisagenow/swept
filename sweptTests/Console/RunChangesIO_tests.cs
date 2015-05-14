using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using System.IO;

namespace swept.Tests
{
    [TestFixture]
    public class RunChangesIO_tests
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
        public void Empty_document_produces_Empty_RunChanges()
        {
            var result = _librarian.ReadRunChanges(new XDocument());

            Assert.That(result, Is.InstanceOf<RunChanges>());
            Assert.That(result.CurrentDateTime, Is.EqualTo(DateTime.MinValue));
            Assert.That(result.RunNumber, Is.EqualTo(1));
            Assert.That(result.Files.Count, Is.EqualTo(0));
        }


        [Test]
        public void Minimal_document_produces_Proper_RunChanges()
        {
            // as we go on, we maintain what 'empty' means for a RunChanges object.

            var changesDoc = XDocument.Parse(
@"<RunChanges RunNumber=""22"" CurrentDateTime=""4/4/2012 10:25:02 AM"" PreviousDateTime=""4/3/2012 11:27:02 AM"">
  <Rules />
  <Files />
</RunChanges>
");

            RunChanges result = _librarian.ReadRunChanges(changesDoc);

            Assert.That(result.RunNumber, Is.EqualTo(22));
            Assert.That(result.CurrentDateTime, Is.EqualTo(DateTime.Parse("4/4/2012 10:25:02 AM")));
            Assert.That(result.PreviousDateTime, Is.EqualTo(DateTime.Parse("4/3/2012 11:27:02 AM")));
            Assert.That(result.Files.Count, Is.EqualTo(0));
        }

        [Test]
        public void Minimal_Rules_element_produces_proper_Rules_collection()
        {
            var changesDoc = XDocument.Parse(
@"<RunChanges RunNumber=""17"" CurrentDateTime=""4/4/2012 10:25:02 AM"" PreviousDateTime=""4/3/2012 11:27:02 AM"">
    <Rules>
        <Rule ID=""INT-012"" Description=""Improper 'Proper' capitalization"" />
    </Rules>
</RunChanges>
");

            RunChanges run17 = _librarian.ReadRunChanges(changesDoc);

            RuleDescription int012 = run17.Rules[0];
            Assert.That(int012.ID, Is.EqualTo("INT-012"));
            Assert.That(int012.Description, Is.EqualTo("Improper 'Proper' capitalization"));
        }


        [Test]
        public void Minimal_file_element_produces_proper_FileChange()
        {
            var changesDoc = XDocument.Parse(
@"<File Name=""foo.cs"" Changed=""true"">
    <Rule ID=""INT-012"" Was=""2"" Is=""5"" />
</File>
");

            FileChange fileFoo = _librarian.ReadFileChange(changesDoc.Root);

            Assert.That(fileFoo, Is.InstanceOf<FileChange>());
            Assert.That(fileFoo.Name, Is.EqualTo("foo.cs"));
            Assert.That(fileFoo.Changed);
            Assert.That(fileFoo.Rules.Count, Is.EqualTo(1));

            RuleChange int012 = fileFoo.Rules[0];
            Assert.That(int012.ID, Is.EqualTo("INT-012"));
            Assert.That(int012.Was, Is.EqualTo(2));
            Assert.That(int012.Is, Is.EqualTo(5));
        }

        [Test]
        public void Another_file_element_produces_Proper_FileChanges()
        {
            var changesDoc = XDocument.Parse(
@"<File Name=""bar.cs"" Changed=""false"">
    <Rule ID=""INT-004"" Was=""2"" Is=""2"" />
    <Rule ID=""INT-007"" Was=""22"" Is=""22"" />
</File>
");

            FileChange fileBar = _librarian.ReadFileChange(changesDoc.Root);

            Assert.That(fileBar, Is.InstanceOf<FileChange>());
            Assert.That(fileBar.Name, Is.EqualTo("bar.cs"));
            Assert.That(fileBar.Changed, Is.False);
            Assert.That(fileBar.Rules.Count, Is.EqualTo(2));

            RuleChange rule4 = fileBar.Rules[0];
            Assert.That(rule4.ID, Is.EqualTo("INT-004"));
            Assert.That(rule4.Was, Is.EqualTo(2));
            Assert.That(rule4.Is, Is.EqualTo(2));

            RuleChange rule7 = fileBar.Rules[1];
            Assert.That(rule7.ID, Is.EqualTo("INT-007"));
            Assert.That(rule7.Was, Is.EqualTo(22));
            Assert.That(rule7.Is, Is.EqualTo(22));
        }

        [Test]
        public void Empty_inputs_produce_empty_RunChanges_document()
        {
            var currentRun = DateTime.Now;
            var previousRun = DateTime.Now.AddDays(-1);

            var expectedText = String.Format(@"<RunChanges RunNumber=""1"" CurrentDateTime=""{0}"" PreviousDateTime=""{1}"">
  <Rules />
  <Files />
</RunChanges>", currentRun, previousRun);
            _librarian.WriteRunChangesDoc(new RunChanges() { CurrentDateTime = currentRun, PreviousDateTime = previousRun }, new List<Rule>());
            string actualText = _storage.RunChanges.ToString();

            Assert.That(actualText, Is.EqualTo(expectedText));
        }


        [Test]
        public void Populated_inputs_produce_full_RunChanges_document()
        {
            var currentRun = DateTime.Now;
            var previousRun = DateTime.Now.AddDays(-1);

            var expectedText = String.Format(@"<RunChanges RunNumber=""1"" CurrentDateTime=""{0}"" PreviousDateTime=""{1}"">
  <Rules>
    <Rule ID=""INT-004"" Description=""Stale jokes replaced with fresh"" />
    <Rule ID=""INT-007"" Description=""Copy-Paste is code reuse, right?"" />
    <Rule ID=""INT-012"" Description=""Improper 'Proper' capitalization"" />
  </Rules>
  <Files>
    <File Name=""bar.cs"" Changed=""true"">
      <Rule ID=""INT-004"" Was=""2"" Is=""2"" />
      <Rule ID=""INT-007"" Was=""22"" Is=""28"" />
    </File>
    <File Name=""foo.cs"" Changed=""true"">
      <Rule ID=""INT-007"" Was=""22"" Is=""0"" />
      <Rule ID=""INT-012"" Was=""7"" Is=""99"" />
    </File>
  </Files>
</RunChanges>", currentRun, previousRun);


            RunChanges runChanges = new RunChanges() { CurrentDateTime = currentRun, PreviousDateTime = previousRun };

            var bar = new FileChange() { Changed = true, Name = "bar.cs" };
            bar.Rules.Add(new RuleChange { ID = "INT-004", Was = 2, Is = 2 });
            bar.Rules.Add(new RuleChange { ID = "INT-007", Was = 22, Is = 28 });

            var foo = new FileChange { Changed = true, Name = "foo.cs" };
            foo.Rules.Add(new RuleChange { ID = "INT-012", Was = 7, Is = 99 });
            foo.Rules.Add(new RuleChange { ID = "INT-007", Was = 22, Is = 0 });

            runChanges.Files.Add(bar);
            runChanges.Files.Add(foo);

            var ruleCatalog = new List<Rule>();
            ruleCatalog.AddRange( new Rule[] {
               new Rule { ID = "INT-004", Description = "Stale jokes replaced with fresh" },
               new Rule { ID = "INT-007", Description = "Copy-Paste is code reuse, right?" },
               new Rule { ID = "INT-012", Description = "Improper 'Proper' capitalization" },
            } );

            _librarian.WriteRunChangesDoc(runChanges, ruleCatalog);
            string actualText = _storage.RunChanges.ToString();

            Assert.That(actualText, Is.EqualTo(expectedText));
        }


        [Test]
        public void Rule_Descriptions_updated_from_Catalog()
        {
            var currentRun = DateTime.Now;
            var previousRun = DateTime.Now.AddDays(-1);

            var expectedText = String.Format(@"<RunChanges RunNumber=""1"" CurrentDateTime=""{0}"" PreviousDateTime=""{1}"">
  <Rules>
    <Rule ID=""INT-004"" Description=""Copy-Paste is code reuse, right?"" />
  </Rules>
  <Files>
    <File Name=""bar.cs"" Changed=""true"">
      <Rule ID=""INT-004"" Was=""2"" Is=""3"" />
    </File>
  </Files>
</RunChanges>", currentRun, previousRun);


            RunChanges runChanges = new RunChanges() { CurrentDateTime = currentRun, PreviousDateTime = previousRun };
            runChanges.Rules.Add(new RuleDescription { ID = "INT-004", Description = "Stale jokes replaced with fresh" });

            var bar = new FileChange() { Changed = true, Name = "bar.cs" };
            bar.Rules.Add(new RuleChange { ID = "INT-004", Was = 2, Is = 3 });

            runChanges.Files.Add(bar);

            var rules = new List<Rule>();
            rules.Add(
               new Rule { ID = "INT-004", Description = "Copy-Paste is code reuse, right?" }
                );

            _librarian.WriteRunChangesDoc(runChanges, rules);
            string actualText = _storage.RunChanges.ToString();

            Assert.That(actualText, Is.EqualTo(expectedText));
        }

    }
}
