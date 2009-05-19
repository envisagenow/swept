//  Swept:  Software Enhancement Progress Tracking.  Copyright 2008 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
using System;
using System.Collections.Generic;
using System.Text;

namespace swept
{
    public class ChangeCatalog
    {
        public bool IsDirty { get; private set; }
        internal List<Change> changes;

        public ChangeCatalog()
        {
            changes = new List<Change>();
        }

        public List<Change> GetListForLanguage( FileLanguage fileLanguage )
        {
            return changes.FindAll( c => c.Language == fileLanguage );
        }

        public void Add(Change change)
        {
            IsDirty = true;
            changes.Add(change);
        }

        public void Remove(string changeID)
        {
            IsDirty = true;
            changes.RemoveAll(c => c.ID == changeID);
        }

        public List<Change> FindAll(Predicate<Change> match)
        {
            return changes.FindAll(match);
        }

        public void MarkClean()
        {
            IsDirty = false;
        }
    }
}
