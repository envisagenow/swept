//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using NUnit.Framework;

namespace swept.Tests
{
    [TestFixture]
    public class IntegrationTests
    {
        private Subscriber _starter;
        private EventSwitchboard _switchboard;
        private ProjectLibrarian _librarian;

        [SetUp]
        public void StartUp()
        {
            _switchboard = new EventSwitchboard();
            _librarian = new ProjectLibrarian( new MockStorageAdapter(), _switchboard );
            _librarian.SolutionPath = @"c:\code\path\to.sln";

            _starter = new Subscriber();
            _starter.Subscribe( _switchboard, _librarian );
        }

        [Test]
        public void When_SolutionOpened_all_RuleCatalog_references_updated()
        {
            RuleCatalog oldCatalog = _librarian._ruleCatalog;

            _switchboard.Raise_SolutionOpened( @"c:\different\place\for.sln" );
            
            RuleCatalog newCatalog = _librarian._ruleCatalog;
            Assert.That( newCatalog, Is.Not.SameAs( oldCatalog ) );
        }

    }
}
