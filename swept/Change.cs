//  Swept:  Software Enhancement Progress Tracking.  Copyright 2008 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System;

namespace swept
{
    public class Change
    {
        protected internal string ID;
        protected internal string Description;
        protected internal FileLanguage Language;

        protected Change() { }
        public Change( string id, string description, FileLanguage language )
        {
            ID = id;
            Description = description;
            Language = language;
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
