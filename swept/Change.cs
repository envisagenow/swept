//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using System;
using System.Text.RegularExpressions;

namespace swept
{
    public class Change
    {
        public string ID { get; internal set; }
        public string Description { get; internal set; }
        public FileLanguage Language { get; internal set; }
        public string Subpath { get; internal set; }
        public string NamePattern { get; internal set; }

        protected internal Change()
        {
            Subpath = string.Empty;
            NamePattern = string.Empty;
        }

        public Change( string id, string description, FileLanguage language ) : this()
        {
            ID = id;
            Description = description;
            Language = language;
        }

        public bool PassesFilter( SourceFile file )
        {
            bool passesLanguage = Language == FileLanguage.None || Language == file.Language;
            bool passesSubpath = file.Name.StartsWith( Subpath );
            bool passesNamePattern = Regex.IsMatch( file.Name, NamePattern, RegexOptions.IgnoreCase );
            return passesLanguage && passesSubpath && passesNamePattern;
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
