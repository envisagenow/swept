//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;

namespace swept
{
    public class Starter
    {
        public ProjectLibrarian Librarian { get; private set; }

        public IStorageAdapter StorageAdapter { get; set; }
        public ChangeCatalog ChangeCatalog { get; set; }
        public StudioAdapter StudioAdapter { get; set; }
        public TaskWindow TaskWindow { get; set; }

        public void Start()
        {
            StudioAdapter = new StudioAdapter();
            StorageAdapter = new StorageAdapter();
            ChangeCatalog = new ChangeCatalog();

            Librarian = new ProjectLibrarian( StorageAdapter, ChangeCatalog );
            StudioAdapter.Librarian = Librarian;

            TaskWindow = new TaskWindow();
            TaskWindow._changeCatalog = ChangeCatalog;
            TaskWindow._fileCatalog = Librarian._sourceCatalog;

            StudioAdapter.taskWindow = TaskWindow;

            //  Subscribe to the StudioAdapter's events
            StudioAdapter.Event_NonSourceGotFocus += TaskWindow.Hear_NonSourceGotFocus;
            StudioAdapter.Event_FileGotFocus += TaskWindow.Hear_FileGotFocus;
            StudioAdapter.Event_SolutionOpened += Librarian.Hear_SolutionOpened;
            StudioAdapter.Event_SolutionRenamed += Librarian.Hear_SolutionRenamed;

            //  Subscribe to the TaskWindow's events
            //  A self-subscription.  Odd, possibly poor practice...
            TaskWindow.Event_TaskWindowToggled += TaskWindow.Hear_TaskWindowToggled;

            // TODO: finish!
            //TaskWindow.Event_TaskLocationSought

            //  Subscribe to the Librarian's events
            Librarian.Event_ChangeCatalogUpdated += TaskWindow.Hear_ChangeCatalogUpdated;
            Librarian.Event_SourceCatalogUpdated += TaskWindow.Hear_SourceCatalogUpdated;
        }

        public void Stop()
        {
            //  Unsubscribe from the Librarian's events
            Librarian.Event_SourceCatalogUpdated -= TaskWindow.Hear_SourceCatalogUpdated;
            Librarian.Event_ChangeCatalogUpdated -= TaskWindow.Hear_ChangeCatalogUpdated;

            //  Unsubscribe from the TaskWindow's events
            //  A self-subscription.  Odd, possibly poor practice...
            TaskWindow.Event_TaskWindowToggled -= TaskWindow.Hear_TaskWindowToggled;

            //  Unsubscribe from the StudioAdapter's events
            StudioAdapter.Event_NonSourceGotFocus -= TaskWindow.Hear_NonSourceGotFocus;
            StudioAdapter.Event_FileGotFocus -= TaskWindow.Hear_FileGotFocus;
            StudioAdapter.Event_SolutionOpened -= Librarian.Hear_SolutionOpened;

            StudioAdapter = null;
            Librarian = null;
            TaskWindow = null;
        }

    }
}
