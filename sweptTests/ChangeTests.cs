//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using NUnit.Framework;
using swept;
using System;

namespace swept.Tests
{
    [TestFixture]
    public class ChangeTests
    {
        [Test]
        public void CanCreateChange()
        {
            const string newID = "NEW";
            const string newDescription = "Brand new";
            const FileLanguage newLanguage = FileLanguage.CSharp;

            Change chg = new Change( newID, newDescription, newLanguage );

            Assert.AreEqual( newID, chg.ID );
            Assert.AreEqual( newDescription, chg.Description );
            Assert.AreEqual( newLanguage, chg.Language );
        }

        [Test]
        public void Can_compare_equality()
        {
            Change change1 = new Change();
            Change change2 = new Change();

            Assert.IsTrue(change1.Equals( change2 ));
        }

        [Test]
        public void Can_compare_inequal_objects()
        {
            Change change1 = new Change( "101-443", "Remove all dinguses", FileLanguage.CSharp );
            Change change2 = new Change( "5987515", "Frob all wobbishes", FileLanguage.CSS );

            Assert.IsFalse( change1.Equals( change2 ) );

            change1.ID = change2.ID;
            Assert.IsFalse( change1.Equals( change2 ) );

            change1.Description = change2.Description;
            Assert.IsFalse( change1.Equals( change2 ) );

            change1.Language = change2.Language;
            Assert.IsTrue( change1.Equals( change2 ) );
        }

        [Test]
        public void Can_compare_to_null()
        {
            Change change1 = new Change();
            Assert.IsFalse( change1.Equals( null ) );
        }
    }
}
