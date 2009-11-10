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
        // TODO: figure out how to keep ChangeTests up to date as attributes are added...
        [Test]
        public void CanCreateChange()
        {
            const string newID = "NEW";
            const string newDescription = "Brand new";

            const string path = @"somewhere";
            const string namePattern = "problematic.code();";
            const FileLanguage newLanguage = FileLanguage.CSharp;

            const string problematicContent = "problematic.code();";

            Change change = new Change { 
                ID = newID, Description = newDescription, 
                Language = newLanguage, Subpath = path, NamePattern = namePattern,
                ContentPattern = problematicContent
            };

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

        // TODO: relocate, expand attributes being compared
        [Test]
        public void Can_compare_inequal_objects()
        {
            Change change1 = new Change { ID = "101-443", Description = "Remove all dinguses", Language = FileLanguage.CSharp };
            Change change2 = new Change { ID = "5987515", Description = "Frob all wobbishes", Language = FileLanguage.CSS };

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
