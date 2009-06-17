using System;
using System.Collections.Generic;
using System.Text;

namespace swept
{
    public class Starter
    {
        public TaskWindow TaskWindow { get; set; }
        public EventDispatcher Dispatcher { get; set; }
        public ProjectLibrarian Librarian { get; private set; }
        public ChangeWindow ChangeWindow { get; set; }

        internal void Start()
        {
            Dispatcher = new EventDispatcher();
            Librarian = new ProjectLibrarian();
            Dispatcher.Librarian = Librarian;

            TaskWindow = new TaskWindow();
            TaskWindow.librarian = Librarian;
            Dispatcher.taskWindow = TaskWindow;

            ChangeWindow = new ChangeWindow();
            Dispatcher.changeWindow = ChangeWindow;
            ChangeWindow.Changes = Librarian.changeCatalog;

            //FUTURE:  Subscribe the dispatcher to the VS IDE events

            //FUTURE:  Subscribe the dispatcher to events from the TaskWindow

            //  Subscribe the librarian and windows to the dispatcher's events
            Dispatcher.RaiseNonSourceGotFocus += TaskWindow.NoSourceFile;
            Dispatcher.RaiseFileGotFocus += TaskWindow.ChangeFile;
            Dispatcher.RaiseFileSaved += Librarian.SaveFile;
            Dispatcher.RaiseFilePasted += Librarian.PasteFile;
            Dispatcher.RaiseFileSavedAs += Librarian.SaveFileAs;
            Dispatcher.RaiseFileChangesAbandoned += Librarian.RevertFile;
            Dispatcher.RaiseFileDeleted += Librarian.DeleteFile;
            Dispatcher.RaiseTaskWindowToggled += TaskWindow.ToggleVisibility;
            Dispatcher.RaiseFileRenamed += Librarian.RenameFile;
            Dispatcher.RaiseChangeAdded += Librarian.AddChange;


            //  Subscribe the dispatcher to events from the ChangeWindow
            ChangeWindow.RaiseChangesUpdated += Dispatcher.ChangesUpdated;





        }
    }
}
