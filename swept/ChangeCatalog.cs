//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Linq;

namespace swept
{
    public class ChangeCatalog
    {
        // TODO: eliminate ChangeCatalog.IsDirty
        //public bool IsDirty { get; private set; }

        // TODO: Make private
        internal SortedList<string, Change> changes;

        public ChangeCatalog()
        {
            changes = new SortedList<string, Change>();
        }

        private ChangeCatalog( SortedList<string, Change> changelist ) : this()
        {
            foreach (var pair in changelist)
            {
                changes[pair.Key] = pair.Value;
            }
        }

        public ChangeCatalog Clone()
        {
            return new ChangeCatalog( changes );
        }

        public bool Equals( ChangeCatalog cat1 )
        {
            if (changes.Count != cat1.changes.Count)
            {
                return false;
            }

            foreach( string key in changes.Keys ) 
            {
                if (!cat1.changes.Keys.Contains( key )) return false;
                if (!changes[key].Equals(cat1.changes[key])) return false;
            }
             
            return true;
        }

        public List<Change> GetChangesForFile(SourceFile file)
        {
            return GetListForLanguage(file.Language);
        }

        private List<Change> GetListForLanguage( FileLanguage fileLanguage )
        {
            List<Change> changeList = changes.Values.ToList<Change>();
            return changeList.FindAll( change => change.Language == fileLanguage );
        }

        public void Add(Change change)
        {
            //IsDirty = true;
            changes.Add( change.ID, change);
        }

        public void Remove(string changeID)
        {
            //IsDirty = true;
            changes.Remove( changeID );
        }

        // TODO: Edit a Change

        public List<Change> FindAll(Predicate<Change> match)
        {
            List<Change> changeList = changes.Values.ToList<Change>();
            return changeList.FindAll( match );
        }

        //public void MarkClean()
        //{
        //    IsDirty = false;
        //}

    }
}
