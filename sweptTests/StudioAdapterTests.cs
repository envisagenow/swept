//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
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
            preparer.ShiftSweptToMocks( starter );

            librarian = starter.Librarian;
            adapter = starter.StudioAdapter;

            changeCat = starter.ChangeCatalog;
            string indentID = "14";
            changeCat.Add( new Change { ID = indentID, Description = "indentation cleanup", Language = FileLanguage.CSharp } );
            librarian._savedChangeCatalog = changeCat.Clone();

            fileCat = librarian._sourceCatalog;

            fileName = "bari.cs";
            file = new SourceFile( fileName );
            file.Completions.Add( new Completion( indentID ) );
            fileCat.Files.Add( file );

            librarian._savedSourceCatalog = fileCat.Clone();
            librarian.SolutionPath = @"d:\old_stuff\old.sln";
            librarian.Persist();

            window = adapter.taskWindow;
        }

        [Test]
        public void when_SolutionRenamed_swept_Library_renamed()
        {
            adapter.Raise_SolutionRenamed( @"d:\old_stuff\old.sln", @"c:\stuff\new.sln" );

            Assert.That( librarian.LibraryPath, Is.EqualTo( @"c:\stuff\new.swept.library" ) );
        }

        [Test]
        public void when_FilePasted_new_file_has_Completions_of_original()
        {
            string pastedName = "Copy of bari.cs";
            adapter.Raise_FilePasted( pastedName );

            Assert.IsTrue( TestProbe.IsCompletionSaved( librarian, pastedName ) );
        }

        [Test]
        public void when_FilePasted_no_Completions_if_no_matching_existing_file()
        {
            string pastedName = "Copy of weezy.cs";
            adapter.Raise_FilePasted( pastedName );

            Assert.IsFalse( TestProbe.IsCompletionSaved( librarian, pastedName ) );
        }

        [Test]
        public void when_FilePasted_no_Completions_if_not_named_copy_of_x()
        {
            string pastedName = "weezy.cs";
            adapter.Raise_FilePasted( pastedName );

            Assert.IsFalse( TestProbe.IsCompletionSaved( librarian, pastedName ) );
        }

        [Test]
        public void when_FileSavedAs_new_file_has_Completions_of_original()
        {
            string originalName = "gadgets.cs";
            SourceFile originalFile = new SourceFile( originalName );
            originalFile.Completions.Add( new Completion( "14" ) );
            fileCat.Files.Add( originalFile );

            string newName = "new" + originalName;
            adapter.Raise_FileSavedAs( originalName, newName );

            //  "newgadgets" now exists, with the "14" completion saved
            Assert.IsTrue( TestProbe.IsCompletionSaved( librarian, newName ) );
        }

        [Test]
        public void when_FileSavedAs_original_still_has_earlier_Completions()
        {
            string originalName = "gadgets.cs";
            SourceFile originalFile = new SourceFile( originalName );
            fileCat.Files.Add( originalFile );

            changeCat.Add( new Change { ID = "12", Description = "Replace old MultiSelect control with new one", Language = FileLanguage.CSharp } );
            originalFile.Completions.Add( new Completion( "12" ) );

            adapter.Raise_FileSaved( originalName );

            originalFile.Completions.Add( new Completion( "14" ) );

            string newName = "new" + originalName;
            adapter.Raise_FileSavedAs( originalName, newName );

            //  Original file still exists, without pending unsaved change
            //  But it does have all earlier saved changes
            Assert.IsFalse( TestProbe.IsCompletionSaved( librarian, originalName ) );
            Assert.IsTrue( TestProbe.IsCompletionSaved( librarian, originalName, "12" ) );
        }

        private SourceFile _add_file_with_comp14_to_catalog( string name)
        {
            SourceFile file = new SourceFile( name );
            file.Completions.Add( new Completion( "14" ) );
            fileCat.Files.Add( file );
            return file;
        }

        [Test]
        public void when_FileSaved_other_files_remain_unsaved()
        {
            string gadgetsName = "gadgets.cs";
            string widgetsName = "widgets.cs";

            _add_file_with_comp14_to_catalog( gadgetsName );
            _add_file_with_comp14_to_catalog( widgetsName );
            
            //  User saves gadgets, but not widgets
            adapter.Raise_FileSaved( gadgetsName );

            Assert.IsTrue( TestProbe.IsCompletionSaved( librarian, "bari.cs" ) );
            Assert.IsTrue( TestProbe.IsCompletionSaved( librarian, gadgetsName ) );
            Assert.IsFalse( TestProbe.IsCompletionSaved( librarian, widgetsName ) );
            Assert.IsFalse( librarian._savedSourceCatalog.Files.Exists( fi => fi.Name == widgetsName ) );
        }

        [Test]
        public void when_FileSaved_Catalog_is_persisted()
        {
            string gadgetsName = "gadgets.cs";
            _add_file_with_comp14_to_catalog( gadgetsName );

            Assert.IsFalse( librarian.IsSaved );
            adapter.Raise_FileSaved( gadgetsName );

            Assert.IsTrue( librarian.IsSaved );
        }

        [Test]
        public void when_FileGotFocus_it_becomes_CurrentFile()
        {
            adapter.Raise_FileGotFocus( "foo.cs", "using System;" );
            Assert.AreEqual( "foo.cs", window.CurrentFile.Name );

            adapter.Raise_FileGotFocus( "bar.cs", "using System;" );
            Assert.AreEqual( "bar.cs", window.CurrentFile.Name );
        }

        [Test]
        public void when_FileGotFocus_SourceFile_gets_content()
        {
            adapter.Raise_FileGotFocus( "foo.cs", "using System;" );
            Assert.That( window.CurrentFile.Content, Is.EqualTo( "using System;" ) );

            adapter.Raise_FileGotFocus( "foo.cs", "using Chaos;" );
            Assert.That( window.CurrentFile.Content, Is.EqualTo( "using Chaos;" ) );
        }

        [Test]
        public void when_FileGotFocus_TaskWindow_updates()
        {
            adapter.Raise_FileGotFocus( "foo.cs", "using System;" );
            Assert.AreEqual( "foo.cs", window.CurrentFile.Name );
        }

        [Test]
        public void when_FileGotFocus_unsaved_completions_included()
        {
            changeCat.Add( new Change { ID = "728", Description = "Date Normalization", Language = FileLanguage.CSharp } );
            adapter.Raise_FileGotFocus( "party_planning.cs", "using System;" );

            Assert.AreEqual( 2, window.Tasks.Count );
            SourceFile partyFile = window.CurrentFile;
            Assert.AreEqual( 0, partyFile.Completions.Count );

            window.ToggleTaskCompletion( 0 );
            window.ToggleTaskCompletion( 1 );

            //  partyFile completions are kept up to date
            Assert.AreEqual( 2, partyFile.Completions.Count );

            //  User done with party planning--switch to another file
            adapter.Raise_FileGotFocus( fileName, "using System;" );

            //  Even though we're working elsewhere, completions are kept correct
            Assert.AreEqual( 2, partyFile.Completions.Count );
        }

        [Test]
        public void when_NonSourceGetsFocus_TaskWindow_is_empty()
        {
            adapter.Raise_FileGotFocus( "foo.cs", "using System;" );
            Assert.AreEqual( "foo.cs", window.Title );
            Assert.AreEqual( 1, window.Tasks.Count );

            adapter.Raise_NonSourceGetsFocus();

            Assert.AreEqual( "No source file", window.Title );
            Assert.AreEqual( 0, window.Tasks.Count );
        }

        [Test]
        public void when_FileChangesAbandoned_preexisting_Completions_kept()
        {
            _add_completions_then_raise_FileChangesAbandoned( fileName );
        }

        [Test]
        public void when_FileChangesAbandoned_new_file_not_recorded()
        {
            _add_completions_then_raise_FileChangesAbandoned( "badfile.cs" );
        }

        private void _add_completions_then_raise_FileChangesAbandoned( string fileName )
        {
            SourceFile file = librarian._sourceCatalog.Fetch( fileName );
            int startingCompletionsCount = file.Completions.Count;

            file.AddNewCompletion( "id_88" );
            file.AddNewCompletion( "id_99" );
            adapter.Raise_FileChangesAbandoned( fileName );

            Assert.AreEqual( startingCompletionsCount, file.Completions.Count );
        }

        [Test]
        public void when_SolutionOpened_Librarian_gets_new_path()
        {
            string newPath = @"new\location";
            adapter.Raise_SolutionOpened( newPath );
            Assert.AreEqual( newPath, adapter.Librarian.SolutionPath );
        }

        [Test]
        public void when_FileRenamed_Completions_are_carried_over()
        {
            Assert.IsTrue( fileCat.Files.Contains( file ) );

            Assert.AreEqual( 1, file.Completions.Count );

            string newName = "nextgreatname.cs";
            adapter.Raise_FileRenamed( fileName, newName );

            SourceFile nextGreat = fileCat.Fetch( newName );
            Assert.AreEqual( 1, nextGreat.Completions.Count );

            Assert.IsTrue( librarian.IsSaved );
            Assert.IsNotNull( librarian._savedSourceCatalog );
        }

        [Test]
        public void when_SolutionSaved_DiskCatalog_saved()
        {
            Assert.IsTrue( librarian.IsSaved );
            librarian._sourceCatalog.Add( new SourceFile( "moo.cs" ) );
            Assert.IsFalse( librarian.IsSaved );
            adapter.Raise_SolutionSaved();
            Assert.IsTrue( librarian.IsSaved );
        }
    }
}
