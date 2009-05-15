//  Swept:  Software Enhancement Progress Tracking.  Copyright 2008 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
using NUnit.Framework;
using swept;
using System.Collections.Generic;
using System.Text;
using System;

namespace swept.Tests
{
    //appears to be our bad seed, here.
    //[TestFixture]
    public class FileChangesViewTests
    {
        private FileChangesView view;
        [SetUp]
        public void CanCreate()
        {
            view = new FileChangesView();
        }

        [Test]
        public void WillShowChanges()
        {
            view.File = new SourceFile( "foo.html" );

            ChangeCatalog chacat = new ChangeCatalog();
            chacat.Add( new Change( "e1", "Clean up <BR>s", FileLanguage.HTML ) );
            view.Catalog = chacat;

            List<Change> changes = view.Changes;
            Assert.AreEqual( 1, changes.Count );
        }


        //[Test]
        //public void EmptyListsResultInNoEnhancementsPending()
        //{
        //    List<Enhancement> pending = file.EnhancementsPending;
        //    Assert.AreEqual( 0, pending.Count );
        //}

        //[Test]
        //public void NoCompletionsResultInAllEnhancementsPending()
        //{
        //    List<Enhancement> pending = file.EnhancementsPending;

        //    Assert.AreEqual( 2, pending.Count );
        //    Assert.AreEqual( "ab1", pending[0].ID );
        //    Assert.AreEqual( "ab2", pending[1].ID );
        //}

        //[Test]
        //public void CompletionsMarkEnhancementsComplete()
        //{
        //    file.Completions.Add( new Completion( "ab2" ) );

        //    List<Enhancement> pending = file.EnhancementsPending;

        //    Assert.AreEqual( 1, pending.Count );
        //    Assert.AreEqual( "ab1", pending[0].ID );
        //}

        //[Test]
        //public void CanMarkEnhancementCompleted()
        //{
        //    file.MarkCompleted( "ab1" );
        //    Enhancement ab1 = file.Enhancements[0];
        //    Assert.IsTrue( ab1.Completed );

        //    // also check completions...
        //}

    }
}
