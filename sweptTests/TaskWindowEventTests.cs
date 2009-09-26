//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using System;
using NUnit.Framework;
using swept;

namespace swept.Tests
{
    [TestFixture]
    public class TaskWindowEventTests
    {
        private Starter starter;

        [SetUp]
        public void StartUp()
        {
            starter = new Starter();
            starter.Start();
        }

        [Test]
        public void when_TaskWindow_toggled_visibility_changed()
        {
            TaskWindow tasks = starter.TaskWindow;
            tasks.Visible = false;
            tasks.Raise_TaskWindowToggled();

            Assert.IsTrue( tasks.Visible );
        }
    }
}