//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.Linq;

namespace swept
{
    public class RunEntry
    {
        public int Number;
        public DateTime Date;
        public bool Passed;
        public Dictionary<string, RuleResult> RuleResults;
        public List<Flag> Flags { get; set; }

        public RunEntry()
        {
            RuleResults = new Dictionary<string, RuleResult>();
            Flags = new List<Flag>();
        }

        public RuleResult AddResult( string id, bool breaking, RuleFailOn ruleFailOn, int threshold, int taskCount, string description )
        {
            var result = new RuleResult {
                ID = id,
                Breaking = breaking,
                FailOn = ruleFailOn,
                Threshold = threshold,
                TaskCount = taskCount,
                Description = description

            };
            RuleResults[id] = result;

            return result;
        }
    }
}