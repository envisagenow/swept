//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Linq;
using System.Collections.Generic;

namespace swept
{
    public class ChangeCatalog
    {
        internal List<Change> _changes;

        public ChangeCatalog()
        {
            _changes = new List<Change>();
        }


        public bool Equals( ChangeCatalog other )
        {
            if( _changes.Count != other._changes.Count )
                return false;

            Comparison<Change> onIDs = (left, right) => left.ID.CompareTo(right.ID);
            _changes.Sort( onIDs );
            other._changes.Sort( onIDs );

            for( int i = 0; i < _changes.Count; i++ )
            {
                if( !_changes[i].Equals( other._changes[i] ) )
                    return false;
            }

            return true;
        }

        public List<Change> GetSortedChanges()
        {
            _changes.Sort( (left, right) => left.ID.CompareTo(right.ID) );
            return _changes;
        }

        public List<Change> GetChangesForFile( SourceFile file )
        {
            return _changes.FindAll( change => change.GetMatches( file ).DoesMatch );
        }

        public void Add( Change change )
        {
            if( _changes.Exists( c => c.ID == change.ID) )
                throw new Exception( string.Format( "There is already a change with the ID [{0}].", change.ID ) );
            _changes.Add( change );
        }
    }
}
