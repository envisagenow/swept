//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;

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
        Unknown
    }

    public class Change : CompoundFilter
    {
        public Change() : base() { }

        public override string Name
        {
            get { return "Change"; }
        }

        public Change Clone()
        {
            return (Change)base.CloneInto( new Change() );
        }
    }
}
