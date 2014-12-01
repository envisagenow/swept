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
        SQL,
    }

    public enum RuleFailOn
    {
        None,
        Any,
        Increase,
    }

    public class Rule
    {
        public string ID { get; internal set; }
        public string Description { get; internal set; }
        public string Notes { get; set; }
        public string Rationale { get; set; }
        public RuleFailOn FailOn { get; set; }
        public List<SeeAlso> SeeAlsos { get; set; }

        public Rule()
        {
            SeeAlsos = new List<SeeAlso>();
        }

        public ISubquery Subquery { get; set; }

        public ClauseMatch GetMatches( SourceFile file )
        {
            ClauseMatch match = Subquery.Answer( file );
            match.Rule = this;
            return match;
        }

    }
}
