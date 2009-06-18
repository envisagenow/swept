using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace swept.Tests
{
    [TestFixture]
    public class StarterTests
    {
        Starter starter;

        [SetUp]
        public void CanCreateStarter()
        {
            starter = new Starter();
            starter.Start();
        }

        [Test]
        public void EventDispatcher_IsConnected()
        {
            StudioAdapter ed = starter.Adapter;

            Assert.IsNotNull(ed.taskWindow);
            Assert.IsNotNull(ed.Librarian);
        }

        [Test]
        public void Librarian_IsConnected()
        {
            ProjectLibrarian lib = starter.Librarian;
            Assert.IsNotNull(lib);
            Assert.IsNotNull(lib.changeCatalog);
        }
    }
}
