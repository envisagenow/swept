//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using swept.DSL;

namespace swept
{
    public enum FileLanguage
    {
        None,
        CSharp,
        HTML,
        JavaScript,
        CSS,
        XSLT,
        VBNet,
        Project,
        Solution,
        Unknown,
    }

    public enum RunFailMode
    {
        None,
        Any,
        Over,
    }

    public class Change
    {
        public RunFailMode RunFail { get; set; }
        public List<SeeAlso> SeeAlsos { get; set; }
        public string ID { get; internal set; }
        public string Description { get; internal set; }
        public int RunFailOverLimit { get; set; }

        public Change()
        {
            SeeAlsos = new List<SeeAlso>();
        }

        public ISubquery Subquery { get; set; }

        public ClauseMatch GetMatches( SourceFile file )
        {
            ClauseMatch match = Subquery.Answer( file );
            match.Change = this;
            return match;
        }
    }
}
