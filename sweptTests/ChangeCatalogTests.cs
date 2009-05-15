//  Swept:  Software Enhancement Progress Tracking.  Copyright 2008 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
using NUnit.Framework;
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
        }

        [Test]
        public void CanGetChangesFilteredByLanguage()
        {
            cat.Add( new Change( "e1", "Fix using statements", FileLanguage.CSharp ) );
            cat.Add( new Change( "e2", "Upgrade to XHTML", FileLanguage.HTML ) );
            cat.Add( new Change( "e3", "Put <title> on all pages", FileLanguage.HTML ) );

            List<Change> changes = cat.GetListForLanguage( FileLanguage.HTML );
            Assert.AreEqual( 2, changes.Count );
            Assert.AreEqual( "e2", changes[0].ID );
            Assert.AreEqual( "e3", changes[1].ID );

            changes = cat.GetListForLanguage( FileLanguage.CSharp );
            Assert.AreEqual( 1, changes.Count );
            Assert.AreEqual( "e1", changes[0].ID );
        }
    }
}
