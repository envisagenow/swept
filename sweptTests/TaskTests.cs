//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using NUnit.Framework;
using System.Collections.Generic;

namespace swept.Tests
{
    [TestFixture]
    public class TaskTests
    {
        [Test]
        public void FromIssueSet_gets_one_task_on_matching_File_scope()
        {
            Clause clause = new Clause { Language = FileLanguage.CSharp };
            IssueSet set = new IssueSet( clause, new SourceFile( "foo.cs" ), ClauseMatchScope.File, new List<int> { 1 } );
            List<Task> tasks = Task.FromIssueSet( set );
            Assert.That( tasks.Count, Is.EqualTo( 1 ) );
            Assert.That( tasks[0].LineNumber, Is.EqualTo( 1 ) );
        }

        [Test]
        public void FromIssueSet_gets_no_tasks_on_unmatching_File_scope()
        {
            IssueSet set = new IssueSet( null, null, ClauseMatchScope.File, new List<int>() );
            List<Task> tasks = Task.FromIssueSet( set );
            Assert.That( tasks.Count, Is.EqualTo( 0 ) );
        }

        [Test]
        public void FromIssueSet_gets_one_Task_per_match_location()
        {
            IssueSet set = new IssueSet( null, null, ClauseMatchScope.Line, new List<int> { 4, 8, 9 } );

            List<Task> tasks = Task.FromIssueSet( set );

            Assert.That( tasks.Count, Is.EqualTo( 3 ) );
            Assert.That( tasks[0].LineNumber, Is.EqualTo( 4 ) );
            Assert.That( tasks[1].LineNumber, Is.EqualTo( 8 ) );
            Assert.That( tasks[2].LineNumber, Is.EqualTo( 9 ) );
        }

        [Test]
        public void FromIssueSet_gets_no_Tasks_when_does_not_match()
        {
            IssueSet set = new IssueSet( null, null, ClauseMatchScope.Line, new List<int>() );

            List<Task> tasks = Task.FromIssueSet( set );

            Assert.That( tasks.Count, Is.EqualTo( 0 ) );
        }





        //[Test]
        //public void ToString_shows_description_for_GUI()
        //{
        //    Assert.That( _task.ToString(), Is.EqualTo( _Description ) );
        //}

        //[Test]
        //public void SeeAlsos_are_copied()
        //{
        //    Assert.That( _task.SeeAlsos.Count, Is.EqualTo( 1 ) );
        //    var see = _task.SeeAlsos[0];
        //    Assert.That( see, Is.EqualTo( _see ) );
        //}

    }
}
