//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;

namespace swept
{
    public class Starter
    {
        public ProjectLibrarian Librarian { get; private set; }
        public StudioAdapter Adapter { get; set; }
        public ChangeWindow ChangeWindow { get; set; }
        public TaskWindow TaskWindow { get; set; }

        public void Start()
        {
            Adapter = new StudioAdapter();
            Librarian = new ProjectLibrarian();
            Adapter.Librarian = Librarian;

            TaskWindow = new TaskWindow();
            TaskWindow._changeCatalog = Librarian._changeCatalog;
            TaskWindow._fileCatalog = Librarian._sourceCatalog;

            Adapter.taskWindow = TaskWindow;

            ChangeWindow = new ChangeWindow();
            Adapter.changeWindow = ChangeWindow;
            ChangeWindow.ChangeCatalog = Librarian._changeCatalog;

            //  Subscribe to the StudioAdapter's events
            Adapter.Event_NonSourceGotFocus += TaskWindow.Hear_NonSourceGotFocus;
            Adapter.Event_FileGotFocus += TaskWindow.Hear_FileGotFocus;
            Adapter.Event_FileSaved += Librarian.Hear_FileSaved;
            Adapter.Event_FilePasted += Librarian.Hear_FilePasted;
            Adapter.Event_FileSavedAs += Librarian.Hear_FileSavedAs;
            Adapter.Event_FileChangesAbandoned += Librarian.Hear_FileChangesAbandoned;
            Adapter.Event_FileDeleted += Librarian.Hear_FileDeleted;
            Adapter.Event_FileRenamed += Librarian.Hear_FileRenamed;
            Adapter.Event_SolutionSaved += Librarian.Hear_SolutionSaved;
            Adapter.Event_SolutionOpened += Librarian.Hear_SolutionOpened;
            Adapter.Event_SolutionRenamed += Librarian.Hear_SolutionRenamed;

            //  Subscribe to the TaskWindow's events
            TaskWindow.Event_TaskWindowToggled += TaskWindow.Hear_TaskWindowToggled;
            //  A self-subscription.  Odd, possibly poor practice...

            //  Subscribe to the ChangeWindow's events
            ChangeWindow.Event_ChangeAdded += Librarian.Hear_ChangeAdded;
            ChangeWindow.Event_ChangeListUpdated += TaskWindow.Hear_ChangeCatalogUpdated;
            ChangeWindow.Event_ChangeListUpdated += Librarian.Hear_ChangeListUpdated;

            //  Subscribe to the Librarian's events
            Librarian.Event_ChangeCatalogUpdated += TaskWindow.Hear_ChangeCatalogUpdated;
            Librarian.Event_SourceCatalogUpdated += TaskWindow.Hear_SourceCatalogUpdated;
        }

        public void Stop()
        {
            //  Unsubscribe from the Librarian's events
            Librarian.Event_ChangeCatalogUpdated -= TaskWindow.Hear_ChangeCatalogUpdated;

            //  Unsubscribe from the ChangeWindow's events
            ChangeWindow.Event_ChangeAdded -= Librarian.Hear_ChangeAdded;
            ChangeWindow.Event_ChangeListUpdated -= TaskWindow.Hear_ChangeCatalogUpdated;
            ChangeWindow.Event_ChangeListUpdated -= Librarian.Hear_ChangeListUpdated;

            //  Unsubscribe from the TaskWindow's events
            TaskWindow.Event_TaskWindowToggled -= TaskWindow.Hear_TaskWindowToggled;
            //  A self-subscription.  Odd, possibly poor practice...

            //  Unsubscribe from the StudioAdapter's events
            Adapter.Event_NonSourceGotFocus -= TaskWindow.Hear_NonSourceGotFocus;
            Adapter.Event_FileGotFocus -= TaskWindow.Hear_FileGotFocus;
            Adapter.Event_FileSaved -= Librarian.Hear_FileSaved;
            Adapter.Event_FilePasted -= Librarian.Hear_FilePasted;
            Adapter.Event_FileSavedAs -= Librarian.Hear_FileSavedAs;
            Adapter.Event_FileChangesAbandoned -= Librarian.Hear_FileChangesAbandoned;
            Adapter.Event_FileDeleted -= Librarian.Hear_FileDeleted;
            Adapter.Event_FileRenamed -= Librarian.Hear_FileRenamed;
            Adapter.Event_SolutionSaved -= Librarian.Hear_SolutionSaved;
            Adapter.Event_SolutionOpened -= Librarian.Hear_SolutionOpened;

            Adapter = null;
            Librarian = null;
            TaskWindow = null;
            ChangeWindow = null;
        }

    }
}
