//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.Linq;

namespace swept
{
    public class RunHistoryEntry
    {
        public int Number;
        public DateTime Date;
        public bool Passed;
        public Dictionary<string, HistoricRuleResult> RuleResults;

        public RunHistoryEntry()
        {
            RuleResults = new Dictionary<string, HistoricRuleResult>();
        }

        public HistoricRuleResult AddResult( string id, bool breaking, RuleFailOn ruleFailOn, int threshold, int taskCount )
        {
            var result = new HistoricRuleResult {
                ID = id,
                Breaking = breaking,
                FailOn = ruleFailOn,
                Threshold = threshold,
                TaskCount = taskCount
            };
            RuleResults[id] = result;

            return result;
        }
    }
}