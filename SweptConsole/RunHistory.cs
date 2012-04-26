//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.Linq;

namespace swept
{
    public class RunHistory
    {
        public List<RunHistoryEntry> Runs;

        public RunHistory()
        {
            Runs = new List<RunHistoryEntry>();
        }
    }

}
