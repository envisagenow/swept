//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using System;
using System.Collections.Generic;
using System.Linq;

namespace swept
{
    public class ChangeCatalog
    {
        // TODO--0.3, DC: move back to standard list with sort on output
        internal SortedList<string, Change> _changes;

        public ChangeCatalog()
        {
            _changes = new SortedList<string, Change>();
        }

        private ChangeCatalog( SortedList<string, Change> changelist )
            : this()
        {
            foreach( var pair in changelist )
            {
                _changes[pair.Key] = pair.Value;
            }
        }

        public ChangeCatalog Clone()
        {
            return new ChangeCatalog( _changes );
        }

        public bool Equals( ChangeCatalog cat1 )
        {
            if( _changes.Count != cat1._changes.Count )
            {
                return false;
            }

            foreach( string key in _changes.Keys )
            {
                if( !cat1._changes.Keys.Contains( key ) ) return false;
                if( !_changes[key].Equals( cat1._changes[key] ) ) return false;
            }

            return true;
        }

        public List<Change> GetChangesForFile( SourceFile file )
        {
            var changes = _changes.Values.ToList<Change>();
            return changes.FindAll( change => change.PassesFilter( file ) );
        }

        public void Add( Change change )
        {
            _changes.Add( change.ID, change );
        }

        public void Remove( string changeID )
        {
            _changes.Remove( changeID );
        }

        // TODO--0.3, DC: Edit a Change

        public List<Change> FindAll( Predicate<Change> match )
        {
            List<Change> changeList = _changes.Values.ToList<Change>();
            return changeList.FindAll( match );
        }

    }
}
