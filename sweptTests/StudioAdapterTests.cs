//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using NUnit.Framework;
using swept;
using System.Collections.Generic;
using System.Text;
using System;
using System.Xml;

namespace swept.Tests
{
    [TestFixture]
    public class StudioAdapterTests
    {
        private Starter starter;
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
        public void WhenFilePasted_VerifyNewFileHasCompletions_OfOriginal()
        {
            string pastedName = "Copy of bari.cs";
            adapter.RaiseFilePasted(pastedName);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(librarian.savedSourceImage.ToXmlText());

            Assert.IsTrue(IsCompletionSaved(doc, pastedName));
        }

        [Test]
        public void WhenFilePasted_ItGetsNoCompletions_IfItDoesNotDuplicate_AnExistingFile()
        {
            string pastedName = "Copy of weezy.cs";
            adapter.RaiseFilePasted(pastedName);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(librarian.savedSourceImage.ToXmlText());

            string completionXPath = String.Format("//SourceFile[@Name='{0}']", pastedName);
            Assert.IsNotNull(doc.SelectSingleNode(completionXPath));

            Assert.IsFalse(IsCompletionSaved(doc, pastedName));
        }

        [Test]
        public void WhenFilePasted_ItGetsNoCompletions_IfItIsNotNamed_CopyOfSomething()
        {
            string pastedName = "weezy.cs";
            adapter.RaiseFilePasted(pastedName);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(librarian.savedSourceImage.ToXmlText());

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
            adapter.RaiseFileSavedAs(originalName, newName);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(librarian.savedSourceImage.ToXmlText());

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

            adapter.RaiseFileSaved(originalName);
            
            originalFile.Completions.Add(new Completion("14"));

            string newName = "new" + originalName;
            adapter.RaiseFileSavedAs(originalName, newName);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(librarian.savedSourceImage.ToXmlText());

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
            adapter.RaiseFileSaved(gadgetsName);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(librarian.savedSourceImage.ToXmlText());

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
            librarian.unsavedSourceChangesExist = true;
 
            Assert.IsTrue(librarian.ChangeNeedsPersisting);

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(librarian.savedSourceImage.ToXmlText());

            Assert.IsTrue(IsCompletionSaved(doc, "bari.cs"));
            adapter.RaiseFileSaved(nameGadgets);

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
            adapter.RaiseFileSaved( fileName );

            XmlDocument doc = new XmlDocument();
            doc.LoadXml( librarian.savedSourceImage.ToXmlText() );

            //check that bari.cs is saved
            Assert.IsTrue( IsCompletionSaved( doc, "bari.cs" ) );

            //check widgets.cs doesn't exist
            Assert.IsNull( doc.SelectSingleNode( "//SourceFile[@Name='widgets.cs']" ) );
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
            adapter.RaiseFileGotFocus( "foo.cs" );
            Assert.AreEqual( "foo.cs", window.CurrentFile.Name );

            adapter.RaiseFileGotFocus( "bar.cs" );
            Assert.AreEqual( "bar.cs", window.CurrentFile.Name );
        }

        [Test]
        public void WhenFileGetsFocus_TaskWindowUpdates()
        {
            adapter.RaiseFileGotFocus( "foo.cs" );
            Assert.AreEqual( "foo.cs", window.CurrentFile.Name );
        }

        [Test]
        public void FileFocusChange_IncludesUnsavedCompletions()
        {
            librarian.changeCatalog.Add( new Change( "728", "Date Normalization", FileLanguage.CSharp ) );
            adapter.RaiseFileGotFocus( "party_planning.cs" );

            Assert.AreEqual( 2, window.Tasks.Count );
            SourceFile partyFile = window.CurrentFile;
            Assert.AreEqual( 0, partyFile.Completions.Count );

            window.ToggleTaskCompletion( 0 );
            window.ToggleTaskCompletion( 1 );

            //  partyFile completions are kept up to date
            Assert.AreEqual(2, partyFile.Completions.Count);

            //  User done with party planning--switch to another file
            adapter.RaiseFileGotFocus( fileName );

            //  Even though we're working elsewhere, completions are kept correct
            Assert.AreEqual( 2, partyFile.Completions.Count );
        }

        [Test]
        public void WhenNonSourceGetsFocus_NoSourceFileInTaskWindow()
        {
            adapter.RaiseFileGotFocus("foo.cs");
            Assert.AreEqual("foo.cs", window.Title);
            Assert.AreEqual(1, window.Tasks.Count);

            adapter.RaiseNonSourceGetsFocus();
            
            Assert.AreEqual( "No source file", window.Title );
            Assert.AreEqual(0, window.Tasks.Count);
        }

        [Test]
        public void WhenPluginStarted_DispatcherCreatesDiskLibrarian()
        {
            ProjectLibrarian incumbent = adapter.Librarian;

            adapter.WhenAddinStarted();

            Assert.IsNotNull( adapter.Librarian );
            Assert.AreNotSame( incumbent, adapter.Librarian );
        }

        [Test]
        public void WhenFileChangesAbandoned_PreexistingCompletionsKept()
        {
            AbandonFileChanges( fileName );
        }

        [Test]
        public void WhenFileChangesAbandoned_NewFileNotRecorded()
        {
            AbandonFileChanges( "badfile.cs" );
        }

        private void AbandonFileChanges( string fileName )
        {
            SourceFile file = librarian.FetchUnsavedFile( fileName );
            int startingCompletionsCount = file.Completions.Count;

            file.AddNewCompletion( "id_88" );
            file.AddNewCompletion( "id_99" );
            adapter.RaiseFileChangesAbandoned( fileName );

            Assert.AreEqual( startingCompletionsCount, file.Completions.Count );
        }


        [Test]
        public void WhenSolutionOpened_LibrarianGetsNewPath()
        {
            string newPath = @"new\location";
            adapter.RaiseSolutionOpened( newPath );
            Assert.AreEqual( newPath, adapter.Librarian.SolutionPath );
        }

        // TODO: Change this to not remove file from catalogs
        [Test]
        public void WhenFileDeleted_FileRemovedFromCatalogs()
        {
            Assert.IsTrue( fileCat.Files.Contains( file ) );

            adapter.RaiseFileDeleted( fileName );

            Assert.IsFalse( fileCat.Files.Contains( file ) );
            Assert.IsFalse(librarian.ChangeNeedsPersisting);
            Assert.IsNotNull(librarian.savedSourceImage);
        }
        // TODO: !! add a dialog when re-adding, 'keep or discard history for this source file?'


        [Test]
        public void WhenSourceFileRenamed_ChangesAreCarriedOver()
        {
            Assert.IsTrue(fileCat.Files.Contains(file));
            
            Assert.AreEqual(1, file.Completions.Count);

            string newName = "nextgreatname.cs";
            adapter.RaiseFileRenamed(fileName, newName);

            SourceFile nextGreat = fileCat.Fetch(newName);
            Assert.AreEqual(1, nextGreat.Completions.Count);

            Assert.IsFalse(librarian.ChangeNeedsPersisting);
            Assert.IsNotNull(librarian.savedSourceImage);
        }

        [Test]
        public void WhenSolutionSaved_DiskCatalogSaved()
        {
            librarian.unsavedSourceChangesExist = true;
            adapter.RaiseSolutionSaved();
            Assert.IsFalse(librarian.ChangeNeedsPersisting);
        }
    }
}