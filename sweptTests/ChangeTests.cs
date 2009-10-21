//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
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

            Change change = new Change( newID, newDescription, newLanguage );

            Assert.AreEqual( newID, change.ID );
            Assert.AreEqual( newDescription, change.Description );
            Assert.AreEqual( newLanguage, change.Language );
        }


        #region Equality
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
        #endregion
    }
}
