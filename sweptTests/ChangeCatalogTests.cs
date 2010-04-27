//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using NUnit.Framework;
using swept;
using System.Collections.Generic;
using System;

namespace swept.Tests
{
    [TestFixture]
    public class ChangeCatalogTests
    {
        private ChangeCatalog cat;
        [SetUp]
        public void given_a_catalog_with_several_changes()
        {
            cat = new ChangeCatalog();

            Change avoidAliasUsing = new Change { ID = "e1", Description = "Don't use 'using' to alias.", Language = FileLanguage.CSharp };
            // TODO--0.3: figure out contentPattern for avoidAliasUsing.
            cat.Add( avoidAliasUsing );

            cat.Add( new Change { ID = "e2", Description = "Upgrade to XHTML", Language = FileLanguage.HTML, ManualCompletion = true } );
            cat.Add( new Change { ID = "e3", Description = "Put <title> on all pages", Language = FileLanguage.HTML } );
        }

        //[Test]
        //public void GetChanges_returns_all_matching_Changes()
        //{
        //    List<Change> changes = cat.GetChangesForFile(new SourceFile("index.html"));
        //    List<Change> moreChanges = cat.FindAll(ch => ch.Language == FileLanguage.HTML);
        //    Assert.IsTrue(changes.TrueForAll(ch => moreChanges.Contains(ch)));
        //    Assert.AreEqual(moreChanges.Count, changes.Count);
        //}

        [Test]
        public void GetChanges_returns_only_pertinent_Changes()
        {
            List<Change> changes = cat.GetChangesForFile( new SourceFile( "hello_world.cs" ) );
            Assert.AreEqual( 1, changes.Count );
            Assert.AreEqual( "e1", changes[0].ID );
        }

        [Test]
        public void GetChanges_returns_no_Changes_when_language_filters_enough()
        {
            List<Change> changes = cat.GetChangesForFile( new SourceFile( "hello_style.css" ) );
            Assert.AreEqual( 0, changes.Count );
        }

        [Test]
        public void GetChanges_returns_Appropriate_CompoundChanges()
        {
            CompoundFilter filter = new CompoundFilter();

            List<Change> changes = cat.GetChangesForFile( new SourceFile( "hello_style.css" ) );
            Assert.AreEqual( 0, changes.Count );
        }

        [Test]
        public void Empty_Catalog_returns_empty_list()
        {
            List<Change> changes = cat.GetChangesForFile(new SourceFile("hello_style.css"));
            Assert.AreEqual(0, changes.Count);
        }

        // TODO--0.3, DC: Test removing Changes from Catalog

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

            Change change = new Change { ID = "testId" };
            cat2.Add( change );


            Assert.IsFalse( cat1.Equals( cat2 ) );
        }

        [Test]
        public void Catalogs_are_Inequal_with_different_IDs()
        {
            ChangeCatalog cat1 = new ChangeCatalog();
            ChangeCatalog cat2 = new ChangeCatalog();

            Change change = new Change { ID = "SomeID", Description = "A really groovy change"  };
            cat1.Add(change);

            change = new Change { ID = "testId", Description = "Test Change" };
            cat2.Add(change);

            Assert.IsFalse(cat1.Equals(cat2));
        }

        [Test]
        public void Catalogs_are_Inequal_with_same_IDs_different_content()
        {
            ChangeCatalog cat1 = new ChangeCatalog();
            ChangeCatalog cat2 = new ChangeCatalog();

            Change change = new Change { ID = "SomeID", Description = "A really groovy change" };
            cat1.Add( change );

            change = new Change { ID = "SomeID", Description = "Test Change" };
            cat2.Add(change);

            Assert.IsFalse(cat1.Equals(cat2));
        }

        [Test]
        public void Identical_content_Catalogs_are_Equal()
        {
            ChangeCatalog cat1 = new ChangeCatalog();
            ChangeCatalog cat2 = new ChangeCatalog();

            Change change = new Change { ID = "SomeID", Description = "A really groovy change", Language = FileLanguage.CSharp };
            cat1.Add( change );
            cat2.Add( change );

            change = new Change { ID = "testId", Description = "Test Change", Language = FileLanguage.CSharp };
            cat1.Add( change );
            cat2.Add( change );

            Assert.IsTrue( cat1.Equals( cat2 ) );
        }
        #endregion

        [Test]
        public void Changes_sort_alphabetically_by_ID()
        {
            Change a_17 = new Change{ID= "a_17", };
            Change a_18 = new Change{ID= "a_18", };
            Change a_117 = new Change{ID= "a_117", };
            Change a_177 = new Change{ID= "a_177", };
            Change b_52 = new Change { ID = "b_52", };

            ChangeCatalog cat = new ChangeCatalog();

            cat.Add( b_52 );
            cat.Add( a_17 );
            cat.Add( a_177 );
            cat.Add( a_117 );
            cat.Add( a_18 );

            var changes = cat.GetSortedChanges();
            Assert.That( changes[0].ID, Is.EqualTo( a_117.ID ) );
            Assert.That( changes[1].ID, Is.EqualTo( a_17.ID ) );
            Assert.That( changes[2].ID, Is.EqualTo( a_177.ID ) );
            Assert.That( changes[3].ID, Is.EqualTo( a_18.ID ) );
            Assert.That( changes[4].ID, Is.EqualTo( b_52.ID ) );
        }


        [Test, ExpectedException( ExpectedMessage = "There is already a change with the ID [a_17]." )]
        public void Duplicate_IDs_not_allowed()
        {
            ChangeCatalog cat = new ChangeCatalog();

            Change a_17a = new Change { ID = "a_17" };
            Change a_17b = new Change { ID = "a_17" };

            cat.Add( a_17a );
            cat.Add( a_17b );
        }

        [Test]
        public void Clone_works_on_empty_Catalog()
        {
            ChangeCatalog cat = new ChangeCatalog();

            ChangeCatalog catClone = cat.Clone();
            Assert.AreNotSame( cat, catClone );
            Assert.IsEmpty( cat._changes );
        }

        [Test]
        public void Clone_works_on_populated_Catalog()
        {
            ChangeCatalog catClone = cat.Clone();
            Assert.AreNotSame( cat, catClone );
            Assert.AreEqual( 3, catClone._changes.Count );
        }

        [Test]
        public void Clone_brings_across_CompoundFilters()
        {
            Change change = new Change {
                ID = "This",
                Children = new List<CompoundFilter> {
                    new CompoundFilter {
                        ID = "Child",
                        Subpath = "hidden"
                    }
                }
            };
            cat.Add( change );
            ChangeCatalog catClone = cat.Clone();
            Assert.AreNotSame( cat, catClone );
            Assert.AreEqual( 4, catClone._changes.Count );
            var hiddenChild = catClone._changes[3].Children[0];
            Assert.That( hiddenChild.ID, Is.EqualTo( "Child" ) );
            Assert.That( hiddenChild.Subpath, Is.EqualTo( "hidden" ) );
        }
    }
}
