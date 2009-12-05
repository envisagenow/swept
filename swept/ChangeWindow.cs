//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
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
                Event_ChangeAdded(this, new ChangeEventArgs { Change = change });

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