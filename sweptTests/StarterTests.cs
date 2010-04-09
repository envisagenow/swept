//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
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
            StudioAdapter studio = starter.StudioAdapter;

            Assert.IsNotNull(studio.taskWindow);
            Assert.IsNotNull(studio.Librarian);
        }

        [Test]
        public void Librarian_is_connected()
        {
            ProjectLibrarian lib = starter.Librarian;
            Assert.IsNotNull(lib);
            Assert.IsNotNull( starter.ChangeCatalog );
        }

        [Test]
        public void Stop_clears_major_entities()
        {
            starter.Stop();

            Assert.IsNull( starter.StudioAdapter );
            Assert.IsNull( starter.Librarian );
            Assert.IsNull( starter.TaskWindow );
            Assert.IsNull( starter.ChangeWindow );
        }
        
        // TODO--0.2: load the appropriate library if Swept started with a solution already loaded
    }
}
