//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using System;
using System.Collections.Generic;
using System.Text;

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
            TaskWindow.ChangeCatalog = Librarian.changeCatalog;
            TaskWindow.FileCatalog = Librarian.sourceCatalog;

            Adapter.taskWindow = TaskWindow;

            ChangeWindow = new ChangeWindow();
            Adapter.changeWindow = ChangeWindow;
            ChangeWindow.ChangeCatalog = Librarian.changeCatalog;

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

            //  Subscribe to the TaskWindow's events
            TaskWindow.Event_TaskWindowToggled += TaskWindow.Hear_TaskWindowToggled;
            //  A self-subscription.  Odd, possibly poor practice...

            //  Subscribe to the ChangeWindow's events
            ChangeWindow.EventChangeAdded += Librarian.Hear_ChangeAdded;
            ChangeWindow.EventChangeListUpdated += TaskWindow.Hear_ChangeListUpdated;
            ChangeWindow.EventChangeListUpdated += Librarian.Hear_ChangeListUpdated;
        }
    }
}
