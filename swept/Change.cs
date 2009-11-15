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

        // TODO--0.N, DC:  Clone and inheritance?
        public Change Clone()
        {
            return new Change
            {
                ID = this.ID,
                Description = this.Description,
                Operator = this.Operator,

                Subpath = this.Subpath,
                NamePattern = this.NamePattern,
                Language = this.Language,

                ContentPattern = this.ContentPattern,
                ManualCompletion = this.ManualCompletion,
            };
        }
    }
}
