//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using System;

namespace swept
{
    public class Change
    {
        protected internal string ID;
        protected internal string Description;
        protected internal FileLanguage Language;

        protected internal Change() { }
        public Change( string id, string description, FileLanguage language )
        {
            ID = id;
            Description = description;
            Language = language;
        }

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
