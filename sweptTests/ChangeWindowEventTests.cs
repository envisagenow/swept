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
        private TaskWindow window;
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

            fileCat = librarian.unsavedSourceImage;

            fileName = "bari.cs";
            file = new SourceFile(fileName);
            file.Completions.Add(new Completion(indentID));
            fileCat.Files.Add(file);

            MockLibraryWriter writer = new MockLibraryWriter();
            librarian.persister = writer;
            librarian.savedSourceImage = SourceFileCatalog.Clone(fileCat);
            librarian.SolutionPath = "mockpath";
            librarian.Persist();

            window = adapter.taskWindow;
        }

        [Test]
        public void WhenChangeListUpdated_TaskWindow_RefreshesTasks()
        {
            adapter.RaiseFileGotFocus("foo.cs");
            int initialChangeCount = window.Tasks.Count;
            changeCat.Add(new Change("Inf09", "Change delegates to lambdas", FileLanguage.CSharp));

            adapter.WhenChangeListUpdated();

            Assert.AreEqual(initialChangeCount + 1, window.Tasks.Count);
        }

        [Test]
        public void WhenChangeListUpdated_EmptyTaskWindow_RefreshesItsEmptiness()
        {
            adapter.RaiseNonSourceGetsFocus();
            changeCat.Add(new Change("Inf09", "Change delegates to lambdas", FileLanguage.CSharp));
            adapter.WhenChangeListUpdated();

            Assert.AreEqual(0, window.Tasks.Count);
        }

        [Test]
        public void WhenChangeAdded_ChangeCatalogPersisted()
        {
            Assert.IsFalse(librarian.ChangeNeedsPersisting);
            changeCat.Add(new Change("Inf09", "Change delegates to lambdas", FileLanguage.CSharp));
            Assert.IsTrue(librarian.ChangeNeedsPersisting);
            adapter.WhenChangeListUpdated();
            Assert.IsFalse(librarian.ChangeNeedsPersisting);
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
            adapter.RaiseChangeAdded( change );

            // Bari SHOULD have 14 completed already
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(librarian.savedSourceImage.ToXmlText());
            Assert.IsTrue(IsCompletionSaved(doc, "bari.cs", "14"));
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
            adapter.RaiseChangeAdded(change);

            adapter.RaiseFileSaved("bari.cs");

            // Bari SHOULD NOT have 14 completed already
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(librarian.savedSourceImage.ToXmlText());
            Assert.IsFalse(IsCompletionSaved(doc, "bari.cs", "14"));
        }

        private static bool IsCompletionSaved(XmlDocument doc, string fileName, string id)
        {
            string completionXPath = String.Format("//SourceFile[@Name='{0}']/Completion[@ID='{1}']", fileName, id);
            return doc.SelectSingleNode(completionXPath) != null;
        }


        [Test]
        public void WhenChangeRemoved_TaskWindow_RefreshesTasks()
        {
            adapter.RaiseFileGotFocus("foo.cs");
            int initialChangeCount = window.Tasks.Count;
            changeCat.Remove("14");

            adapter.WhenChangeListUpdated();

            Assert.AreEqual(initialChangeCount - 1, window.Tasks.Count);
        }
    }
}
