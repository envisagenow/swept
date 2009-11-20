//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using NUnit.Framework;
using swept;

namespace swept.Tests
{
    [TestFixture]
    public class TaskWindowEventTests
    {
        private Starter _starter;
        private TaskWindow _tasks;

        [SetUp]
        public void StartUp()
        {
            _starter = new Starter();
            _starter.Start();
            _tasks = _starter.TaskWindow;
        }

        [Test]
        public void when_TaskWindow_toggled_visibility_changed()
        {
            _tasks.Visible = false;
            _tasks.Raise_TaskWindowToggled();

            Assert.IsTrue( _tasks.Visible );
        }

        [Test]
        public void when_TaskChecked_completion_is_set()
        {
            Task task = new Task { ID = "x", Description = "I get checked." };
            _tasks.Tasks.Add( task );
            Assert.IsFalse( _tasks.Tasks[0].Completed );

            _tasks.Hear_TaskCheck( this, new TaskEventArgs{ Task = task, Checked = true } );
            Assert.IsTrue( _tasks.Tasks[0].Completed );
        }

        [Test]
        public void when_SeeAlsoChosen_GUI_shows_it()
        {
            Task task = new Task { ID = "1", Description = "First things first.", /*SeeAlsos*/ };
            _tasks.Tasks.Add( task );

        }
    }
}