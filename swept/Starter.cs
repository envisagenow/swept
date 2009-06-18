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

        internal void Start()
        {
            Adapter = new StudioAdapter();
            Librarian = new ProjectLibrarian();
            Adapter.Librarian = Librarian;

            TaskWindow = new TaskWindow();
            TaskWindow.ChangeCatalog = Librarian.changeCatalog;
            TaskWindow.FileCatalog = Librarian.unsavedSourceImage;

            Adapter.taskWindow = TaskWindow;

            ChangeWindow = new ChangeWindow();
            Adapter.changeWindow = ChangeWindow;
            ChangeWindow.ChangeCatalog = Librarian.changeCatalog;

            //FUTURE:  Subscribe the adapter to the VS IDE events

            //  Subscribe to the StudioAdapter's events
            Adapter.EventNonSourceGotFocus += TaskWindow.HearNonSourceGotFocus;
            Adapter.EventFileGotFocus += TaskWindow.HearFileGotFocus;
            Adapter.EventFileSaved += Librarian.HearFileSaved;
            Adapter.EventFilePasted += Librarian.HearFilePasted;
            Adapter.EventFileSavedAs += Librarian.HearFileSavedAs;
            Adapter.EventFileChangesAbandoned += Librarian.HearFileChangesAbandoned;
            Adapter.EventFileDeleted += Librarian.HearFileDeleted;
            Adapter.EventFileRenamed += Librarian.HearFileRenamed;
            Adapter.EventSolutionSaved += Librarian.HearSolutionSaved;
            Adapter.EventSolutionOpened += Librarian.HearSolutionOpened;

            //  Subscribe to the TaskWindow's events
            TaskWindow.EventTaskWindowToggled += TaskWindow.HearTaskWindowToggled;  //SELF:  This is interesting...
            TaskWindow.EventTaskCompletionChanged += Librarian.HearTaskCompletionChanged;

            //  Subscribe to the ChangeWindow's events
            ChangeWindow.EventChangeAdded += Librarian.HearChangeAdded;
            ChangeWindow.EventChangeListUpdated += TaskWindow.HearChangeListUpdated;
            ChangeWindow.EventChangeListUpdated += Librarian.HearChangeListUpdated;
        }
    }
}
