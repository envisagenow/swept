//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using swept;

namespace swept.Tests
{
    [TestFixture]
    public class TaskWindowTests
    {
        private SourceFile file;
        private List<Change> changes;
        private TaskWindow window;

        [SetUp]
        public void CanCreate()
        {
            window = new TaskWindow();

            changes = new List<Change>();
            changes.Add( new Change( "id1", "sticky", FileLanguage.CSharp ) );
            changes.Add( new Change( "id2", "mop-up", FileLanguage.CSharp ) );

            file = new SourceFile( "glue.cs" );
            file.Completions.Add( new Completion( "id1" ) );

            window.ChangeFile( file, changes );
        }

        [Test]
        public void ChangeFile_SetsFileDetails()
        {
            Assert.AreSame( file, window.File );
            Assert.AreEqual( file.Name, window.Title );
        }

        [Test]
        public void ChangeFile_SetsEntries()
        {
            Assert.AreEqual( 2, window.Tasks.Count );
            Assert.IsTrue( window.Tasks[0].Completed );
            Assert.IsFalse( window.Tasks[1].Completed );
        }

        [Test]
        public void ChangeFile_NullFileBehaves()
        {
            window.ChangeFile( null, changes );
            Assert.AreEqual( "No source file", window.Title );
            Assert.AreEqual( 0, window.Tasks.Count );
        }

        [Test]
        public void ChangeFile_NullChangesBehaves()
        {
            window.ChangeFile( file, null );
            Assert.AreEqual( 0, window.Tasks.Count );
        }

        [Test]
        public void EntryClick_TogglesCompletion()
        {
            //  We should start with 0 completed, 1 not
            Assert.IsTrue( window.Tasks[0].Completed );
            Assert.IsFalse( window.Tasks[1].Completed );

            //  Clicking 0 should clear 0, leave 1
            window.ClickEntry( 0 );
            Assert.IsFalse( window.Tasks[0].Completed );
            Assert.IsFalse( window.Tasks[1].Completed );

            //  Clicking 0 again should mark it completed, again
            window.ClickEntry( 0 );
            Assert.IsTrue( window.Tasks[0].Completed );
            Assert.IsFalse( window.Tasks[1].Completed );

            //  Clicking 1 should leave 0
            window.ClickEntry( 1 );
            Assert.IsTrue( window.Tasks[0].Completed );
            Assert.IsTrue( window.Tasks[1].Completed );

            //  Clicking 1 should leave 0
            window.ClickEntry( 1 );
            Assert.IsTrue( window.Tasks[0].Completed );
            Assert.IsFalse( window.Tasks[1].Completed );
        }


    }
}
