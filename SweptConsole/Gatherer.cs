//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
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

        public Dictionary<Change, List<IssueSet>> GetIssuesPerChange()
        {
            List<string> troublemakers = new List<string>();

            var result = new Dictionary<Change, List<IssueSet>>();

            foreach (string fileName in _files)
            {
                try
                {
                    var sourceFile = _storageAdapter.LoadFile( fileName );

                    foreach (var change in _changes)
                    {
                        if (change.GetMatchList( sourceFile ).Any())
                        {
                            if (!result.ContainsKey( change ))
                            {
                                result.Add( change, new List<IssueSet>() );
                            }
                            var reportFile = new SourceFile( sourceFile.Name );
                            // TODO: fix the tests that this breaks.  Eliminate SourceFile.TaskCount.
                            //reportFile.TaskCount = change.GetMatchList().Count;
                            IssueSet issueSet = change.GetIssueSet( reportFile );
                            result[change].Add( issueSet );
                        }
                    }
                }
                catch
                {
                    troublemakers.Add( fileName );
                }
            }

            return result;
        }
    }
}
