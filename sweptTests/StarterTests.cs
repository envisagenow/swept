//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using NUnit.Framework;

namespace swept.Tests
{
    [CoverageExclude]
    [TestFixture]
    public class StarterTests
    {
        Starter starter;

        [SetUp]
        public void CanCreateStarter()
        {
            starter = new Starter();
            starter.Start( new EventSwitchboard() );
        }

        [Test]
        public void StudioAdapter_is_connected()
        {
            EventSwitchboard studio = starter.Switchboard;

            Assert.IsNotNull(studio.Librarian);
        }

        [Test]
        public void Librarian_is_connected()
        {
            ProjectLibrarian lib = starter.Librarian;
            Assert.IsNotNull(lib);
            Assert.IsNotNull( lib._changeCatalog );
        }

        [Test]
        public void Stop_clears_major_entities()
        {
            starter.Stop();

            Assert.IsNull( starter.Switchboard );
            Assert.IsNull( starter.Librarian );
        }
        
        // TODO--0.2: load the appropriate library if Swept started with a solution already loaded
    }
}
