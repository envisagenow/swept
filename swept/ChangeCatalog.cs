//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2012 Jason Cole and Envisage Technologies Corp.
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
