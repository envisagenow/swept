//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2011 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Linq;
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

        public Dictionary<Change, Dictionary<SourceFile, ClauseMatch>> GetMatchesPerChange()
        {
            var result = new Dictionary<Change, Dictionary<SourceFile,ClauseMatch>>();

            foreach (string fileName in _files)
            {
                var sourceFile = _storageAdapter.LoadFile( fileName );

                foreach (var change in _changes.OrderBy( c => c.ID))
                {
                    var match = change.Subquery.Answer( sourceFile );

                    if (!result.ContainsKey( change ))
                        result[change] = new Dictionary<SourceFile, ClauseMatch>();

                    result[change][sourceFile] = match;
                }
            }

            return result;
        }
    }
}
