//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using NUnit.Framework;
using swept;

namespace swept.Tests
{

    [TestFixture]
    public class TaskWindowTests
    {
        private SourceFile file;
        private List<Change> changes;
        private TaskWindow window;

        [SetUp]
        public void CanShowFile()
        {
            window = new TaskWindow();

            changes = new List<Change>();
            changes.Add( new Change{ ID = "id1", Description = "sticky", Language = FileLanguage.CSharp } );
            changes.Add( new Change { ID = "id2", Description = "mop-up", Language = FileLanguage.CSharp } );

            file = new SourceFile( "glue.cs" );
            file.Completions.Add( new Completion( "id1" ) );

            //  Using this entry point is handy for testing--lets us skip building a ChangeCatalog
            window.ShowFile( file, changes );
        }
        // TODO: 0.2 Have "Manually Uncheckable" in the check box list.  Intercept the click event?

        [Test]
        public void ShowFile_sets_CurrentFile_and_Title()
        {
            Assert.AreSame( file, window.CurrentFile );
            Assert.AreEqual( file.Name, window.Title );
        }

        [Test]
        public void ShowFile_checkmarks_Tasks_to_match_File_Completions()
        {
            Assert.AreEqual( 2, window.Tasks.Count );
            Assert.IsTrue( window.Tasks[0].Completed );
            Assert.IsFalse( window.Tasks[1].Completed );
        }

        [Test]
        public void ShowFile_with_no_file_may_still_show_Tasks()
        {
            //  No file and some changes isn't a current use case--but it "works".
            window.ShowFile( null, changes );
            Assert.AreEqual( "No source file", window.Title );
            Assert.AreEqual( 2, window.Tasks.Count );
        }

        [Test]
        public void ShowFile_with_no_changes_makes_empty_TaskList()
        {
            window.ShowFile( file, new List<Change>() );
            Assert.AreEqual( 0, window.Tasks.Count );
        }

        [Test]
        public void clicking_a_Task_toggles_its_Completion()
        {
            //  We start with 0 completed, 1 not
            Assert.IsTrue(window.Tasks[0].Completed);
            Assert.IsFalse(window.Tasks[1].Completed);

            //  Clicking 0 should clear 0, and not affect 1
            window.ToggleTaskCompletion(0);
            Assert.IsFalse(window.Tasks[0].Completed);
            Assert.IsFalse(window.Tasks[1].Completed);

            //  Clicking 0 again should complete 0, and not affect 1
            window.ToggleTaskCompletion(0);
            Assert.IsTrue(window.Tasks[0].Completed);
            Assert.IsFalse(window.Tasks[1].Completed);

            //  Clicking 1 should complete 1, and not affect 0
            window.ToggleTaskCompletion(1);
            Assert.IsTrue(window.Tasks[0].Completed);
            Assert.IsTrue(window.Tasks[1].Completed);

            //  Clicking 1 again should clear 1, and not affect 0
            window.ToggleTaskCompletion(1);
            Assert.IsTrue(window.Tasks[0].Completed);
            Assert.IsFalse(window.Tasks[1].Completed);
        }

        [Test]
        public void clicking_a_Task_alters_CurrentFile_Completions()
        {
            //  We start with 0 completed, 1 not
            Assert.AreEqual( 1, file.Completions.Count );
            Assert.AreEqual("id1", file.Completions[0].ChangeID);

            //  Clicking 0 should clear 0, and not affect 1
            window.ToggleTaskCompletion(0);
            Assert.AreEqual(0, file.Completions.Count);

            //  Clicking 0 again should complete 0, and not affect 1
            window.ToggleTaskCompletion(0);
            Assert.AreEqual(1, file.Completions.Count);
            Assert.AreEqual("id1", file.Completions[0].ChangeID);

            //  Clicking 1 should complete 1, and not affect 0
            window.ToggleTaskCompletion(1);
            Assert.AreEqual(2, file.Completions.Count);
            Assert.AreEqual("id1", file.Completions[0].ChangeID);
            Assert.AreEqual("id2", file.Completions[1].ChangeID);

            //  Clicking 1 again should clear 1, and not affect 0
            window.ToggleTaskCompletion(1);
            Assert.AreEqual(1, file.Completions.Count);
            Assert.AreEqual("id1", file.Completions[0].ChangeID);
        }
    }
}
