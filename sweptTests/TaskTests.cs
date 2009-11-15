//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using NUnit.Framework;

namespace swept.Tests
{
    [TestFixture]
    public class TaskTests
    {
        private const string _Description = "this change";
        Change _change;
        Task _task;

        [SetUp]
        public void given_a_Task_created_from_a_Change()
        {
            _change = new Change { ID = "id1", Description = _Description, Language = FileLanguage.CSharp };
            _task = Task.FromChange( _change );
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
    }
}
