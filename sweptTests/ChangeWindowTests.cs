//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
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

        private StudioAdapter dispatcher;
        private ProjectLibrarian librarian;

        private TaskWindow taskWindow;
        private ChangeWindow changeWindow;


        [SetUp]
        public void SetUp()
        {
            starter = new Starter();
            starter.Start();
            librarian = starter.Librarian;
            dispatcher = starter.Adapter;

            changeCat = librarian._changeCatalog;
            string indentID = "14";
            changeCat.Add( new Change { ID = indentID, Description = "indentation cleanup", Language = FileLanguage.CSharp } );

            bar = new SourceFile("bar.cs");

            fileCat = librarian._sourceCatalog;
            fileCat.Files.Add(bar);

            taskWindow = dispatcher.taskWindow;
            changeWindow = dispatcher.changeWindow;
        }

        [Test]
        public void ChangeWindow_ListsChanges_AddedToCatalog()
        {
            Assert.AreEqual(1, changeWindow.ChangeCount);

            changeCat.Add( new Change { ID = "32", Description = "Copyright block", Language = FileLanguage.CSharp } );

            Assert.AreEqual(2, changeWindow.ChangeCount);
        }

        [Test]
        public void ChangeWindow_AddChange_AddsToCatalog()
        {
            Assert.AreEqual(1, changeCat._changes.Count);

            changeWindow.AddChange(new Change{ ID = "99", Description = "More widgets", Language = FileLanguage.HTML});

            Assert.AreEqual(2, changeCat._changes.Count);
        }

    }
}
