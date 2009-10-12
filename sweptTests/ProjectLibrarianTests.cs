//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using NUnit.Framework;
using swept;
using System;
using System.Xml;

namespace swept.Tests
{
    [TestFixture]
    public class ProjectLibrarianTests
    {
        private ProjectLibrarian Horace;

        private string _testingSolutionPath;
        private MockStorageAdapter _FSAdapter;

        [SetUp]
        public void Setup()
        {
            _testingSolutionPath = @"f:\over\here.sln";
            Horace = new ProjectLibrarian { SolutionPath = _testingSolutionPath };

            _FSAdapter = new MockStorageAdapter();
            Horace._storageAdapter = _FSAdapter;
            Horace._userAdapter = new MockUserAdapter();
        }

        private static FileEventArgs Get_testfile_FileEventArgs()
        {
            return new FileEventArgs { Name = @"d:\code\CoolProject\mySolution.sln" };
        }

        [Test]
        public void Swept_Library_sought_in_expected_location()
        {
            Assert.AreEqual( @"f:\over\here.swept.library", Horace.LibraryPath );

            Horace.Hear_SolutionOpened( this, Get_testfile_FileEventArgs() );
            Assert.AreEqual( @"d:\code\CoolProject\mySolution.swept.library", Horace.LibraryPath );
        }

        [Test]
        public void OpenSolution_finding_Swept_Library_will_load_SourceFiles()
        {
            _FSAdapter.LibraryDoc = new XmlDocument();
            _FSAdapter.LibraryDoc.LoadXml( TestProbe.SingleFileLibrary_text );
            Horace.Hear_SolutionOpened( this, Get_testfile_FileEventArgs() );

            SourceFile someFile = Horace._savedSourceCatalog.Fetch( "some_file.cs" );

            Assert.AreEqual( 1, someFile.Completions.Count );
        }

        [Test]
        public void OpenSolution_finding_Swept_Library_will_load_Changes()
        {
            _FSAdapter.LibraryDoc = new XmlDocument();
            _FSAdapter.LibraryDoc.LoadXml( TestProbe.SingleChangeLibrary_text );
            Horace.Hear_SolutionOpened( this, Get_testfile_FileEventArgs() );

            Assert.AreEqual( 1, Horace._changeCatalog._changes.Count );
            Change change = Horace._changeCatalog._changes[0];
            Assert.AreEqual( "Update to use persister", change.Description );
            Assert.AreEqual( FileLanguage.CSharp, change.Language );
        }

        [Test]
        public void OpenSolution_with_no_Swept_Library_will_start_smoothly()
        {
            _FSAdapter.LibraryDoc = new XmlDocument();
            _FSAdapter.LibraryDoc.LoadXml( StorageAdapter.emptyCatalogText );

            Horace.Hear_SolutionOpened( this, Get_testfile_FileEventArgs() );

            Assert.AreEqual( 0, Horace._changeCatalog._changes.Count );
            Assert.AreEqual( 0, Horace._sourceCatalog.Files.Count );
        }

        //[Test]
        //public void OpenSolution_with_invalid_xml_UserChoice_new_library()
        //{
        //    _FSAdapter.ThrowBadXmlException = true;

        //    var mockGui = new MockGUIAdapter();
        //    Horace._GUIAdapter = mockGui;
        //    mockGui.StartNewCatalog = true;

        //    Horace.Hear_SolutionOpened( this, Get_testfile_FileEventArgs() );

        //    Assert.AreEqual( 0, Horace._changeCatalog.changes.Count );
        //    Assert.AreEqual( 0, Horace._sourceCatalog.Files.Count );
        //}

        //[Test, ExpectedException]
        //public void OpenSolution_with_invalid_xml_UserChoice_shutdown_swept()
        //{
        //    _FSAdapter.ThrowBadXmlException = true;

        //    var mockGui = new MockGUIAdapter();
        //    Horace._GUIAdapter = mockGui;
        //    mockGui.StartNewCatalog = false;

        //    Horace.Hear_SolutionOpened( this, Get_testfile_FileEventArgs() );
        //}

        [Test]
        public void SaveSolution_will_persist_all_unsaved_SourceFiles()
        {
            string someFileName = "some_file.cs";
            SourceFile someFile = new SourceFile( someFileName );
            someFile.AddNewCompletion( "007" );
            Horace._sourceCatalog.Add( someFile );

            Horace.Hear_SolutionSaved( this, new EventArgs() );

            Assert.AreEqual( toOuterXml( TestProbe.SingleFileLibrary_text ), _FSAdapter.LibraryDoc.OuterXml );
        }

        // TODO--0.2, DC: public void RenameSolution_will_rename_Library_file

        [Test]
        public void Can_set_SolutionPath()
        {
            Assert.AreEqual( _testingSolutionPath, Horace.SolutionPath );

            string myPath = @"c:\my\project.sln";
            Horace.SolutionPath = myPath;
            Assert.AreEqual( myPath, Horace.SolutionPath );
        }

        [Test]
        public void SeparateCatalogs_CreatedBy_SolutionPathChange()
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

            Assert.AreEqual( toOuterXml( TestProbe.SingleFileLibrary_text ), _FSAdapter.LibraryDoc.OuterXml );
        }

        [Test]
        public void Persist_saves_ChangeCatalog()
        {
            Horace._changeCatalog.Add( new Change( "Uno", "Eliminate profanity from error messages.", FileLanguage.CSharp ) );
            Horace.Persist();

            string expectedXmlText =
@"<SweptProjectData>
<ChangeCatalog>
    <Change ID='Uno' Description='Eliminate profanity from error messages.' Language='CSharp' />
</ChangeCatalog>
<SourceFileCatalog>
</SourceFileCatalog>
</SweptProjectData>";

            Assert.AreEqual( toOuterXml( expectedXmlText ), _FSAdapter.LibraryDoc.OuterXml );
        }

        private string toOuterXml( string text )
        {
            var doc = new XmlDocument();
            doc.LoadXml( text );
            return doc.OuterXml;
        }


        [Test]
        public void When_ChangeCatalog_altered_Librarian_reports_not_saved()
        {
            Assert.IsTrue( Horace.ChangeCatalogSaved );
            Assert.IsTrue( Horace.IsSaved );

            Horace._savedChangeCatalog.Add( new Change( "eek", "A mouse", FileLanguage.CSharp ) );

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

            Change change = new Change( "14", "here I am", FileLanguage.CSharp );
            Horace.Hear_ChangeAdded( this, new ChangeEventArgs { change = change } );

            Assert.AreEqual( 1, Horace._changeCatalog._changes.Count );
        }

    }
}
