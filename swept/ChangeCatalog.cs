//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
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

        //FUTURE: Sort changes when writing them out
        public string ToXmlText()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<ChangeCatalog>");

            foreach(Change change in this.changes)
            {
                sb.AppendLine(change.ToXmlText());
            }

            sb.Append("</ChangeCatalog>");
            return sb.ToString();
        }
    }
}
