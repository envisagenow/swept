//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using NUnit.Framework;
using swept;
using System.Collections.Generic;
using System;
using swept.DSL;

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

            Change avoidAliasUsing = new Change { ID = "e1", Description = "Don't use 'using' to alias.", Subquery = new QueryLanguageNode { Language = FileLanguage.CSharp } };
            cat.Add( avoidAliasUsing );

            cat.Add( new Change { ID = "e2", Description = "Upgrade to XHTML", Subquery = new QueryLanguageNode { Language = FileLanguage.HTML }  } );
            cat.Add( new Change { ID = "e3", Description = "Put <title> on all pages", Subquery = new QueryLanguageNode { Language = FileLanguage.HTML } } );
        }

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
        public void Empty_Catalog_returns_empty_list()
        {
            List<Change> changes = cat.GetChangesForFile(new SourceFile("hello_style.css"));
            Assert.AreEqual(0, changes.Count);
        }

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

            Change change = new Change { ID = "SomeID", Description = "A really groovy change", Subquery = new QueryLanguageNode { Language = FileLanguage.CSharp } };
            cat1.Add( change );
            cat2.Add( change );

            change = new Change { ID = "testId", Description = "Test Change", Subquery = new QueryLanguageNode { Language = FileLanguage.CSharp } };
            cat1.Add( change );
            cat2.Add( change );

            Assert.IsTrue( cat1.Equals( cat2 ) );
        }
        #endregion

        [Test]
        public void Changes_sort_alphabetically_by_ID()
        {
            Change a_17 = new Change { ID = "a_17", };
            Change a_18 = new Change { ID = "a_18", };
            Change a_117 = new Change { ID = "a_117", };
            Change a_177 = new Change { ID = "a_177", };
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

        [Test]
        public void Duplicate_IDs_not_allowed()
        {
            ChangeCatalog cat = new ChangeCatalog();

            Change a_17a = new Change { ID = "a_17" };
            Change a_17b = new Change { ID = "a_17" };

            cat.Add( a_17a );
            var ex = Assert.Throws<Exception>( () => cat.Add( a_17b ) );
            Assert.That( ex.Message, Is.EqualTo( "There is already a change with the ID [a_17]." ));
        }

    }
}
