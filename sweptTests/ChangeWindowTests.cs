using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace swept.Tests
{
    [TestFixture]
    public class ChangeWindowTests
    {
        private string fileNameBari;
        private SourceFile bar;
        private ChangeCatalog changeCat;
        private SourceFileCatalog sourceCat;

        private EventDispatcher dispatcher;
        private Librarian librarian;

        private TaskWindow taskWindow;
        private ChangeWindow changeWindow;
        //private ChangeCatalog changeCat;
        //private EventDispatcher dispatcher;


        [SetUp]
        public void SetUp()
        {
            changeCat = new ChangeCatalog();
            string indentID = "14";
            changeCat.Add(new Change(indentID, "indentation cleanup", FileLanguage.CSharp));

            bar = new SourceFile("bar.cs");

            sourceCat = new SourceFileCatalog();
            sourceCat.ChangeCatalog = changeCat;

            sourceCat.Files.Add(bar);

            librarian = new Librarian();
            librarian.InMemorySourceFiles = sourceCat;
            librarian.LastSavedSourceFiles = new SourceFileCatalog(sourceCat);
            librarian.changeCatalog = changeCat;
            librarian.SolutionPath = "mockpath";

            dispatcher = new EventDispatcher();
            dispatcher.Librarian = librarian;

            librarian.Persist();
            taskWindow = dispatcher.taskWindow = new TaskWindow();

            changeWindow = new ChangeWindow();
            changeWindow.Changes = changeCat;
        }

        [Test]
        public void ChangeWindow_ListsChanges_AddedToCatalog()
        {
            Assert.AreEqual(1, changeWindow.ChangeCount);

            changeCat.Add(new Change("32", "Copyright block", FileLanguage.CSharp));

            Assert.AreEqual(2, changeWindow.ChangeCount);
        }

        [Test]
        public void ChangeWindow_AddChange_AddsToCatalog()
        {
            Assert.AreEqual(1, changeCat.changes.Count);

            changeWindow.AddChange(new Change("99", "More widgets", FileLanguage.HTML));

            Assert.AreEqual(2, changeCat.changes.Count);
        }

    }
}
