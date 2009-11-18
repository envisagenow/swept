//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using NUnit.Framework;

namespace swept.Tests
{
    [TestFixture]
    public class ChangeWindowEventTests
    {
        Starter starter;
        private TaskWindow _taskWindow;
        private ChangeWindow changeWindow;
        private string fileName;
        private SourceFile file;
        private ChangeCatalog changeCat;
        private SourceFileCatalog fileCat;

        private StudioAdapter adapter;
        private ProjectLibrarian librarian;

        [SetUp]
        public void SetUp()
        {
            starter = new Starter();
            starter.Start();

            librarian = starter.Librarian;
            adapter = starter.Adapter;
            librarian.SolutionPath = @"c:\somewhere\for_the.sln";

            changeCat = librarian._changeCatalog;
            string indentID = "14";
            changeCat.Add( new Change { ID = indentID, Description = "indentation cleanup", Language = FileLanguage.CSharp } );
            librarian._savedChangeCatalog = changeCat.Clone();

            fileCat = librarian._sourceCatalog;

            fileCat.SolutionPath = @"c:\somewhere\for_the.sln";
            fileName = "bari.cs";
            file = new SourceFile( fileName );
            file.Completions.Add( new Completion( indentID ) );
            fileCat.Add( file );

            MockStorageAdapter writer = new MockStorageAdapter();
            librarian._storageAdapter = writer;
            librarian._savedSourceCatalog = fileCat.Clone();
            librarian.SolutionPath = "mockpath";
            librarian.Persist();

            _taskWindow = adapter.taskWindow;
            _taskWindow._GUIAdapter = new MockUserAdapter();

            changeWindow = adapter.changeWindow;
        }

        [Test]
        public void WhenChangeListUpdated_TaskWindow_RefreshesTasks()
        {
            adapter.Raise_FileGotFocus( "foo.cs", "using System;" );
            int initialChangeCount = _taskWindow.Tasks.Count;
            changeCat.Add( new Change { ID = "Inf09", Language = FileLanguage.CSharp } );

            changeWindow.Raise_ChangeListUpdated();

            Assert.AreEqual( initialChangeCount + 1, _taskWindow.Tasks.Count );
        }

        [Test]
        public void WhenChangeListUpdated_EmptyTaskWindow_RefreshesItsEmptiness()
        {
            adapter.Raise_NonSourceGetsFocus();
            changeCat.Add( new Change { ID = "Inf09", Description = "Change delegates to lambdas", Language = FileLanguage.CSharp } );

            changeWindow.Raise_ChangeListUpdated();

            Assert.AreEqual( 0, _taskWindow.Tasks.Count );
        }

        [Test]
        public void WhenChangeAdded_ChangeCatalogPersisted()
        {
            changeCat = librarian._changeCatalog;
            Assert.IsTrue( librarian.IsSaved );
            changeCat.Add( new Change { ID = "Inf09_altered" } );
            Assert.IsFalse( librarian.IsSaved );
            changeWindow.Raise_ChangeListUpdated();
            Assert.IsTrue( librarian.IsSaved );
        }

        [Test]
        public void WhenChangeAdded_IfChangeExistedPreviously_UserCanKeepHistory()
        {
            changeCat.Remove( "14" );

            MockUserAdapter mockGUI = new MockUserAdapter();
            adapter.Librarian._userAdapter = mockGUI;
            //  When the dialog is presented, the 'user' responds 'keep', for this test
            mockGUI.KeepHistoricalResponse = true;

            // Add Change 14 back
            Change change = new Change { ID = "14" };
            changeWindow.Raise_ChangeAdded( change );

            // Bari has kept the completion of Change 14
            SourceFile savedBari = librarian._savedSourceCatalog.Fetch( "bari.cs" );
            Assert.AreEqual( 1, savedBari.Completions.Count );
            Assert.AreEqual( "14", savedBari.Completions[0].ChangeID );
        }

        [Test]
        public void WhenChangeAdded_IfChangeExistedPreviously_UserCanRemoveHistory()
        {
            changeCat.Remove( "14" );

            MockUserAdapter mockGUI = new MockUserAdapter();
            adapter.Librarian._userAdapter = mockGUI;
            //  When the dialog is presented, the 'user' responds 'remove', for this test
            mockGUI.KeepHistoricalResponse = false;

            // Add Change 14 back
            Change change = new Change { ID = "14" };
            changeWindow.Raise_ChangeAdded( change );

            adapter.Raise_FileSaved( "bari.cs" );

            // Bari has removed the completion of Change 14
            Assert.IsFalse( TestProbe.IsCompletionSaved( librarian, "bari.cs", "14" ) );
        }

        [Test]
        public void WhenChangeRemoved_TaskWindow_RefreshesTasks()
        {
            adapter.Raise_FileGotFocus( "foo.cs", "using System;" );
            int initialChangeCount = _taskWindow.Tasks.Count;
            changeCat.Remove( "14" );

            changeWindow.Raise_ChangeListUpdated();

            Assert.AreEqual( initialChangeCount - 1, _taskWindow.Tasks.Count );
        }
    }
}
