//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;

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

    public class Change : Clause
    {
        public List<SeeAlso> SeeAlsos { get; set; }
        public Change() : base() 
        {
            SeeAlsos = new List<SeeAlso>();
        }

        public new Change Clone()
        {
            return (Change)base.CloneInto( new Change() );
            // TODO: also the SeeAlsos
        }
    }
}
