using System;
using System.Collections.Generic;
using System.Text;

namespace swept
{
    public class Starter
    {
        public EventDispatcher Dispatcher { get; set; }

        public ProjectLibrarian Librarian { get; private set; }

        internal void Start()
        {
            Dispatcher = new EventDispatcher();
            Librarian = new ProjectLibrarian();
            Dispatcher.Librarian = Librarian;

            TaskWindow taskWindow = new TaskWindow();
            taskWindow.librarian = Librarian;
            Dispatcher.taskWindow = taskWindow;
            Dispatcher.RaiseNonSourceGotFocus += taskWindow.NoSourceFile;
            Dispatcher.RaiseFileGotFocus += taskWindow.ChangeFile;
            Dispatcher.RaiseFileSaved += Librarian.SaveFile;
            Dispatcher.RaiseFilePasted += Librarian.PasteFile;
            Dispatcher.RaiseFileSavedAs += Librarian.SaveFileAs;

            Dispatcher.changeWindow = new ChangeWindow();
            Dispatcher.changeWindow.Changes = Librarian.changeCatalog;
        }
    }
}
