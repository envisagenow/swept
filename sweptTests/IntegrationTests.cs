﻿//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using System;
using NUnit.Framework;

namespace swept.Tests
{
    [TestFixture]
    public class IntegrationTests
    {
        private Starter _starter;
        private StudioAdapter _adapter;
        private ChangeWindow _changes;
        private ProjectLibrarian _librarian;
        private TaskWindow _tasks;

        [SetUp]
        public void StartUp()
        {
            _starter = new Starter();
            _starter.Start();

            _librarian = _starter.Librarian;
            _librarian.SolutionPath = @"c:\code\path\to.sln";
            
            _tasks = _starter.TaskWindow;
            _adapter = _starter.Adapter;
            _changes = _starter.ChangeWindow;

            TestPreparer preparer = new TestPreparer();
            preparer.ShiftStarterToMocks( _starter );
        }

        [Test]
        public void when_Task_Completion_changed_Library_has_an_unsaved_change()
        {
            _changes.Raise_ChangeAdded( new Change( "100", "test change", FileLanguage.CSharp ) );
            _adapter.Raise_FileGotFocus( "foo.cs" );
            _adapter.Raise_SolutionSaved();

            Assert.IsTrue( _librarian.IsSaved );

            _tasks.ToggleTaskCompletion( 0 );

            Assert.IsFalse( _librarian.IsSaved );
        }

    }
}
