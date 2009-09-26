//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using System;
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
