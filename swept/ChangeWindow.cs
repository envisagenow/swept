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
        public int ChangeCount {
            get
            {
                return Changes.changes.Count;
            }
        }

        public ChangeCatalog Changes { get; set; }

        public void AddChange(Change change)
        {
            Changes.Add(change);
        }

        //public event EventHandler RaiseChangesUpdated;
        //public void WhenChangesUpdated()
        //{
        //    if (RaiseChangesUpdated != null)
        //    {
        //        RaiseChangesUpdated(this, new EventArgs());
        //    }
        //}
    }
}
