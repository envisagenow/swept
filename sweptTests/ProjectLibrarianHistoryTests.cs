//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using NUnit.Framework;
using swept;
using System;

namespace swept.Tests
{
    [TestFixture]
    public class ProjectLibrarianHistoryTests
    {
        string _testingSolutionPath;
        ProjectLibrarian Horace;
        MockStorageAdapter _storageAdapter;

        Change historicalChange;
        SourceFile foo;
        MockUserAdapter talker;


        [SetUp]
        public void given_a_deleted_change()
        {
            _testingSolutionPath = @"f:\over\here.sln";
            Horace = new ProjectLibrarian { SolutionPath = _testingSolutionPath };

            _storageAdapter = new MockStorageAdapter();
            Horace._storageAdapter = _storageAdapter;
            Horace._userAdapter = new MockUserAdapter();


            historicalChange = new Change { ID = "14" };
            Horace.Hear_ChangeAdded( this, new ChangeEventArgs { Change = historicalChange } );
            foo = new SourceFile( "foo.cs" ) { Language = FileLanguage.CSharp };
            Horace._sourceCatalog.Add( foo );
            foo.Completions.Add( new Completion( "14" ) );

            Horace._changeCatalog.Remove( "14" );

            talker = new MockUserAdapter();
            Horace._userAdapter = talker;
        }

        [Test]
        public void AddChange_can_keep_historical_Completions()
        {
            //  In this case, the user chooses to keep history.
            talker.KeepHistoricalResponse = true;

            Horace.Hear_ChangeAdded( this, new ChangeEventArgs { Change = historicalChange } );

            Assert.AreEqual( 1, foo.Completions.Count );
        }

        [Test]
        public void AddChange_can_discard_historical_Completions()
        {
            //  In this case, the user chooses to discard history.
            talker.KeepHistoricalResponse = false;

            Horace.Hear_ChangeAdded( this, new ChangeEventArgs { Change = historicalChange } );

            Assert.AreEqual( 0, foo.Completions.Count );
        }


    }
}
