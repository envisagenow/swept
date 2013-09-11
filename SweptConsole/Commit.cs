//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2013 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;

namespace swept
{
    /// <summary>A commit of unspecified files by one committer at one moment</summary>
    public class Commit {
        public string ID { get; set; }
        public string Person { get; set; }
        public string Time { get; set; }
    }
}
