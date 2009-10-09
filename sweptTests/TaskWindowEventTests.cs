//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
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
    }
}