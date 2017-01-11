//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Linq;
using System.Collections.Generic;

namespace swept
{
    public class Gatherer
    {
        private readonly IEnumerable<Rule> _rules;
        private readonly string _folder;
        private readonly IEnumerable<string> _files;
        private readonly IStorageAdapter _storage;

        public Gatherer( IEnumerable<Rule> rules, string folder, IEnumerable<string> files, IStorageAdapter storageAdapter )
        {
            _rules = rules;
            _folder = folder;
            _files = files;
            _storage = storageAdapter;
        }

        public RuleTasks GetRuleTasks()
        {
            var result = new RuleTasks();

            foreach (string fileName in _files)
            {
                var sourceFile = _storage.LoadFile( _folder, fileName );
                if (sourceFile == null) continue;

                foreach (var rule in _rules.OrderBy( c => c.ID ))
                {
                    var match = rule.Subquery.Answer( sourceFile );

                    if (!result.ContainsKey( rule ))
                        result[rule] = new FileTasks();

                    result[rule][sourceFile] = match;
                }
            }

            return result;
        }
    }
}
