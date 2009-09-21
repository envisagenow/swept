//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Xml;

namespace swept.Tests
{
    [TestFixture]
    public class ChangeWindowEventTests
    {
        Starter starter;
        private TaskWindow taskWindow;
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

            changeCat = librarian.changeCatalog;
            string indentID = "14";
            changeCat.Add(new Change(indentID, "indentation cleanup", FileLanguage.CSharp));
            librarian.savedChangeCatalog = changeCat.Clone();

            fileCat = librarian.sourceCatalog;

            fileName = "bari.cs";
            file = new SourceFile(fileName);
            file.Completions.Add(new Completion(indentID));
            fileCat.Files.Add(file);

            MockLibraryPersister writer = new MockLibraryPersister();
            librarian.persister = writer;
            librarian.savedSourceCatalog = fileCat.Clone();
            librarian.SolutionPath = "mockpath";
            librarian.Persist();

            taskWindow = adapter.taskWindow;
            changeWindow = adapter.changeWindow;
        }

        [Test]
        public void WhenChangeListUpdated_TaskWindow_RefreshesTasks()
        {
            adapter.Raise_FileGotFocus("foo.cs");
            int initialChangeCount = taskWindow.Tasks.Count;
            changeCat.Add(new Change("Inf09", "Change delegates to lambdas", FileLanguage.CSharp));

            changeWindow.RaiseChangeListUpdated();

            Assert.AreEqual(initialChangeCount + 1, taskWindow.Tasks.Count);
        }

        [Test]
        public void WhenChangeListUpdated_EmptyTaskWindow_RefreshesItsEmptiness()
        {
            adapter.Raise_NonSourceGetsFocus();
            changeCat.Add(new Change("Inf09", "Change delegates to lambdas", FileLanguage.CSharp));

            changeWindow.RaiseChangeListUpdated();

            Assert.AreEqual(0, taskWindow.Tasks.Count);
        }

        [Test]
        public void WhenChangeAdded_ChangeCatalogPersisted()
        {
            Assert.IsTrue( librarian.IsSaved );
            changeCat.Add(new Change("Inf09", "Change delegates to lambdas", FileLanguage.CSharp));
            Assert.IsFalse(librarian.IsSaved);
            changeWindow.RaiseChangeListUpdated();
            Assert.IsTrue(librarian.IsSaved);
        }

        [Test]
        public void WhenChangeAdded_IfChangeExistedPreviously_UserCanKeepHistory()
        {
            changeCat.Remove("14");

            MockDialogPresenter mockGUI = new MockDialogPresenter();
            adapter.Librarian.showGUI = mockGUI;
            //  When the dialog is presented, the 'user' responds 'keep', for this test
            mockGUI.KeepHistoricalResponse = true;

            // Add Change 14 back
            Change change = new Change("14", "indentation cleanup", FileLanguage.CSharp);
            changeWindow.RaiseChangeAdded( change );

            // Bari has kept the completion of Change 14
            SourceFile savedBari = librarian.savedSourceCatalog.Fetch("bari.cs");
            Assert.AreEqual(1, savedBari.Completions.Count);
            Assert.AreEqual("14", savedBari.Completions[0].ChangeID);
        }

        [Test]
        public void WhenChangeAdded_IfChangeExistedPreviously_UserCanRemoveHistory()
        {
            changeCat.Remove("14");

            MockDialogPresenter mockGUI = new MockDialogPresenter();
            adapter.Librarian.showGUI = mockGUI;
            //  When the dialog is presented, the 'user' responds 'remove', for this test
            mockGUI.KeepHistoricalResponse = false;

            // Add Change 14 back
            Change change = new Change("14", "indentation cleanup", FileLanguage.CSharp);
            changeWindow.RaiseChangeAdded(change);

            adapter.Raise_FileSaved("bari.cs");

            // Bari has removed the completion of Change 14
            Assert.IsFalse(TestProbe.IsCompletionSaved(librarian, "bari.cs", "14"));
        }

        [Test]
        public void WhenChangeRemoved_TaskWindow_RefreshesTasks()
        {
            adapter.Raise_FileGotFocus("foo.cs");
            int initialChangeCount = taskWindow.Tasks.Count;
            changeCat.Remove("14");

            changeWindow.RaiseChangeListUpdated();

            Assert.AreEqual(initialChangeCount - 1, taskWindow.Tasks.Count);
        }
    }
}
