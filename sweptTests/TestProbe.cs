//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using NUnit.Framework;
using System.Collections.Generic;

namespace swept.Tests
{
    public class TestProbe
    {
        public static string SingleFileLibrary_text
        {
            get
            {
                return
@"<SweptProjectData>
<ChangeCatalog>
</ChangeCatalog>
<SourceFileCatalog>
    <SourceFile Name='some_file.cs'>
    </SourceFile>
</SourceFileCatalog>
</SweptProjectData>";
            }
        }


        public static string SingleChangeLibrary_text
        {
            get
            {
                return
@"<SweptProjectData>
<ChangeCatalog>
    <Change ID='30-Persist' Description='Update to use persister' Language='CSharp' />
</ChangeCatalog>
<SourceFileCatalog>
</SourceFileCatalog>
</SweptProjectData>";
            }
        }


        public static string SeveralFileCatalog_text
        {
            get
            {
                return
@"<SweptProjectData>
<SourceFileCatalog>
    <SourceFile Name='bar.cs'>
        <Completion ID='AB1' />
        <Completion ID='AB2' />
    </SourceFile>
    <SourceFile Name='foo.cs'>
        <Completion ID='anotherID' />
    </SourceFile>
</SourceFileCatalog>
</SweptProjectData>";
            }
        }

        [Test]
        public void When_set_to_populated_list()
        {
            List<Task> tasks;

            for (int i = 0; i < 100; i++)
            {
                tasks = GetSlightlyRandomTasks();
                //_form.Hear_TaskListReset( tasks, null );
                //Assert.That( _form.tasks.Items.Count, Is.EqualTo( 5 ) );

                //Assert.That( _form.tasks.GetItemChecked( 0 ), Is.EqualTo( tasks[0].Completed ) );
                //Assert.That( _form.tasks.GetItemChecked( 1 ), Is.EqualTo( tasks[1].Completed ) );
                //Assert.That( _form.tasks.GetItemChecked( 2 ), Is.EqualTo( tasks[2].Completed ) );
                //Assert.That( _form.tasks.GetItemChecked( 3 ), Is.EqualTo( tasks[3].Completed ) );
                //Assert.That( _form.tasks.GetItemChecked( 4 ), Is.EqualTo( tasks[4].Completed ) );
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
                Language = FileLanguage.CSharp,
            };
        }

    }
}
