//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using NUnit.Framework;

namespace swept.Tests
{
    [TestFixture]
    public class TaskTests
    {
        [Test]
        public void CanCreateFromChange()
        {
            Change change = new Change( "id1", "this change", FileLanguage.CSharp );
            Task entry = Task.FromChange( change );

            Assert.AreEqual( change.ID, entry.ID );
            Assert.AreEqual( change.Description, entry.Description );
            Assert.AreEqual( change.Language, entry.Language );
            Assert.IsFalse( entry.Completed );
        }
    }
}
