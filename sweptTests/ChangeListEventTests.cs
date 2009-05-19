using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using System.Xml;

namespace swept.Tests
{
    [TestFixture]
    public class ChangeListEventTests
    {
        private TaskWindow window;
        private string fileNameBari;
        private SourceFile bari;
        private ChangeCatalog changeCat;
        private SourceFileCatalog fileCat;

        private EventDispatcher dispatcher;
        private Librarian librarian;

        [SetUp]
        public void SetUp()
        {
            changeCat = new ChangeCatalog();
            string indentID = "14";
            changeCat.Add(new Change(indentID, "indentation cleanup", FileLanguage.CSharp));

            fileCat = new SourceFileCatalog();
            fileCat.ChangeCatalog = changeCat;

            fileNameBari = "bari.cs";
            bari = new SourceFile(fileNameBari);
            bari.Completions.Add(new Completion(indentID));
            fileCat.Files.Add(bari);

            librarian = new Librarian();
            librarian.InMemorySourceFiles = fileCat;
            librarian.LastSavedSourceFiles = new SourceFileCatalog(fileCat);
            librarian.changeCatalog = changeCat;
            librarian.SolutionPath = "mockpath";

            dispatcher = new EventDispatcher();
            dispatcher.Librarian = librarian;

            librarian.Persist();
            window = dispatcher.taskWindow = new TaskWindow();
        }


        [Test]
        public void WhenChangeListUpdated_TaskWindow_RefreshesTasks()
        {
            dispatcher.WhenFileGetsFocus("foo.cs");
            int initialChangeCount = window.Tasks.Count;
            changeCat.Add(new Change("Inf09", "Change delegates to lambdas", FileLanguage.CSharp));

            dispatcher.WhenChangeListUpdated();

            Assert.AreEqual(initialChangeCount + 1, window.Tasks.Count);
        }

        [Test]
        public void WhenChangeListUpdated_EmptyTaskWindow_RefreshesItsEmptiness()
        {
            dispatcher.WhenNonSourceGetsFocus();
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

        [Test, Ignore("Incomplete")]
        public void WhenChangeAdded_IfChangeExistedPreviously_UserCanKeepHistory()
        {
            // Remove Change 14
            changeCat.Remove("14");

            // Add Change 14 back
            // User Requests to keep History

            // Bari should have 14 completed already

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(librarian.LastSavedSourceFiles.ToXmlText());
            Assert.IsTrue(IsCompletionSaved(doc, "bari.cs", "14"));
        }

        [Test, Ignore("Incomplete")]
        public void WhenChangeAdded_IfChangeExistedPreviously_UserCanRemoveHistory()
        {
            // Remove Change 14
            changeCat.Remove("14");

            // Add Change 14 back
            // User Requests to remove History

            // Bari should have 14 completed already

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(librarian.LastSavedSourceFiles.ToXmlText());
            Assert.IsTrue(IsCompletionSaved(doc, "bari.cs", "14"));
        }

        private static bool IsCompletionSaved(XmlDocument doc, string fileName, string id)
        {
            string completionXPath = String.Format("//SourceFile[@Name='{0}']/Completion[@ID='{1}']", fileName, id);
            return doc.SelectSingleNode(completionXPath) != null;
        }


        [Test]
        public void WhenChangeRemoved_TaskWindow_RefreshesTasks()
        {
            dispatcher.WhenFileGetsFocus("foo.cs");
            int initialChangeCount = window.Tasks.Count;
            changeCat.Remove("14");

            dispatcher.WhenChangeListUpdated();

            Assert.AreEqual(initialChangeCount - 1, window.Tasks.Count);
        }


    }
}
