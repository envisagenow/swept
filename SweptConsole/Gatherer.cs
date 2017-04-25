//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            var files = _files.ToList();
            var result = new RuleTasks();
            var orderedRules = _rules.OrderBy(c => c.ID).ToArray();
            Parallel.ForEach(
                files,
                fileName =>
                {
                    var sourceFile = _storage.LoadFile( _folder, fileName );
                    if (sourceFile != null)
                    {
                        for (var i = 0; i < orderedRules.Length; i++)
                        {
                            var rule = orderedRules[i];
                            var match = rule.Subquery.Answer(sourceFile);

                            lock (result)
                            {
                                if (!result.ContainsKey(rule))
                                    result[rule] = new FileTasks();

                                result[rule][sourceFile] = match;
                            }
                        }
                    }
                });

            return result;
        }
    }
}
