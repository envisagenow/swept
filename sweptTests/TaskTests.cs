//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using NUnit.Framework;
using System.Collections.Generic;

namespace swept.Tests
{
    [TestFixture]
    public class TaskTests
    {
        private const string _Description = "this change";
        Change _change;
        List<Task> _tasks;
        Task _task;
        SeeAlso _see;

        [SetUp]
        public void given_a_Task_created_from_a_Change()
        {
            _change = new Change { ID = "id1", Description = _Description, Language = FileLanguage.CSharp };
            _see = new SeeAlso { Description = "Look here", Target = "here.com", TargetType = TargetType.URL };
            _change.SeeAlsos.Add( _see );
            _tasks = Task.FromChange( _change );
            _task = _tasks[0];
        }

        [Test]
        public void attributes_filled_from_Change()
        {
            Assert.AreEqual( _change.ID, _task.ID );
            Assert.AreEqual( _change.Description, _task.Description );
        }

        [Test]
        public void Task_incomplete_by_default()
        {
            Assert.IsFalse( _task.Completed );
        }

        [Test]
        public void ToString_shows_description_for_GUI()
        {
            Assert.That( _task.ToString(), Is.EqualTo( _Description ) );
        }

        [Test]
        public void SeeAlsos_are_copied()
        {
            Assert.That( _task.SeeAlsos.Count, Is.EqualTo( 1 ) );
            var see = _task.SeeAlsos[0];
            Assert.That( see, Is.EqualTo( _see ) );
        }

        [Test]
        public void FromChange_gets_a_task_per_match_location()
        {
            _change._matchList = new List<int> { 4, 8, 9 };
            List<Task> tasks = Task.FromChange( _change );
            Assert.That( tasks.Count, Is.EqualTo( 3 ) );
        }
    }
}
