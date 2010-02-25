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
    public class TaskWindowEventTests
    {
        private TestPreparer _preparer;
        private Starter _starter;
        private TaskWindow _window;

        [SetUp]
        public void StartUp()
        {
            _starter = new Starter();

            _preparer = new TestPreparer();

            _starter.Start();
            _window = _starter.TaskWindow;
        }

        [Test]
        public void when_TaskWindow_hears_SeekingTaskLocation_the_UserAdapter_gets_the_task()
        {
            //unfinished
            _preparer.ShiftSweptToMocks( _starter );
            MockUserAdapter userAdapter = _preparer.MockGUI;

            Task task = new Task
            {
                Children = null,
                Description = "hi!",
            };
            TaskEventArgs args = new TaskEventArgs { Task = task };

            _window.Hear_TaskChosen( this, args );

            Assert.That( userAdapter.DoubleClickedTask, Is.SameAs( task ) );
        }

        [Test]
        public void when_TaskWindow_toggled_visibility_changed()
        {
            _window.Visible = false;
            _window.Raise_TaskWindowToggled();

            Assert.IsTrue( _window.Visible );
        }

        [Test]
        public void when_TaskChecked_completion_is_set()
        {
            Task task = new Task { ID = "x", Description = "I get checked." };
            _window.Tasks.Add( task );
            Assert.IsFalse( _window.Tasks[0].Completed );

            _window.Hear_TaskCheck( this, new TaskEventArgs{ Task = task, Checked = true } );
            Assert.IsTrue( _window.Tasks[0].Completed );
        }

        [Test]
        public void when_SeeAlsoFollowed_GUI_shows_it()
        {
            _preparer.ShiftSweptToMocks( _starter );

            SeeAlso search = new SeeAlso { Description = "Search", Target = "www.google.com", TargetType = TargetType.URL };

            _window.Follow( search );

            Assert.That( _preparer.MockGUI.SentSeeAlso, Is.SameAs( search ) );
        }
    }
}