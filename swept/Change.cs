//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;

namespace swept
{
    public class Change : CompoundFilter
    {

        public Change() { }
        public Change( string id, string description, FileLanguage language ) 
            : base( id, description, language )
            { }

        public bool Equals( Change change )
        {
            if (change == null) return false;
            
            if (ID != change.ID)
                return false;
            if (Description != change.Description)
                return false;
            if (Language != change.Language)
                return false;

            return true;
        }

        public Change Clone()
        {
            return new Change
            {
                ID = this.ID,
                Description = this.Description,
                Language = this.Language,
                Subpath = this.Subpath,
                NamePattern = this.NamePattern
            };
        }
    }

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
}
