//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2011 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using NUnit.Framework;
using System.Collections.Generic;

namespace swept.Tests
{
    [CoverageExclude]
    [TestFixture]
    public class TaskTests
    {
        [Test]
        public void FromMatch_gets_one_task_on_matching_File_scope()
        {
            var match = new FileMatch( true );  //  This file matches
            List<Task> tasks = Task.FromMatch( match, null, null );
            Assert.That( tasks.Count, Is.EqualTo( 1 ) );
            Assert.That( tasks[0].LineNumber, Is.EqualTo( 1 ) );
        }

        [Test]
        public void FromIssueSet_gets_one_Task_per_match_location()
        {
            var match = new LineMatch( new List<int> { 4, 8, 9 } );

            List<Task> tasks = Task.FromMatch( match, null, null );

            Assert.That( tasks.Count, Is.EqualTo( 3 ) );
            Assert.That( tasks[0].LineNumber, Is.EqualTo( 4 ) );
            Assert.That( tasks[1].LineNumber, Is.EqualTo( 8 ) );
            Assert.That( tasks[2].LineNumber, Is.EqualTo( 9 ) );
        }

        [Test]
        public void FromMatch_gets_no_tasks_from_LineMatch_with_no_lines()
        {
            var match = new LineMatch( new List<int>() );
            List<Task> tasks = Task.FromMatch( match, null, null );
            Assert.That( tasks.Count, Is.EqualTo( 0 ) );
        }

        [Test]
        public void FromMatch_gets_no_Tasks_from_FileMatch_with_DoesMatch_false()
        {
            var match = new FileMatch(false);
            List<Task> tasks = Task.FromMatch( match, null, null );
            Assert.That( tasks.Count, Is.EqualTo( 0 ) );
        }

        [Test]
        public void ToString_shows_description_for_GUI()
        {
            var match = new FileMatch( true );
            string description = "XML must have correct namespace.";
            var change = new Change { Description = description };

            List<Task> tasks = Task.FromMatch( match, change, null );
            Assert.That( tasks.Count, Is.EqualTo( 1 ) );

            var task = tasks[0];
            Assert.That( task.ToString(), Is.EqualTo( description ) );
        }

        //  So FromMatch we don't have SeeAlsos--which live on the Change.
        //  We can pass SeeAlsos as we now pass description.
        //  We can pass Change, then copy desc and SAs
        //  We can pass Change, and keep ref to change, and delegate values from change

        //[Test]
        //public void SeeAlsos_are_copied()
        //{
        //    Assert.That( _task.SeeAlsos.Count, Is.EqualTo( 1 ) );
        //    var see = _task.SeeAlsos[0];
        //    Assert.That( see, Is.EqualTo( _see ) );
        //}

    }
}
