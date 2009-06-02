using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace swept.Tests
{
    [TestFixture]
    public class ChangeWindowTests
    {
        Starter starter;

        private SourceFile bar;
        private ChangeCatalog changeCat;
        private SourceFileCatalog fileCat;

        private EventDispatcher dispatcher;
        private ProjectLibrarian librarian;

        private TaskWindow taskWindow;
        private ChangeWindow changeWindow;


        [SetUp]
        public void SetUp()
        {
            starter = new Starter();
            starter.Start();
            librarian = starter.Librarian;
            dispatcher = starter.Dispatcher;

            changeCat = librarian.changeCatalog;
            string indentID = "14";
            changeCat.Add(new Change(indentID, "indentation cleanup", FileLanguage.CSharp));

            bar = new SourceFile("bar.cs");

            fileCat = librarian.InMemorySourceFiles;
            fileCat.Files.Add(bar);

            taskWindow = dispatcher.taskWindow;
            changeWindow = dispatcher.changeWindow;
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
