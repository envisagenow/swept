//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using NUnit.Framework;
using swept;
using System;
using System.Xml;
using System.Collections.Generic;
using System.IO;

namespace swept.Tests
{
    [CoverageExclude]
    [TestFixture]
    public class ProjectLibrarianTests
    {
        private ProjectLibrarian Horace;

        private string _testingSolutionPath;
        private ChangeCatalog _changeCatalog;
        private MockStorageAdapter _storageAdapter;
        private MockUserAdapter _userAdapter;

        [SetUp]
        public void Setup()
        {
            _testingSolutionPath = @"f:\over\here.sln";
            _storageAdapter = new MockStorageAdapter();
            _changeCatalog = new ChangeCatalog();

            Horace = new ProjectLibrarian( _storageAdapter, _changeCatalog ) 
            { 
                SolutionPath = _testingSolutionPath,
                LibraryPath = Path.ChangeExtension( _testingSolutionPath, "swept.library" )
            };

            _userAdapter = new MockUserAdapter();
            Horace._userAdapter = _userAdapter;
        }

        private static FileEventArgs Get_testfile_FileEventArgs()
        {
            return new FileEventArgs { Name = @"d:\code\CoolProject\mySolution.sln" };
        }

        [Test]
        public void Can_GetSortedChanges()
        {
            Change a_17 = new Change { ID = "a_17", };
            Change a_177 = new Change { ID = "a_177", };
            Change b_52 = new Change { ID = "b_52", };

            _changeCatalog._changes.Clear();
            _changeCatalog.Add( b_52 );
            _changeCatalog.Add( a_17 );
            _changeCatalog.Add( a_177 );

            var changes = Horace.GetSortedChanges();
            Assert.That( changes[0].ID, Is.EqualTo( a_17.ID ) );
            Assert.That( changes[1].ID, Is.EqualTo( a_177.ID ) );
            Assert.That( changes[2].ID, Is.EqualTo( b_52.ID ) );
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

            Assert.AreEqual( 0, _changeCatalog._changes.Count );
            Assert.AreEqual( 0, Horace._sourceCatalog.Files.Count );
        }

        [Test]
        public void OpenSolution_with_invalid_xml_makes_empty_library()
        {
            _storageAdapter.ThrowBadXmlException = true;
            Horace.Hear_SolutionOpened( this, Get_testfile_FileEventArgs() );

            Assert.AreEqual( 0, _changeCatalog._changes.Count );
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
        public void Can_set_SolutionPath()
        {
            Assert.AreEqual( _testingSolutionPath, Horace.SolutionPath );

            string myPath = @"c:\my\project.sln";
            Horace.SolutionPath = myPath;
            Assert.AreEqual( myPath, Horace.SolutionPath );
        }

        [Test]
        public void SolutionName_from_LibraryName()
        {
            
        }
    }
}
