//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
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
            adapter = starter.StudioAdapter;
            librarian.SolutionPath = @"c:\somewhere\for_the.sln";

            changeCat = starter.ChangeCatalog;
            string indentID = "14";
            changeCat.Add( new Change { ID = indentID, Description = "indentation cleanup", Language = FileLanguage.CSharp } );
            librarian._savedChangeCatalog = changeCat.Clone();

            fileCat = librarian._sourceCatalog;

            fileCat.SolutionPath = @"c:\somewhere\for_the.sln";
            fileName = "bari.cs";
            file = new SourceFile( fileName );
            fileCat.Add( file );

            MockStorageAdapter writer = new MockStorageAdapter();
            librarian._storageAdapter = writer;
            librarian._savedSourceCatalog = fileCat.Clone();
            librarian.SolutionPath = "mockpath";
            librarian.Persist();

            _taskWindow = adapter.taskWindow;
            _taskWindow._UserAdapter = new MockUserAdapter();

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
