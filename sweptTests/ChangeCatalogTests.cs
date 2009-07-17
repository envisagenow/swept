//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using NUnit.Framework;
using NUnit.Framework.Constraints;
using swept;
using System.Collections.Generic;
using System.Text;
using System;

namespace swept.Tests
{
    [TestFixture]
    public class ChangeCatalogTests
    {
        private ChangeCatalog cat;
        [SetUp]
        public void CanCreate()
        {
            cat = new ChangeCatalog();

            cat.Add(new Change("e1", "Fix 'using' statements", FileLanguage.CSharp));
            cat.Add(new Change("e2", "Upgrade to XHTML", FileLanguage.HTML));
            cat.Add(new Change("e3", "Put <title> on all pages", FileLanguage.HTML));

        }

        [Test]
        public void GetChanges_returns_all_pertinent_Changes()
        {
            List<Change> changes = cat.GetChangesForFile(new SourceFile("index.html"));
            List<Change> moreChanges = cat.FindAll(ch => ch.Language == FileLanguage.HTML);
            Assert.IsTrue(changes.TrueForAll(ch => moreChanges.Contains(ch)));
            Assert.AreEqual(moreChanges.Count, changes.Count);
        }

        [Test]
        public void GetChanges_returns_only_pertinent_Changes()
        {
            List<Change> changes = cat.GetChangesForFile(new SourceFile("hello_world.cs"));
            Assert.AreEqual(1, changes.Count);
            Assert.AreEqual("e1", changes[0].ID);
        }

        [Test]
        public void Empty_Catalog_returns_empty_list()
        {
            List<Change> changes = cat.GetChangesForFile(new SourceFile("hello_style.css"));
            Assert.AreEqual(0, changes.Count);
        }

        // TODO: Remove, MarkClean

        // TODO: Equals
    }
}
