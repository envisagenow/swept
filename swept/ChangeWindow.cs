//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using System;

namespace swept
{
    public class ChangeWindow
    {
        public ChangeCatalog ChangeCatalog;

        public int ChangeCount
        {
            get { return ChangeCatalog._changes.Count; }
        }

        public void AddChange(Change change)
        {
            ChangeCatalog.Add(change);
        }

        #region Publish events
        public event EventHandler<ChangeEventArgs> Event_ChangeAdded;
        public void Raise_ChangeAdded(Change change)
        {
            if (Event_ChangeAdded != null)
                Event_ChangeAdded(this, new ChangeEventArgs { change = change });

            Raise_ChangeListUpdated();
        }

        public event EventHandler<ChangeCatalogEventArgs> Event_ChangeListUpdated;
        public void Raise_ChangeListUpdated()
        {
            if (Event_ChangeListUpdated != null)
                Event_ChangeListUpdated(this, new ChangeCatalogEventArgs{ Catalog = ChangeCatalog } );
        }
        #endregion
    }
}