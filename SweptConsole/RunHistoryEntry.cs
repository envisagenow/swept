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
        public Dictionary<String, int> Violations;
        public bool Passed;

        public RunHistoryEntry()
        {
            Violations = new Dictionary<string, int>();
        }
    }
}
