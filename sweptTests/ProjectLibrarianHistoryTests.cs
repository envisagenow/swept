using NUnit.Framework;
using swept;
using System;
using System.Xml;

namespace swept.Tests
{
    [TestFixture]
    public class ProjectLibrarianHistoryTests
    {
        string _testingSolutionPath;
        ProjectLibrarian Horace;
        MockFSAdapter _FSAdapter;

        Change historicalChange;
        SourceFile foo;
        MockGUIAdapter talker;


        [SetUp]
        public void given_a_deleted_change()
        {
            _testingSolutionPath = @"f:\over\here.sln";
            Horace = new ProjectLibrarian { SolutionPath = _testingSolutionPath };

            _FSAdapter = new MockFSAdapter();
            Horace._FSAdapter = _FSAdapter;
            Horace._GUIAdapter = new MockGUIAdapter();


            historicalChange = new Change( "14", "here I am", FileLanguage.CSharp );
            Horace.Hear_ChangeAdded( this, new ChangeEventArgs { change = historicalChange } );
            foo = new SourceFile( "foo.cs" ) { Language = FileLanguage.CSharp };
            Horace._sourceCatalog.Add( foo );
            foo.Completions.Add( new Completion( "14" ) );

            Horace._changeCatalog.Remove( "14" );

            talker = new MockGUIAdapter();
            Horace._GUIAdapter = talker;

        }


        [Test]
        public void AddChange_can_keep_historical_Completions()
        {
            //  In this case, the user chooses to keep history.
            talker.KeepHistoricalResponse = true;

            Horace.Hear_ChangeAdded( this, new ChangeEventArgs { change = historicalChange } );

            Assert.AreEqual( 1, foo.Completions.Count );
        }

        [Test]
        public void AddChange_can_discard_historical_Completions()
        {
            //  In this case, the user chooses to discard history.
            talker.KeepHistoricalResponse = false;

            Horace.Hear_ChangeAdded( this, new ChangeEventArgs { change = historicalChange } );

            Assert.AreEqual( 0, foo.Completions.Count );
        }


    }
}