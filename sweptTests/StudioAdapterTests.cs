//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
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
            fileCat.Files.Add( file );

            librarian._savedSourceCatalog = fileCat.Clone();
            librarian.SolutionPath = @"d:\old_stuff\old.sln";

            window = adapter.taskWindow;
        }

        [Test]
        public void when_SolutionRenamed_swept_Library_renamed()
        {
            adapter.Raise_SolutionRenamed( @"d:\old_stuff\old.sln", @"c:\stuff\new.sln" );

            Assert.That( librarian.LibraryPath, Is.EqualTo( @"c:\stuff\new.swept.library" ) );
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
        public void when_SolutionOpened_Librarian_gets_new_path()
        {
            string newPath = @"new\location";
            adapter.Raise_SolutionOpened( newPath );
            Assert.AreEqual( newPath, adapter.Librarian.SolutionPath );
        }
    }
}
