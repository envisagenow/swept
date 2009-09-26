//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
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
