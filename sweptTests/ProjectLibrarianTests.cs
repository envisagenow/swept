//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using NUnit.Framework;
using swept;
using System;
using System.Xml;
using System.IO;
using swept.DSL;
using System.Collections.Generic;

namespace swept.Tests
{
    [TestFixture]
    public class ProjectLibrarianTests
    {
        private ProjectLibrarian Horace;

        private string _testingSolutionPath;
        private RuleCatalog _ruleCatalog;
        private MockStorageAdapter _storageAdapter;

        private EventSwitchboard _switchboard;

        [SetUp]
        public void Setup()
        {
            _testingSolutionPath = @"f:\over\here.sln";
            _storageAdapter = new MockStorageAdapter();
            _switchboard = new EventSwitchboard();

            Horace = new ProjectLibrarian( _storageAdapter, _switchboard ) 
            { 
                SolutionPath = _testingSolutionPath,
                LibraryPath = Path.ChangeExtension( _testingSolutionPath, "swept.library" )
            };

            _ruleCatalog = Horace._ruleCatalog;
            _tasks = null;
        }

        private static FileEventArgs Get_testfile_FileEventArgs()
        {
            return new FileEventArgs { Name = @"d:\code\CoolProject\mySolution.sln" };
        }

        [Test]
        public void Can_GetSortedRules()
        {
            Rule a_17 = new Rule { ID = "a_17", };
            Rule a_177 = new Rule { ID = "a_177", };
            Rule b_52 = new Rule { ID = "b_52", };

            _ruleCatalog._rules.Clear();
            _ruleCatalog.Add( b_52 );
            _ruleCatalog.Add( a_17 );
            _ruleCatalog.Add( a_177 );

            var rules = Horace.GetSortedRules( new List<string>() );
            Assert.That( rules[0].ID, Is.EqualTo( a_17.ID ) );
            Assert.That( rules[1].ID, Is.EqualTo( a_177.ID ) );
            Assert.That( rules[2].ID, Is.EqualTo( b_52.ID ) );
        }

        [Test]
        public void GetSortedRules_filters_on_rules_argument()
        {
            Rule a_17 = new Rule { ID = "a_17", };
            Rule a_177 = new Rule { ID = "a_177", };
            Rule b_52 = new Rule { ID = "b_52", };

            _ruleCatalog._rules.Clear();
            _ruleCatalog.Add( b_52 );
            _ruleCatalog.Add( a_17 );
            _ruleCatalog.Add( a_177 );

            var rules = Horace.GetSortedRules( new List<string>() );
            Assert.That( rules[0].ID, Is.EqualTo( a_17.ID ) );
            Assert.That( rules[1].ID, Is.EqualTo( a_177.ID ) );
            Assert.That( rules[2].ID, Is.EqualTo( b_52.ID ) );
        }

        [Test]
        public void librarian_Excludedfolders_available()
        {
            _storageAdapter.LibraryDoc = new XmlDocument();
            _storageAdapter.LibraryDoc.LoadXml( TestProbe.ExcludedFolderLibrary_text );

            Horace.Hear_SolutionOpened( this, Get_testfile_FileEventArgs() );

            Assert.That( Horace.GetExcludedFolders().Count, Is.EqualTo( 8 ) );
        }

        [Test]
        public void Swept_Library_opened_sought_in_expected_location()
        {
            Assert.AreEqual( @"f:\over\here.swept.library", Horace.LibraryPath );

            Horace.Hear_SolutionOpened( this, Get_testfile_FileEventArgs() );
            Assert.AreEqual( @"d:\code\CoolProject\mySolution.swept.library", Horace.LibraryPath );
        }

        [Test]
        public void OpenSolution_finding_Swept_Library_will_load_Rules()
        {
            _storageAdapter.LibraryDoc = new XmlDocument();
            _storageAdapter.LibraryDoc.LoadXml( TestProbe.SingleRuleLibrary_text );
            Horace.Hear_SolutionOpened( this, Get_testfile_FileEventArgs() );

            Assert.AreEqual( 1, Horace._ruleCatalog._rules.Count );
            Rule rule = Horace._ruleCatalog._rules[0];
            Assert.AreEqual( "Update to use persister", rule.Description );
            var dq = rule.Subquery as QueryLanguageNode;
            Assert.AreEqual( FileLanguage.CSharp, dq.Language );
        }

        [Test]
        public void OpenSolution_with_no_Swept_Library_will_start_smoothly()
        {
            _storageAdapter.LibraryDoc = new XmlDocument();
            _storageAdapter.LibraryDoc.LoadXml( StorageAdapter.emptyCatalogText );

            Horace.Hear_SolutionOpened( this, Get_testfile_FileEventArgs() );

            Assert.AreEqual( 0, _ruleCatalog._rules.Count );
        }

        [Test]
        public void OpenSolution_with_invalid_xml_makes_empty_library()
        {
            _storageAdapter.ThrowBadXmlException = true;
            Horace.Hear_SolutionOpened( this, Get_testfile_FileEventArgs() );

            Assert.AreEqual( 0, _ruleCatalog._rules.Count );
        }

        [Test]
        public void OpenSolution_with_invalid_xml_sends_bad_library_message()
        {
            _storageAdapter.ThrowBadXmlException = true;
            Horace.Hear_SolutionOpened( this, Get_testfile_FileEventArgs() );

            //  FIX:  Subscribe to the message, listener sets a val, Assert checks it
            //Assert.That( _userAdapter.SentBadLibraryMessage );
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
        public void Exception_unfound_library_upgraded_message()
        {
            var mockStorageAdapter = new MockStorageAdapter();
            mockStorageAdapter.LoadLibrary_Throw( new IOException( "This is nonsense." ) );

            var librarian = new ProjectLibrarian( mockStorageAdapter, new EventSwitchboard() );

            var ex = Assert.Throws<IOException>( () => librarian.OpenLibrary( "C:\\hither\\this.library" ) );
            Assert.That( ex.Message.Contains( "C:\\hither\\this.library" ) );
        }


        [Test]
        public void OpenFile_will_Raise_TaskListChanged_Event()
        {
            _switchboard.Event_TaskListChanged += Listen_for_TasksChanged_Event;
            Assert.That( _tasks, Is.Null );

            Horace.OpenSourceFile( "foo.cs", "//hello, world!" );

            Assert.That( _tasks, Is.Not.Null );
        }

        [Test]
        public void OpenFile_will_Add_new_FileMatch_to_Tasks()
        {
            _switchboard.Event_TaskListChanged += Listen_for_TasksChanged_Event;
            Rule allCSharpMustGo = new Rule
            {
                Description = "We hate CPound.", 
                Subquery = new QueryLanguageNode { Language = FileLanguage.CSharp } 
            };
            Horace._ruleCatalog.Add( allCSharpMustGo );
            Assert.That( _tasks, Is.Null );

            Horace.OpenSourceFile( "foo.cs", "//hello, world!" );

            Assert.That( _tasks.Count, Is.EqualTo( 1 ) );
        }


        private List<Task> _tasks;
        private void Listen_for_TasksChanged_Event( object caller, TasksEventArgs e )
        {
            _tasks = e.Tasks;
        }
    }
}
