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
        private string fileNameBari;
        private SourceFile bari;
        private ChangeCatalog changeCat;
        private SourceFileCatalog fileCat;

        private StudioAdapter dispatcher;
        private ProjectLibrarian librarian;

        [SetUp]
        public void SetUp()
        {
            starter = new Starter();
            starter.Start();

            librarian = starter.Librarian;
            dispatcher = starter.Adapter;

            changeCat = librarian.changeCatalog;
            string indentID = "14";
            changeCat.Add(new Change(indentID, "indentation cleanup", FileLanguage.CSharp));

            fileCat = librarian.unsavedSourceImage;

            fileNameBari = "bari.cs";
            bari = new SourceFile(fileNameBari);
            bari.Completions.Add(new Completion(indentID));
            fileCat.Files.Add(bari);

            MockLibraryWriter writer = new MockLibraryWriter();
            librarian.persister = writer;
            librarian.savedSourceImage = SourceFileCatalog.Clone(fileCat);
            librarian.SolutionPath = "mockpath";
            librarian.Persist();

            window = dispatcher.taskWindow;
        }

        //  As of now, Add/Edit/Delete all kick off a WhenChangeListUpdated event.  Finer granularity may pay off later.

        [Test]
        public void WhenChangeListUpdated_TaskWindow_RefreshesTasks()
        {
            dispatcher.RaiseFileGotFocus("foo.cs");
            int initialChangeCount = window.Tasks.Count;
            changeCat.Add(new Change("Inf09", "Change delegates to lambdas", FileLanguage.CSharp));

            dispatcher.WhenChangeListUpdated();

            Assert.AreEqual(initialChangeCount + 1, window.Tasks.Count);
        }

        [Test]
        public void WhenChangeListUpdated_EmptyTaskWindow_RefreshesItsEmptiness()
        {
            dispatcher.RaiseNonSourceGetsFocus();
            changeCat.Add(new Change("Inf09", "Change delegates to lambdas", FileLanguage.CSharp));
            dispatcher.WhenChangeListUpdated();

            Assert.AreEqual(0, window.Tasks.Count);
        }

        [Test]
        public void WhenChangeAdded_ChangeCatalogPersisted()
        {
            Assert.IsFalse(librarian.ChangeNeedsPersisting);
            changeCat.Add(new Change("Inf09", "Change delegates to lambdas", FileLanguage.CSharp));
            Assert.IsTrue(librarian.ChangeNeedsPersisting);
            dispatcher.WhenChangeListUpdated();
            Assert.IsFalse(librarian.ChangeNeedsPersisting);
        }

        [Test]
        public void WhenChangeAdded_IfChangeExistedPreviously_UserCanKeepHistory()
        {
            // Remove Change 14
            changeCat.Remove("14");

            // User Requests to keep History
            MockDialogPresenter talker = new MockDialogPresenter();
            talker.KeepHistoricalResponse = true;
            dispatcher.Librarian.showGUI = talker;

            // Add Change 14 back
            Change change = new Change("14", "indentation cleanup", FileLanguage.CSharp);
            dispatcher.RaiseChangeAdded( change );

            // Bari SHOULD have 14 completed already
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(librarian.savedSourceImage.ToXmlText());
            Assert.IsTrue(IsCompletionSaved(doc, "bari.cs", "14"));
        }

        [Test]
        public void WhenChangeAdded_IfChangeExistedPreviously_UserCanRemoveHistory()
        {
            // Remove Change 14
            changeCat.Remove("14");

            // User Requests to remove History
            MockDialogPresenter talker = new MockDialogPresenter();
            talker.KeepHistoricalResponse = false;
            dispatcher.Librarian.showGUI = talker;

            // Add Change 14 back
            Change change = new Change("14", "indentation cleanup", FileLanguage.CSharp);
            dispatcher.RaiseChangeAdded(change);

            dispatcher.RaiseFileSaved("bari.cs");

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
            dispatcher.RaiseFileGotFocus("foo.cs");
            int initialChangeCount = window.Tasks.Count;
            changeCat.Remove("14");

            dispatcher.WhenChangeListUpdated();

            Assert.AreEqual(initialChangeCount - 1, window.Tasks.Count);
        }
    }
}
