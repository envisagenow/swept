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
        public Dictionary<string, RuleResult> RuleResults;
        public bool Passed;

        public RunHistoryEntry()
        {
            RuleResults = new Dictionary<string, RuleResult>();
        }
    }
}