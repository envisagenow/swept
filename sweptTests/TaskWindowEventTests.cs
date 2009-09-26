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