//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace swept
{
    public enum FilterOperator
    {
		And,
        Not,
        Or,
        NotAnd
    }

    public class CompoundFilter
    {
        public string ID { get; internal set; }
        public string Description { get; internal set; }
        public FileLanguage Language { get; internal set; }
        public string Subpath { get; internal set; }
        public string NamePattern { get; internal set; }

        public string ContentPattern { get; set; }
        public FilterOperator Operator { get; internal set; }

        public List<CompoundFilter> Children { get; set; }
        public CompoundFilter()
        {
            Subpath = string.Empty;
            NamePattern = string.Empty;
            Children = new List<CompoundFilter>();
            Operator = FilterOperator.And;
        }

        public CompoundFilter( string id, string description, FileLanguage language )
            : this()
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
            matches = matches && MatchesChildren( file );

            if (Operator == FilterOperator.Not) matches = !matches;

            return matches;
        }

        public virtual bool MatchesChildren( SourceFile file )
        {
            bool matches = true;
            foreach (CompoundFilter child in Children)
            {
                if (child.Operator == FilterOperator.Or)
                { matches = matches || child.Matches( file ); }
                else
                { matches = matches && child.Matches( file ); }
            }

            return matches;
        }
    }
}
