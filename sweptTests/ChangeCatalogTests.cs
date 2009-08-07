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

        // TODO: Add tests for Remove Change from Catalog

        #region Equals tests
        [Test]
        public void Empty_Catalogs_are_Equal()
        {
            ChangeCatalog cat1 = new ChangeCatalog();
            ChangeCatalog cat2 = new ChangeCatalog();

            Assert.IsTrue( cat1.Equals( cat2 ) );
        }

        [Test]
        public void Different_sized_Catalogs_are_Inequal()
        {
            ChangeCatalog cat1 = new ChangeCatalog();
            ChangeCatalog cat2 = new ChangeCatalog();

            Change change = new Change( "testId", "Test CHange", FileLanguage.CSharp );
            cat2.Add( change );


            Assert.IsFalse( cat1.Equals( cat2 ) );
        }

        [Test]
        public void Different_content_Catalogs_are_Inequal()
        {
            ChangeCatalog cat1 = new ChangeCatalog();
            ChangeCatalog cat2 = new ChangeCatalog();

            Change change = new Change( "SomeID", "A really groovy change", FileLanguage.CSharp );
            cat1.Add( change );

            change = new Change( "testId", "Test CHange", FileLanguage.CSharp );
            cat2.Add( change );

            Assert.IsFalse( cat1.Equals( cat2 ) );
        }

        [Test]
        public void Identical_content_Catalogs_are_Equal()
        {
            ChangeCatalog cat1 = new ChangeCatalog();
            ChangeCatalog cat2 = new ChangeCatalog();

            Change change = new Change( "SomeID", "A really groovy change", FileLanguage.CSharp );
            cat1.Add( change );
            cat2.Add( change );

            change = new Change( "testId", "Test Change", FileLanguage.CSharp );
            cat1.Add( change );
            cat2.Add( change );

            Assert.IsTrue( cat1.Equals( cat2 ) );
        }
        #endregion

        [Test]
        public void Changes_sort_alphabetically_by_ID()
        {
            Change a_17 = new Change( "a_17", "Do this thing", FileLanguage.CSharp );
            Change a_18 = new Change( "a_18", "Do this thing", FileLanguage.CSharp );
            Change a_117 = new Change( "a_117", "Do this thing", FileLanguage.CSharp );
            Change a_177 = new Change( "a_177", "Do this thing", FileLanguage.CSharp );
            Change b_52 = new Change( "b_52", "Do this, too", FileLanguage.CSharp );

            ChangeCatalog cat = new ChangeCatalog();

            cat.Add( b_52 );
            cat.Add( a_17 );
            cat.Add( a_177 );
            cat.Add( a_117 );
            cat.Add( a_18 );

            Assert.AreEqual( 0, cat.changes.IndexOfKey( a_117.ID ) );
            Assert.AreEqual( 1, cat.changes.IndexOfKey( a_17.ID ) );
            Assert.AreEqual( 2, cat.changes.IndexOfKey( a_177.ID ) );
            Assert.AreEqual( 3, cat.changes.IndexOfKey( a_18.ID ) );
            Assert.AreEqual( 4, cat.changes.IndexOfKey( b_52.ID ) );
        }


        [Test, ExpectedException( ExpectedMessage = "An entry with the same key already exists.")]
        public void Duplicate_IDs_not_allowed()
        {
            ChangeCatalog cat = new ChangeCatalog();

            Change a_17 = new Change( "a_17", "Do this thing", FileLanguage.CSharp );
            Change a_17a = new Change( "a_17", "Do this thing", FileLanguage.CSharp );

            cat.Add( a_17 );
            cat.Add( a_17a );
        }

        [Test]
        public void Clone_works_on_empty_Catalog()
        {
            ChangeCatalog cat = new ChangeCatalog();

            ChangeCatalog catClone = cat.Clone();
            Assert.AreNotSame( cat, catClone );
            Assert.IsEmpty( cat.changes );
        }

        [Test]
        public void Clone_works_on_populated_Catalog()
        {
            ChangeCatalog catClone = cat.Clone();
            Assert.AreNotSame( cat, catClone );
            Assert.AreEqual( 3, catClone.changes.Count );
        }
    }
}
