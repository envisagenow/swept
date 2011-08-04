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
        public EventSwitchboard Switchboard { get; set; }
        //private IUserAdapter UserAdapter { get; set; }

        public void Start( EventSwitchboard switchboard )
        {
            //UserAdapter = userAdapter;
            Switchboard = switchboard;
            StorageAdapter = new StorageAdapter();

            Librarian = new ProjectLibrarian( StorageAdapter, Switchboard );
            Switchboard.Librarian = Librarian;

            //  Subscribe to the StudioSwitchboard's events
            Switchboard.Event_SolutionOpened += Librarian.Hear_SolutionOpened;
            Switchboard.Event_SolutionClosed += Librarian.Hear_SolutionClosed;
            Switchboard.Event_FileOpened += Librarian.Hear_FileOpened;
            Switchboard.Event_FileClosing += Librarian.Hear_FileClosing;
        }

        public void Stop()
        {
            //  Unsubscribe from the StudioSwitchboard's events
            Switchboard.Event_SolutionOpened -= Librarian.Hear_SolutionOpened;
            Switchboard.Event_SolutionClosed -= Librarian.Hear_SolutionClosed;
            Switchboard.Event_FileOpened -= Librarian.Hear_FileOpened;
            Switchboard.Event_FileClosing -= Librarian.Hear_FileClosing;

            Switchboard = null;
            Librarian = null;
        }

    }
}
