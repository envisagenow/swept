//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using System;
using System.Collections.Generic;
using System.Text;

namespace swept
{
    public class ChangeWindow
    {
        public ChangeCatalog ChangeCatalog;

        public int ChangeCount
        {
            get { return ChangeCatalog.changes.Count; }
        }

        public void AddChange(Change change)
        {
            ChangeCatalog.Add(change);
        }

        #region Publish events
        public event EventHandler<ChangeEventArgs> EventChangeAdded;
        public void RaiseChangeAdded(Change change)
        {
            if (EventChangeAdded != null)
                EventChangeAdded(this, new ChangeEventArgs { change = change });

            RaiseChangeListUpdated();
        }

        public event EventHandler EventChangeListUpdated;
        public void RaiseChangeListUpdated()
        {
            if (EventChangeListUpdated != null)
                EventChangeListUpdated(this, new EventArgs());
        }
        #endregion
    }
}