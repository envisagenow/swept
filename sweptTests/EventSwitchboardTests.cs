//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2011 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using NUnit.Framework;
using swept;
using System;
using swept.DSL;

namespace swept.Tests
{
    [CoverageExclude]
    [TestFixture]
    public class EventSwitchboardTests
    {
        private Starter starter;
        private string fileName;
        private SourceFile file;
        private ChangeCatalog changeCat;

        private EventSwitchboard switchboard;
        private ProjectLibrarian librarian;

        [SetUp]
        public void SetUp()
        {
            starter = new Starter();
            starter.Start( new EventSwitchboard() );
            var preparer = new TestPreparer();
            preparer.ShiftSweptToMocks( starter );

            librarian = starter.Librarian;
            switchboard = starter.Switchboard;

            changeCat = librarian._changeCatalog;
            string indentID = "14";
            changeCat.Add( new Change { ID = indentID, Description = "indentation cleanup", Subquery = new QueryLanguageNode { Language = FileLanguage.CSharp } } );


            fileName = "bari.cs";
            file = new SourceFile( fileName );

            librarian.SolutionPath = @"d:\old_stuff\old.sln";
        }

        [Test]
        public void when_SolutionOpened_Librarian_gets_new_path()
        {
            string newPath = @"new\location";
            switchboard.Raise_SolutionOpened( newPath );
            Assert.AreEqual( newPath, switchboard.Librarian.SolutionPath );
        }
    }
}
