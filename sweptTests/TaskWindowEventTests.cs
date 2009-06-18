using System;
using System.Collections.Generic;
using System.Text;
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
        public void when_Task_Completion_changed_Library_has_an_unsaved_change()
        {
            ProjectLibrarian librarian = starter.Librarian;
            TaskWindow window = starter.TaskWindow;

            Assert.IsFalse(librarian.ChangeNeedsPersisting);

            window.RaiseTaskCompletionChanged();

            Assert.IsTrue(librarian.ChangeNeedsPersisting);
        }

        [Test]
        public void when_TaskWindow_toggled_visibility_changed()
        {
            starter.TaskWindow.Visible = false;
            starter.TaskWindow.RaiseTaskWindowToggled();

            Assert.IsTrue(starter.TaskWindow.Visible);
        }
    }
}
