//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using NUnit.Framework;
using swept;
using System;
using System.Xml;
using System.Collections.Generic;

namespace swept.Tests
{
    [TestFixture]
    public class ProjectLibrarianTests
    {
        private ProjectLibrarian Horace;

        private string _testingSolutionPath;
        private MockStorageAdapter _storageAdapter;
        private MockUserAdapter _userAdapter;

        [SetUp]
        public void Setup()
        {
            _testingSolutionPath = @"f:\over\here.sln";
            Horace = new ProjectLibrarian { SolutionPath = _testingSolutionPath };

            _storageAdapter = new MockStorageAdapter();
            Horace._storageAdapter = _storageAdapter;
            _userAdapter = new MockUserAdapter();
            Horace._userAdapter = _userAdapter;
        }

        private static FileEventArgs Get_testfile_FileEventArgs()
        {
            return new FileEventArgs { Name = @"d:\code\CoolProject\mySolution.sln" };
        }

        [Test]
        public void Swept_Library_opened_sought_in_expected_location()
        {
            Assert.AreEqual( @"f:\over\here.swept.library", Horace.LibraryPath );

            Horace.Hear_SolutionOpened( this, Get_testfile_FileEventArgs() );
            Assert.AreEqual( @"d:\code\CoolProject\mySolution.swept.library", Horace.LibraryPath );
        }

        [Test]
        public void Swept_Library_renamed_sought_in_expected_location()
        {
            FileListEventArgs renameArgs = new FileListEventArgs { Names = new List<string> { @"d:\code\old.sln", @"c:\newplace\new.sln" } };
            Horace.Hear_SolutionRenamed( this, renameArgs );

            Assert.AreEqual( @"c:\newplace\new.swept.library", Horace.LibraryPath );
        }

        [Test]
        public void Swept_Library_renamed_renames_the_library_on_disk()
        {
            string oldSln = @"f:\over\here.sln";
            string newSln = @"c:\newplace\new.sln";
            FileListEventArgs renameArgs = new FileListEventArgs { Names = new List<string> { oldSln, newSln } };
            Horace.Hear_SolutionRenamed( this, renameArgs );

            Assert.That( _storageAdapter.renamedOldLibraryPath, Is.EqualTo( @"f:\over\here.swept.library" ) );
            Assert.That( _storageAdapter.renamedNewLibraryPath, Is.EqualTo( @"c:\newplace\new.swept.library" ) );
        }

        [Test]
        public void OpenSolution_finding_Swept_Library_will_load_SourceFiles()
        {
            _storageAdapter.LibraryDoc = new XmlDocument();
            _storageAdapter.LibraryDoc.LoadXml( TestProbe.SingleFileLibrary_text );
            Horace.Hear_SolutionOpened( this, Get_testfile_FileEventArgs() );

            SourceFile someFile = Horace._savedSourceCatalog.Fetch( "some_file.cs" );

            Assert.AreEqual( 1, someFile.Completions.Count );
        }

        [Test]
        public void OpenSolution_finding_Swept_Library_will_load_Changes()
        {
            _storageAdapter.LibraryDoc = new XmlDocument();
            _storageAdapter.LibraryDoc.LoadXml( TestProbe.SingleChangeLibrary_text );
            Horace.Hear_SolutionOpened( this, Get_testfile_FileEventArgs() );

            Assert.AreEqual( 1, Horace._changeCatalog._changes.Count );
            Change change = Horace._changeCatalog._changes[0];
            Assert.AreEqual( "Update to use persister", change.Description );
            Assert.AreEqual( FileLanguage.CSharp, change.Language );
        }

        [Test]
        public void OpenSolution_with_no_Swept_Library_will_start_smoothly()
        {
            _storageAdapter.LibraryDoc = new XmlDocument();
            _storageAdapter.LibraryDoc.LoadXml( StorageAdapter.emptyCatalogText );

            Horace.Hear_SolutionOpened( this, Get_testfile_FileEventArgs() );

            Assert.AreEqual( 0, Horace._changeCatalog._changes.Count );
            Assert.AreEqual( 0, Horace._sourceCatalog.Files.Count );
        }

        [Test]
        public void OpenSolution_with_invalid_xml_makes_empty_library()
        {
            _storageAdapter.ThrowBadXmlException = true;
            Horace.Hear_SolutionOpened( this, Get_testfile_FileEventArgs() );

            Assert.AreEqual( 0, Horace._changeCatalog._changes.Count );
            Assert.AreEqual( 0, Horace._sourceCatalog.Files.Count );
        }

        [Test]
        public void OpenSolution_with_invalid_xml_sends_bad_library_message()
        {
            _storageAdapter.ThrowBadXmlException = true;
            Horace.Hear_SolutionOpened( this, Get_testfile_FileEventArgs() );

            Assert.That( _userAdapter.SentBadLibraryMessage );
        }

        [Test]
        public void SaveSolution_will_persist_all_unsaved_SourceFiles()
        {
            string someFileName = "some_file.cs";
            SourceFile someFile = new SourceFile( someFileName );
            someFile.AddNewCompletion( "007" );
            Horace._sourceCatalog.Add( someFile );

            Horace.Hear_SolutionSaved( this, new EventArgs() );

            Assert.AreEqual( toOuterXml( TestProbe.SingleFileLibrary_text ), _storageAdapter.LibraryDoc.OuterXml );
        }

        [Test]
        public void Can_set_SolutionPath()
        {
            Assert.AreEqual( _testingSolutionPath, Horace.SolutionPath );

            string myPath = @"c:\my\project.sln";
            Horace.SolutionPath = myPath;
            Assert.AreEqual( myPath, Horace.SolutionPath );
        }

        [Test]
        public void separate_Catalogs_created_by_solution_path_difference()
        {
            Assert.IsNotNull( Horace._sourceCatalog );
            Assert.IsNotNull( Horace._savedSourceCatalog );
            Assert.AreNotSame( Horace._sourceCatalog, Horace._savedSourceCatalog );
        }

        [Test]
        public void SaveFile_persists_file_Completions()
        {
            string someFileName = "some_file.cs";
            SourceFile someFile = new SourceFile( someFileName );
            someFile.AddNewCompletion( "007" );
            Horace._sourceCatalog.Add( someFile );

            FileEventArgs args = new FileEventArgs { Name = someFileName };
            Horace.Hear_FileSaved( this, args );

            Assert.AreEqual( toOuterXml( TestProbe.SingleFileLibrary_text ), _storageAdapter.LibraryDoc.OuterXml );
        }

        [Test]
        public void persist_saves_ChangeCatalog()
        {
            Horace._changeCatalog.Add( new Change { ID = "Uno", Description = "Eliminate profanity from error messages.", Language = FileLanguage.CSharp } );
            Horace.Persist();

            string expectedXmlText =
@"<SweptProjectData>
<ChangeCatalog>
    <Change ID='Uno' Description='Eliminate profanity from error messages.' Language='CSharp' />
</ChangeCatalog>
<SourceFileCatalog>
</SourceFileCatalog>
</SweptProjectData>";

            Assert.AreEqual( toOuterXml( expectedXmlText ), _storageAdapter.LibraryDoc.OuterXml );
        }

        private string toOuterXml( string text )
        {
            var doc = new XmlDocument();
            doc.LoadXml( text );
            return doc.OuterXml;
        }


        [Test]
        public void when_ChangeCatalog_altered_Librarian_reports_not_saved()
        {
            Assert.IsTrue( Horace.ChangeCatalogSaved );
            Assert.IsTrue( Horace.IsSaved );

            Horace._savedChangeCatalog.Add( new Change { ID = "eek" } );

            Assert.IsFalse( Horace.ChangeCatalogSaved );
            Assert.IsFalse( Horace.IsSaved );
        }

        private static SourceFile newTestingFile( string someFileName )
        {
            SourceFile someFile = new SourceFile( someFileName );
            someFile.AddNewCompletion( "007" );
            return someFile;
        }

        [Test]
        public void When_SourceFileCatalog_altered_Librarian_reports_not_saved()
        {
            Assert.IsTrue( Horace.SourceFileCatalogSaved );
            Assert.IsTrue( Horace.IsSaved );

            Horace._sourceCatalog.Add( newTestingFile( "some_file.cs" ) );

            Assert.IsFalse( Horace.SourceFileCatalogSaved );
            Assert.IsFalse( Horace.IsSaved );
        }

        [Test]
        public void When_SaveSolution_fires_Librarian_reports_saved()
        {
            Horace._sourceCatalog.Add( newTestingFile( "some_file.cs" ) );
            Assert.IsFalse( Horace.IsSaved );

            Horace.Hear_SolutionSaved( this, new EventArgs() );

            Assert.IsTrue( Horace.SourceFileCatalogSaved );
            Assert.IsTrue( Horace.IsSaved );
        }

        [Test]
        public void Can_AddChange()
        {
            Assert.AreEqual( 0, Horace._changeCatalog._changes.Count );

            Change change = new Change { ID = "14" };
            Horace.Hear_ChangeAdded( this, new ChangeEventArgs { change = change } );

            Assert.AreEqual( 1, Horace._changeCatalog._changes.Count );
        }

    }
}
