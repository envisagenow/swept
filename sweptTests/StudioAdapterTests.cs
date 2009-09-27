//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using NUnit.Framework;
using swept;
using System;

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
            var preparer = new TestPreparer();
            preparer.ShiftStarterToMocks( starter );
            
            librarian = starter.Librarian;
            adapter = starter.Adapter;

            changeCat = librarian._changeCatalog;
            string indentID = "14";
            changeCat.Add(new Change(indentID, "indentation cleanup", FileLanguage.CSharp));
            librarian._savedChangeCatalog = changeCat.Clone();

            fileCat = librarian._sourceCatalog;

            fileName = "bari.cs";
            file = new SourceFile(fileName);
            file.Completions.Add(new Completion(indentID));
            fileCat.Files.Add(file);

            librarian._savedSourceCatalog = fileCat.Clone();
            librarian.SolutionPath = "mockpath";
            librarian.Persist();

            window = adapter.taskWindow;
        }

        
        [Test]
        public void WhenFilePasted_VerifyNewFileHasCompletions_OfOriginal()
        {
            string pastedName = "Copy of bari.cs";
            adapter.Raise_FilePasted(pastedName);

            Assert.IsTrue(TestProbe.IsCompletionSaved(librarian, pastedName));
        }

        [Test]
        public void WhenFilePasted_ItGetsNoCompletions_IfItDoesNotDuplicate_AnExistingFile()
        {
            string pastedName = "Copy of weezy.cs";
            adapter.Raise_FilePasted(pastedName);

            Assert.IsFalse(TestProbe.IsCompletionSaved(librarian, pastedName));
        }

        [Test]
        public void WhenFilePasted_ItGetsNoCompletions_IfItIsNotNamed_CopyOfSomething()
        {
            string pastedName = "weezy.cs";
            adapter.Raise_FilePasted(pastedName);

            Assert.IsFalse(TestProbe.IsCompletionSaved(librarian, pastedName));
        }


        [Test]
        public void WhenFileSavedAs_NewFileGetsCompletions_OfOriginal()
        {
            string originalName = "gadgets.cs";
            SourceFile originalFile = new SourceFile(originalName);
            originalFile.Completions.Add(new Completion("14"));
            fileCat.Files.Add(originalFile);

            string newName = "new" + originalName;
            adapter.Raise_FileSavedAs(originalName, newName);

            //  "newgadgets" now exists, with the "14" completion saved
            Assert.IsTrue(TestProbe.IsCompletionSaved(librarian, newName));
        }

        [Test]
        public void WhenFileSavedAs_OriginalStillHas_EarlierCompletions()
        {
            string originalName = "gadgets.cs";
            SourceFile originalFile = new SourceFile(originalName);
            fileCat.Files.Add(originalFile);

            changeCat.Add(new Change("12", "Replace old MultiSelect control with new one", FileLanguage.CSharp));
            originalFile.Completions.Add(new Completion("12"));

            adapter.Raise_FileSaved(originalName);
            
            originalFile.Completions.Add(new Completion("14"));

            string newName = "new" + originalName;
            adapter.Raise_FileSavedAs(originalName, newName);

            //  Original file still exists, without pending unsaved change
            //  But it does have all earlier saved changes
            Assert.IsFalse(TestProbe.IsCompletionSaved(librarian, originalName));
            Assert.IsTrue(TestProbe.IsCompletionSaved(librarian, originalName, "12"));
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
            adapter.Raise_FileSaved(gadgetsName);

            Assert.IsTrue(TestProbe.IsCompletionSaved(librarian, "bari.cs"));
            Assert.IsTrue(TestProbe.IsCompletionSaved(librarian, gadgetsName));
            Assert.IsFalse(TestProbe.IsCompletionSaved(librarian, widgetsName));
        }

        [Test]
        public void WhenFileSaved_CatalogIsPersisted()
        {
            string nameGadgets = "gadgets.cs";
            SourceFile fileGadgets = new SourceFile(nameGadgets);
            fileGadgets.Completions.Add(new Completion("14"));
            fileCat.Add(fileGadgets);
 
            Assert.IsFalse(librarian.IsSaved);

            Assert.IsTrue(TestProbe.IsCompletionSaved(librarian, "bari.cs"));
            adapter.Raise_FileSaved(nameGadgets);

            Assert.IsTrue(librarian.IsSaved);

            Assert.IsTrue(TestProbe.IsCompletionSaved(librarian, "bari.cs"));
        }

        [Test]
        public void When_single_file_saved_other_files_remain_unsaved()
        {
            // set up a new file (widgets.cs) with unsaved changes
            Completion completion = new Completion( "14" );

            string fileNameUnsaved = "widgets.cs";
            SourceFile fileUnsaved = new SourceFile( fileNameUnsaved );
            fileCat.Files.Add( fileUnsaved );

            fileUnsaved.Completions.Add( completion );

            // save bari
            adapter.Raise_FileSaved( fileName );
            Assert.IsTrue(TestProbe.IsCompletionSaved(librarian, "bari.cs"));

            //check widgets.cs doesn't exist
            Assert.IsFalse(librarian._savedSourceCatalog.Files.Exists(fi => fi.Name == fileNameUnsaved));
        }

        [Test]
        public void WhenFileGetsFocus_BecomesCurrentFile()
        {
            adapter.Raise_FileGotFocus( "foo.cs" );
            Assert.AreEqual( "foo.cs", window.CurrentFile.Name );

            adapter.Raise_FileGotFocus( "bar.cs" );
            Assert.AreEqual( "bar.cs", window.CurrentFile.Name );
        }

        [Test]
        public void WhenFileGetsFocus_TaskWindowUpdates()
        {
            adapter.Raise_FileGotFocus( "foo.cs" );
            Assert.AreEqual( "foo.cs", window.CurrentFile.Name );
        }

        [Test]
        public void FileFocusChange_IncludesUnsavedCompletions()
        {
            librarian._changeCatalog.Add( new Change( "728", "Date Normalization", FileLanguage.CSharp ) );
            adapter.Raise_FileGotFocus( "party_planning.cs" );

            Assert.AreEqual( 2, window.Tasks.Count );
            SourceFile partyFile = window.CurrentFile;
            Assert.AreEqual( 0, partyFile.Completions.Count );

            window.ToggleTaskCompletion( 0 );
            window.ToggleTaskCompletion( 1 );

            //  partyFile completions are kept up to date
            Assert.AreEqual(2, partyFile.Completions.Count);

            //  User done with party planning--switch to another file
            adapter.Raise_FileGotFocus( fileName );

            //  Even though we're working elsewhere, completions are kept correct
            Assert.AreEqual( 2, partyFile.Completions.Count );
        }

        [Test]
        public void WhenNonSourceGetsFocus_NoSourceFileInTaskWindow()
        {
            adapter.Raise_FileGotFocus("foo.cs");
            Assert.AreEqual("foo.cs", window.Title);
            Assert.AreEqual(1, window.Tasks.Count);

            adapter.Raise_NonSourceGetsFocus();
            
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

        private void AbandonFileChanges(string fileName)
        {
            SourceFile file = librarian._sourceCatalog.Fetch( fileName );
            int startingCompletionsCount = file.Completions.Count;

            file.AddNewCompletion( "id_88" );
            file.AddNewCompletion( "id_99" );
            adapter.Raise_FileChangesAbandoned( fileName );

            Assert.AreEqual( startingCompletionsCount, file.Completions.Count );
        }


        [Test, Exemplary( "Not really, but I wanted an example." )]
        public void WhenSolutionOpened_LibrarianGetsNewPath()
        {
            string newPath = @"new\location";
            adapter.Raise_SolutionOpened( newPath );
            Assert.AreEqual( newPath, adapter.Librarian.SolutionPath );
        }

        // TODO: Change this to not remove file from catalogs
        [Test]
        public void WhenFileDeleted_FileRemovedFromCatalogs()
        {
            Assert.IsTrue( fileCat.Files.Contains( file ) );

            adapter.Raise_FileDeleted( fileName );

            Assert.IsFalse( fileCat.Files.Contains( file ) );
            Assert.IsTrue(librarian.IsSaved);
            Assert.IsNotNull(librarian._savedSourceCatalog);
        }
        // TODO: !! add a dialog when re-adding, 'keep or discard history for this source file?'


        [Test]
        public void WhenSourceFileRenamed_ChangesAreCarriedOver()
        {
            Assert.IsTrue(fileCat.Files.Contains(file));
            
            Assert.AreEqual(1, file.Completions.Count);

            string newName = "nextgreatname.cs";
            adapter.Raise_FileRenamed(fileName, newName);

            SourceFile nextGreat = fileCat.Fetch(newName);
            Assert.AreEqual(1, nextGreat.Completions.Count);

            Assert.IsTrue(librarian.IsSaved);
            Assert.IsNotNull(librarian._savedSourceCatalog);
        }

        [Test]
        public void WhenSolutionSaved_DiskCatalogSaved()
        {
            Assert.IsTrue( librarian.IsSaved );
            librarian._sourceCatalog.Add( new SourceFile( "moo.cs" ) );
            Assert.IsFalse(librarian.IsSaved);
            adapter.Raise_SolutionSaved();
            Assert.IsTrue(librarian.IsSaved);
        }
    }
}
