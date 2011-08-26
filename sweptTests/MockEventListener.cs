//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2011 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;

namespace swept.Tests
{
    public class MockEventListener : ISweptEventListener
    {
        #region ISweptEventListener Members

        public FileEventArgs SolutionOpened_args;
        public void Hear_SolutionOpened( object sender, FileEventArgs args )
        {
            SolutionOpened_args = args;
        }

        public EventArgs SolutionClosed_args;
        public void Hear_SolutionClosed( object sender, EventArgs args )
        {
            SolutionClosed_args = args;
        }

        public FileEventArgs FileOpened_args;
        public void Hear_FileOpened( object sender, FileEventArgs args )
        {
            FileOpened_args = args;
        }

        public FileEventArgs FileClosed_args;
        public void Hear_FileClosing( object sender, FileEventArgs args )
        {
            FileClosed_args = args;
        }

        #endregion
    }
}
