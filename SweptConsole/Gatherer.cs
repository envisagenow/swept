//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;

namespace swept
{

    public class Gatherer
    {
        private IEnumerable<Change> _changes;
        private IEnumerable<string> _files;
        private IStorageAdapter _storageAdapter;

        public Gatherer( IEnumerable<Change> changes, IEnumerable<string> files, IStorageAdapter storageAdapter )
        {
            _changes = changes;
            _files = files;
            _storageAdapter = storageAdapter;
        }

        public Dictionary<Change, List<SourceFile>> GetIssueSetDictionary()
        {
            var result = new Dictionary<Change, List<SourceFile>>();

            foreach (string fileName in _files)
            {
                var sourceFile = _storageAdapter.LoadFile( fileName );

                foreach (var change in _changes)
                {
                    if (change.DoesMatch( sourceFile ))
                    {
                        if (!result.ContainsKey( change ))
                        {
                            //result.Add( change, new List<IssueSet>() );
                            result.Add( change, new List<SourceFile>() );
                        }
                        //result[change].Add( new IssueSet( change, sourceFile ) );
                        result[change].Add( sourceFile );
                    }
                }
            }

            return result;
        }
    }
}
