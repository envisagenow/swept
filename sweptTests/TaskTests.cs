using System;
using System.Collections.Generic;
using System.Text;
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
