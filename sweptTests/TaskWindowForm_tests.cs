using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using swept.Addin;
using NUnit.Framework.SyntaxHelpers;

namespace swept.Tests
{
    [TestFixture]
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
            Assert.That( _form.tasks.Items.Count, Is.EqualTo( 0 ) );
        }

        [Test]
        public void When_Reset_to_empty_list()
        {
            _form.Hear_TaskListReset( new List<Task>(), null );
            Assert.That( _form.tasks.Items.Count, Is.EqualTo( 0 ) );
        }


        [Test]
        public void When_set_to_populated_list()
        {
            List<Task> tasks;

            for( int i = 0; i < 100; i++ )
            {
                tasks = GetSlightlyRandomTasks();
                _form.Hear_TaskListReset( tasks, null );
                Assert.That( _form.tasks.Items.Count, Is.EqualTo( 5 ) );

                Assert.That( _form.tasks.GetItemChecked( 0 ), Is.EqualTo( tasks[0].Completed ) );
                Assert.That( _form.tasks.GetItemChecked( 1 ), Is.EqualTo( tasks[1].Completed ) );
                Assert.That( _form.tasks.GetItemChecked( 2 ), Is.EqualTo( tasks[2].Completed ) );
                Assert.That( _form.tasks.GetItemChecked( 3 ), Is.EqualTo( tasks[3].Completed ) );
                Assert.That( _form.tasks.GetItemChecked( 4 ), Is.EqualTo( tasks[4].Completed ) );
            }
        }

        private List<Task> GetSlightlyRandomTasks()
        {
            List<Task> tasks = new List<Task>();

            Random r = new Random();

            tasks.Add( slightlyRandomTask( "007", "Bond, James Bond.", r ) );
            tasks.Add( slightlyRandomTask( "06525", "New Haven, CT", r ) );
            tasks.Add( slightlyRandomTask( "43126", "Harrisburg, OH", r ) );
            tasks.Add( slightlyRandomTask( "78562", "La Villa, TX", r ) );
            tasks.Add( slightlyRandomTask( "93442", "Morro Bay, CA", r ) );

            return tasks;
        }

        private Task slightlyRandomTask( string id, string description, Random r )
        {
            return new Task
            {
                ID = id,
                Description = description,
                Completed = r.Next( 0, 100 ) % 2 == 1,
                Language = FileLanguage.CSharp,
            };
        }
    }
}
