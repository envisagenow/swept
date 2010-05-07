//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using NUnit.Framework;

namespace swept.Tests
{
    [TestFixture]
    public class IntegrationTests
    {
        private Starter _starter;
        private StudioAdapter _adapter;
        private ChangeWindow _changes;
        private ProjectLibrarian _librarian;
        private TaskWindow _tasks;

        [SetUp]
        public void StartUp()
        {
            _starter = new Starter();
            _starter.Start();

            _librarian = _starter.Librarian;
            _librarian.SolutionPath = @"c:\code\path\to.sln";
            
            _tasks = _starter.TaskWindow;
            _adapter = _starter.StudioAdapter;
            _changes = _starter.ChangeWindow;

            TestPreparer preparer = new TestPreparer();
            preparer.ShiftSweptToMocks( _starter );
        }

        [Test]
        public void When_SolutionOpened_all_source_catalog_references_updated()
        {
            SourceFileCatalog oldCatalog = _librarian._sourceCatalog;
            _adapter.Raise_SolutionOpened( @"c:\different\place\for.sln" );
            SourceFileCatalog newCatalog = _librarian._sourceCatalog;
            Assert.That( newCatalog, Is.Not.EqualTo( oldCatalog ) );
            Assert.That( newCatalog, Is.EqualTo( _tasks._fileCatalog ) );
        }

        [Test]
        public void When_SolutionOpened_all_change_catalog_references_updated()
        {
            ChangeCatalog oldCatalog = _starter.ChangeCatalog;
            _adapter.Raise_SolutionOpened( @"c:\different\place\for.sln" );
            ChangeCatalog newCatalog = _starter.ChangeCatalog;
            Assert.That( newCatalog, Is.Not.SameAs( oldCatalog ) );
            Assert.That( newCatalog, Is.EqualTo( _tasks._changeCatalog ) );
        }

    }
}
