using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using System.IO;

namespace swept.Tests
{
    [TestFixture]
    public class ReadRunChanges_tests
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
            Assert.That(result.DateTime, Is.EqualTo(DateTime.MinValue));
            Assert.That(result.RunNumber, Is.EqualTo(0));
            Assert.That(result.Files.Count, Is.EqualTo(0));
        }


        [Test]
        public void Minimal_document_produces_Proper_RunChanges()
        {
            // as we go on, we maintain what 'empty' means for a RunDetails object.

            var changesDoc = XDocument.Parse(
@"<RunChanges RunNumber=""22"" DateTime=""4/4/2012 10:25:02 AM"">
  <Rules />
  <Files />
</RunChanges>
");

            RunChanges result = _librarian.ReadRunChanges(changesDoc);

            Assert.That(result.RunNumber, Is.EqualTo(22));
            Assert.That(result.DateTime, Is.EqualTo(DateTime.Parse("4/4/2012 10:25:02 AM")));
            Assert.That(result.Files.Count, Is.EqualTo(0));
        }

        [Test]
        public void Minimal_file_element_produces_Proper_FileChange()
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
    }
}
