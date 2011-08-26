//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2011 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;

namespace swept
{
    public class Subscriber
    {
        private ISweptEventListener Librarian { get; set; }
        private EventSwitchboard Switchboard { get; set; }
        public bool HasSubscribed { get; private set; }

        public const string MSG_Resubscribe_Exception = "A second subscription.  Swept's startup code is busted.";
        public const string MSG_Unsubscribe_Exception = "A second unsubscription.  Swept's shutdown code is busted.";

        public void Subscribe( EventSwitchboard switchboard, ISweptEventListener librarian )
        {
            if (HasSubscribed)
                throw new Exception( MSG_Resubscribe_Exception );

            Switchboard = switchboard;
            Librarian = librarian;

            Switchboard.Event_SolutionOpened += Librarian.Hear_SolutionOpened;
            Switchboard.Event_SolutionClosed += Librarian.Hear_SolutionClosed;
            Switchboard.Event_FileOpened += Librarian.Hear_FileOpened;
            Switchboard.Event_FileClosing += Librarian.Hear_FileClosing;

            HasSubscribed = true;
        }

        public void Unsubscribe()
        {
            if (!HasSubscribed) 
                throw new Exception( MSG_Unsubscribe_Exception );

            //  Unsubscribe from the StudioSwitchboard's events
            Switchboard.Event_SolutionOpened -= Librarian.Hear_SolutionOpened;
            Switchboard.Event_SolutionClosed -= Librarian.Hear_SolutionClosed;
            Switchboard.Event_FileOpened -= Librarian.Hear_FileOpened;
            Switchboard.Event_FileClosing -= Librarian.Hear_FileClosing;

            Switchboard = null;
            Librarian = null;
            HasSubscribed = false;
        }

    }
}
