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
        public void StudioAdapter_is_connected()
        {
            StudioAdapter studio = starter.Adapter;

            Assert.IsNotNull(studio.taskWindow);
            Assert.IsNotNull(studio.Librarian);
        }

        [Test]
        public void Librarian_is_connected()
        {
            ProjectLibrarian lib = starter.Librarian;
            Assert.IsNotNull(lib);
            Assert.IsNotNull(lib._changeCatalog);
        }

        [Test]
        public void Stop_clears_major_entities()
        {
            starter.Stop();

            Assert.IsNull( starter.Adapter );
            Assert.IsNull( starter.Librarian );
            Assert.IsNull( starter.TaskWindow );
            Assert.IsNull( starter.ChangeWindow );
        }
    }
}
