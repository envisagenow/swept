//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2013 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.Linq;

namespace swept
{
    public class Flag
    {
        public string RuleID { get; set; }
        public List<Commit> Commits { get; set; }
        public int Threshold { get; set; }
        public int TaskCount { get; set; }

        public Flag()
        {
            Commits = new List<Commit>();
        }
    }

}
