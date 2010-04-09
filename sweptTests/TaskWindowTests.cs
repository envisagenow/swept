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

            //  Using this entry point is handy for testing--lets us skip building a ChangeCatalog
            window.ShowFile( file, changes );
        }

        [Test]
        public void ShowFile_sets_CurrentFile_and_Title()
        {
            Assert.AreSame( file, window.CurrentFile );
            Assert.AreEqual( file.Name, window.Title );
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
        public void ShowFile_raises_TaskListReset_to_notify_TaskForm()
        {
            window.Event_TaskListReset += Hear_TaskListReset;

            _taskList_reset = false;
            window.ShowFile( file, new List<Change>() );

            Assert.That( _taskList_reset );
        }

        private bool _taskList_reset;
        private void Hear_TaskListReset( object objNewTasks, EventArgs e )
        {
            _taskList_reset = true;
        }
    }
}
