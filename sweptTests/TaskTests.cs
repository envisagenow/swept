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
            Change change = new Change{ ID = "id1", Description = "this change", Language = FileLanguage.CSharp };
            Task entry = Task.FromChange( change );

            Assert.AreEqual( change.ID, entry.ID );
            Assert.AreEqual( change.Description, entry.Description );
            Assert.IsFalse( entry.Completed );
        }
    }
}
