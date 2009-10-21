//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace swept
{
    public class CompoundFilter
    {
        public string ID { get; internal set; }
        public string Description { get; internal set; }
        public FileLanguage Language { get; internal set; }
        public string Subpath { get; internal set; }
        public string NamePattern { get; internal set; }

        public List<CompoundFilter> Children { get; set; }
        public CompoundFilter()
        {
            Subpath = string.Empty;
            NamePattern = string.Empty;
            Children = new List<CompoundFilter>();
        }

        public CompoundFilter( string id, string description, FileLanguage language ) : this()
        {
            ID = id;
            Description = description;
            Language = language;
        }

        public virtual bool Matches( SourceFile file )
        {
            bool matches = true;
            matches = matches && Language == FileLanguage.None || Language == file.Language;
            matches = matches && file.Name.StartsWith( Subpath );
            matches = matches && Regex.IsMatch( file.Name, NamePattern, RegexOptions.IgnoreCase );
            if( matches )
                foreach( CompoundFilter child in Children )
                    matches = matches && child.Matches( file );
            return matches;
        }
    }

}
