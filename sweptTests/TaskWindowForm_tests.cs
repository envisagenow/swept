﻿//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using NUnit.Framework;
using swept.Addin;

namespace swept.Tests
{
    [TestFixture]
    [CoverageExclude]
    public class TaskWindowForm_tests
    {
        TaskWindow_GUI _form;
        [SetUp]
        public void given()
        {
            _form = new TaskWindow_GUI();
        }

        [TearDown]
        public void clean_up_properly()
        {
            _form.Dispose();
        }

        [Test]
        public void task_list_begins_empty()
        {
            Assert.That( _form._taskGridView.RowCount, Is.EqualTo( 0 ) );
        }

        [Test]
        public void form_list_updated_on_Reset_events()
        {
            var change = new Change();
            List<swept.Task> tasks = new List<Task> { new Task( change, 1 ) };
            _form.Hear_TaskListReset( tasks, null );
            Assert.That( _form._taskGridView.RowCount, Is.EqualTo( 1 ) );

            tasks.Clear();
            _form.Hear_TaskListReset( tasks, null );
            Assert.That( _form._taskGridView.RowCount, Is.EqualTo( 0 ) );
        }
    }
}
