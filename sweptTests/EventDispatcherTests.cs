//  Swept:  Software Enhancement Progress Tracking.  Copyright 2008 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
using NUnit.Framework;
using swept;
using System.Collections.Generic;
using System.Text;
using System;
using System.Xml;

namespace swept.Tests
{
    [TestFixture]
    public class EventDispatcherTests
    {
        Starter starter;
        private TaskWindow window;
        private string fileNameBari;
        private SourceFile bari;
        private ChangeCatalog changeCat;
        private SourceFileCatalog fileCat;

        private EventDispatcher dispatcher;
        private ProjectLibrarian librarian;

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

            fileCat = librarian.InMemorySourceFiles;

            fileNameBari = "bari.cs";
            bari = new SourceFile(fileNameBari);
            bari.Completions.Add(new Completion(indentID));
            fileCat.Files.Add(bari);

            MockLibraryWriter writer = new MockLibraryWriter();
            librarian.writer = writer;

            librarian.LastSavedSourceFiles = new SourceFileCatalog(fileCat);
            librarian.SolutionPath = "mockpath";
            librarian.Persist();

            window = dispatcher.taskWindow;
        }

        
        [Test]
        public void WhenFilePasted_VerifyNewFileHasCompletions_OfOriginal()
        {
            string pastedName = "Copy of bari.cs";
            dispatcher.WhenFilePasted(pastedName);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(librarian.LastSavedSourceFiles.ToXmlText());

            Assert.IsTrue(IsCompletionSaved(doc, pastedName));
        }

        [Test]
        public void WhenFilePasted_ItGetsNoCompletions_IfItDoesNotDuplicate_AnExistingFile()
        {
            string pastedName = "Copy of weezy.cs";
            dispatcher.WhenFilePasted(pastedName);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(librarian.LastSavedSourceFiles.ToXmlText());

            string completionXPath = String.Format("//SourceFile[@Name='{0}']", pastedName);
            Assert.IsNotNull(doc.SelectSingleNode(completionXPath));

            Assert.IsFalse(IsCompletionSaved(doc, pastedName));
        }

        [Test]
        public void WhenFilePasted_ItGetsNoCompletions_IfItIsNotNamed_CopyOfSomething()
        {
            string pastedName = "weezy.cs";
            dispatcher.WhenFilePasted(pastedName);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(librarian.LastSavedSourceFiles.ToXmlText());

            string completionXPath = String.Format("//SourceFile[@Name='{0}']", pastedName);
            Assert.IsNotNull(doc.SelectSingleNode(completionXPath));

            Assert.IsFalse(IsCompletionSaved(doc, pastedName));
        }


        [Test]
        public void WhenFileSavedAs_NewFileGetsCompletions_OfOriginal()
        {
            string originalName = "gadgets.cs";
            SourceFile originalFile = new SourceFile(originalName);
            originalFile.Completions.Add(new Completion("14"));
            fileCat.Files.Add(originalFile);

            string newName = "new" + originalName;
            dispatcher.WhenFileSavedAs(originalName, newName);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(librarian.LastSavedSourceFiles.ToXmlText());

            //  "newgadgets" now exists, with the "14" completion saved
            Assert.IsTrue(IsCompletionSaved(doc, newName));
        }

        [Test]
        public void WhenFileSavedAs_OriginalStillHas_EarlierCompletions()
        {
            string originalName = "gadgets.cs";
            SourceFile originalFile = new SourceFile(originalName);
            fileCat.Files.Add(originalFile);

            changeCat.Add(new Change("12", "Replace old MultiSelect control with new one", FileLanguage.CSharp));
            originalFile.Completions.Add(new Completion("12"));

            dispatcher.WhenFileSaved(originalName);
            
            originalFile.Completions.Add(new Completion("14"));

            string newName = "new" + originalName;
            dispatcher.WhenFileSavedAs(originalName, newName);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(librarian.LastSavedSourceFiles.ToXmlText());

            //  Original file still exists, without pending unsaved change
            //  But it does have all earlier saved changes
            Assert.IsFalse(IsCompletionSaved(doc, originalName));
            Assert.IsTrue(IsCompletionSaved(doc, originalName, "12"));
        }


        [Test]
        public void WhenFileSaved_OtherFilesRemainUnsaved()
        {
            string gadgetsName = "gadgets.cs";
            SourceFile gadgetsFile = new SourceFile(gadgetsName);
            gadgetsFile.Completions.Add(new Completion("14"));
            fileCat.Files.Add(gadgetsFile);

            string widgetsName = "widgets.cs";
            SourceFile widgetsFile = new SourceFile(widgetsName);
            widgetsFile.Completions.Add(new Completion("14"));
            fileCat.Files.Add(widgetsFile);

            //  User saves gadgets, but not widgets
            dispatcher.WhenFileSaved(gadgetsName);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(librarian.LastSavedSourceFiles.ToXmlText());

            Assert.IsTrue(IsCompletionSaved(doc, "bari.cs"));
            Assert.IsTrue(IsCompletionSaved(doc, gadgetsName));
            Assert.IsFalse(IsCompletionSaved(doc, widgetsName));
        }

        [Test]
        public void WhenFileSaved_CatalogIsPersisted()
        {
            string nameGadgets = "gadgets.cs";
            SourceFile fileGadgets = new SourceFile(nameGadgets);
            fileGadgets.Completions.Add(new Completion("14"));
            fileCat.Add(fileGadgets);
            librarian.sourceIsDirty = true;
 
            Assert.IsTrue(librarian.ChangeNeedsPersisting);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(librarian.LastSavedSourceFiles.ToXmlText());

            Assert.IsTrue(IsCompletionSaved(doc, "bari.cs"));
            dispatcher.WhenFileSaved(nameGadgets);

            Assert.IsFalse(librarian.ChangeNeedsPersisting);

            //TODO, check that the output is as it should be
            // i.e., bari should have a completion for change id 14.
        }

        [Test]
        public void WhenFileSaved_NewUnsavedFile_NotAddedToCatalog()
        {
            //prep filecatalog with two SourceFiles
            Completion completion = new Completion( "14" );

            string fileNameUnsaved = "widgets.cs";
            SourceFile fileUnsaved = new SourceFile( fileNameUnsaved );
            fileCat.Files.Add( fileUnsaved );

            // complete something on widgets
            fileUnsaved.Completions.Add( completion );

            // save bari
            dispatcher.WhenFileSaved( fileNameBari );

            XmlDocument doc = new XmlDocument();
            doc.LoadXml( librarian.LastSavedSourceFiles.ToXmlText() );

            //check that bari.cs is saved
            Assert.IsTrue( IsCompletionSaved( doc, "bari.cs" ) );

            //check widgets.cs doesn't exist
            Assert.IsNull( doc.SelectSingleNode( "//SourceFile[@Name='widgets.cs']" ) );
        } 

        [Test]
        public void WhenTaskCompletionChanged_CatalogNeedsPersistence()
        {
            Assert.IsFalse(librarian.ChangeNeedsPersisting);

            dispatcher.WhenTaskCompletionChanged();

            Assert.IsTrue(librarian.ChangeNeedsPersisting);
        }

        private static bool IsCompletionSaved(XmlDocument doc, string fileName)
        {
            return IsCompletionSaved(doc, fileName, "14");
        }

        private static bool IsCompletionSaved(XmlDocument doc, string fileName, string id)
        {
            string completionXPath = String.Format("//SourceFile[@Name='{0}']/Completion[@ID='{1}']", fileName, id);
            return doc.SelectSingleNode(completionXPath) != null;
        }


        [Test]
        public void WhenFileGetsFocus_BecomesCurrentFile()
        {
            dispatcher.WhenFileGotFocus( "foo.cs" );
            Assert.AreEqual( "foo.cs", window.File.Name );

            dispatcher.WhenFileGotFocus( "bar.cs" );
            Assert.AreEqual( "bar.cs", window.File.Name );
        }

        [Test]
        public void WhenFileGetsFocus_TaskWindowUpdates()
        {
            dispatcher.WhenFileGotFocus( "foo.cs" );
            Assert.AreEqual( "foo.cs", window.File.Name );
        }

        [Test]
        public void FileFocusChange_IncludesUnsavedCompletions()
        {
            librarian.changeCatalog.Add( new Change( "728", "Date Normalization", FileLanguage.CSharp ) );
            dispatcher.WhenFileGotFocus( "party_planning.cs" );

            Assert.AreEqual( 2, window.Tasks.Count );
            SourceFile partyFile = window.File;
            Assert.AreEqual( 0, partyFile.Completions.Count );

            window.ClickEntry( 0 );
            window.ClickEntry( 1 );

            //  Nothing recorded in the partyFile completions yet
            Assert.AreEqual( 0, partyFile.Completions.Count );

            //  User done with party planning--switch to another file
            dispatcher.WhenFileGotFocus( fileNameBari );

            //  Now we've stored completions in the working source file object
            Assert.AreEqual( 2, partyFile.Completions.Count );
        }

        [Test]
        public void WhenNonSourceGetsFocus_NoSourceFileInTaskWindow()
        {
            dispatcher.WhenFileGotFocus("foo.cs");
            Assert.AreEqual("foo.cs", window.Title);
            Assert.AreEqual(1, window.Tasks.Count);

            dispatcher.WhenNonSourceGetsFocus();
            
            Assert.AreEqual( "No source file", window.Title );
            Assert.AreEqual(0, window.Tasks.Count);
        }

        [Test]
        public void WhenPluginStarted_DispatcherCreatesDiskLibrarian()
        {
            ProjectLibrarian incumbent = dispatcher.Librarian;

            dispatcher.WhenPluginStarted();

            Assert.IsNotNull( dispatcher.Librarian );
            Assert.AreNotSame( incumbent, dispatcher.Librarian );
        }

        [Test]
        public void WhenFileChangesAbandoned_PreexistingCompletionsKept()
        {
            AbandonFileChanges( fileNameBari );
        }

        [Test]
        public void WhenFileChangesAbandoned_NewFileNotRecorded()
        {
            AbandonFileChanges( "badfile.cs" );
        }

        private void AbandonFileChanges( string fileName )
        {
            SourceFile file = librarian.FetchWorkingFile( fileName );
            int startingCompletionsCount = file.Completions.Count;

            file.AddNewCompletion( "id_88" );
            file.AddNewCompletion( "id_99" );
            dispatcher.WhenFileChangesAbandoned( fileName );

            Assert.AreEqual( startingCompletionsCount, file.Completions.Count );
        }


        [Test]
        public void WhenSolutionOpened_LibrarianGetsNewPath()
        {
            string newPath = @"new\location";
            dispatcher.WhenSolutionOpened( newPath );
            Assert.AreEqual( newPath, dispatcher.Librarian.SolutionPath );
        }

        [Test]
        public void WhenFileDeleted_FileRemovedFromCatalogs()
        {
            Assert.IsTrue( fileCat.Files.Contains( bari ) );

            dispatcher.WhenFileDeleted( fileNameBari );

            Assert.IsFalse( fileCat.Files.Contains( bari ) );
            Assert.IsFalse(librarian.ChangeNeedsPersisting);
            Assert.IsNotNull(librarian.LastSavedSourceFiles);
        }

        [Test]
        public void WhenSourceFileRenamed_ChangesAreCarriedOver()
        {
            Assert.IsTrue(fileCat.Files.Contains(bari));
            
            Assert.AreEqual(1, bari.Completions.Count);

            string newName = "nextgreatname.cs";
            dispatcher.WhenFileRenamed(fileNameBari, newName);

            SourceFile nextGreat = fileCat.FetchFile(newName);
            Assert.AreEqual(1, nextGreat.Completions.Count);

            Assert.IsFalse(librarian.ChangeNeedsPersisting);
            Assert.IsNotNull(librarian.LastSavedSourceFiles);
        }

        [Test]
        public void WhenSolutionSaved_DiskCatalogSaved()
        {
            librarian.sourceIsDirty = true;
            dispatcher.WhenSolutionSaved();
            Assert.IsFalse(librarian.ChangeNeedsPersisting);
        }

        [Test]
        public void WhenTaskWindowToggled_VisibilityChanges()
        {
            dispatcher.taskWindow.Visible = false;
            dispatcher.WhenTaskWindowToggled();

            Assert.IsTrue(dispatcher.taskWindow.Visible);
        }
    }
}
