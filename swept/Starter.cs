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

            //FUTURE:  Subscribe the dispatcher to the VS IDE events

            //FUTURE:  Subscribe the dispatcher to events from the TaskWindow
            //FUTURE:  Subscribe the dispatcher to events from the ChangeWindow

            //  Subscribe the librarian and windows to the dispatcher's events
            Dispatcher.RaiseNonSourceGotFocus += taskWindow.NoSourceFile;
            Dispatcher.RaiseFileGotFocus += taskWindow.ChangeFile;
            Dispatcher.RaiseFileSaved += Librarian.SaveFile;
            Dispatcher.RaiseFilePasted += Librarian.PasteFile;
            Dispatcher.RaiseFileSavedAs += Librarian.SaveFileAs;
            Dispatcher.RaiseFileChangesAbandoned += Librarian.RevertFile;
            Dispatcher.RaiseFileDeleted += Librarian.DeleteFile;
            Dispatcher.RaiseTaskWindowToggled += taskWindow.ToggleVisibility;
            Dispatcher.RaiseFileRenamed += Librarian.RenameFile;
            Dispatcher.RaiseChangeAdded += Librarian.AddChange;

            Dispatcher.changeWindow = new ChangeWindow();
            Dispatcher.changeWindow.Changes = Librarian.changeCatalog;
        }
    }
}
