//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using NUnit.Framework;
//using NUnit.Framework.SyntaxHelpers;

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
            _adapter = _starter.Adapter;
            _changes = _starter.ChangeWindow;

            TestPreparer preparer = new TestPreparer();
            preparer.ShiftStarterToMocks( _starter );
        }

        [Test]
        public void when_Task_Completion_changed_Library_has_an_unsaved_change()
        {
            _changes.Raise_ChangeAdded( new Change { ID = "100", Description = "test change", Language = FileLanguage.CSharp } );
            _adapter.Raise_FileGotFocus( "foo.cs", "using System;" );
            _adapter.Raise_SolutionSaved();

            Assert.IsTrue( _librarian.IsSaved );

            _tasks.ToggleTaskCompletion( 0 );

            Assert.IsFalse( _librarian.IsSaved );
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
            ChangeCatalog oldCatalog = _librarian._changeCatalog;
            _adapter.Raise_SolutionOpened( @"c:\different\place\for.sln" );
            ChangeCatalog newCatalog = _librarian._changeCatalog;
            Assert.That( newCatalog, Is.Not.EqualTo( oldCatalog ) );
            Assert.That( newCatalog, Is.EqualTo( _tasks._changeCatalog ) );
        }

    }
}
