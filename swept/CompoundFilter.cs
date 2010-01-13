using System.Text;
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
    }

    public class CompoundFilter
    {
        public string ID                { get; internal set; }
        public string Description       { get; internal set; }
        public FilterOperator Operator  { get; internal set; }
        public FileLanguage Language    { get; internal set; }
        public string LanguageString
        {
            get { return Language == FileLanguage.None ? string.Empty : Language.ToString(); }
        }
        public string Subpath           { get; internal set; }
        public string NamePattern       { get; internal set; }
        public string ContentPattern    { get; internal set; }

        public CompoundFilter()
        {
            ID = string.Empty;
            Subpath = string.Empty;
            NamePattern = string.Empty;
            Language = FileLanguage.None;
            ContentPattern = string.Empty;
            Children = new List<CompoundFilter>();
            Operator = FilterOperator.And;
            ManualCompletion = false;
            _lineIndices = new List<int>();
            _matchList = new List<int> { 1 };
        }

        public virtual string Name
        {
            get
            {
                switch( Operator )
                {
                case FilterOperator.And:
                    return FirstChild ? "When" : "And";

                case FilterOperator.Not:
                    return FirstChild ? "Not" : "AndNot";

                case FilterOperator.Or:
                    return FirstChild ? "Either" : "Or";

                default:
                    throw new Exception( string.Format( "Can't get Name for Operator [{0}].", Operator.ToString() ) );
                }
            }
        }
        public bool ManualCompletion { get; set; }
        public string ManualCompletionString
        {
            get
            {
                return ManualCompletion ? "Allowed" : string.Empty;
            }
        }
        public bool FirstChild { get; set; }
        public List<CompoundFilter> Children { get; set; }

        internal List<int> _lineIndices;
        internal List<int> _matchList;

        internal void generateLineIndices( string multiLineFile )
        {
            //list of newline indexes
            Regex lineCatcher = new Regex( "\n", RegexOptions.Multiline );
            MatchCollection lineMatches = lineCatcher.Matches( multiLineFile );

            _lineIndices = new List<int>();
            foreach (Match match in lineMatches)
            {
                _lineIndices.Add( match.Index );
            }
        }
        public void identifyMatchLineNumbers( string multiLineFile, string pattern)
        {
            Regex rx = new Regex( pattern );
            MatchCollection matches = rx.Matches( multiLineFile );
            _matchList = new List<int>();

            foreach (Match match in matches)
            {
                int line = lineNumberOfMatch( match.Index, _lineIndices );
                _matchList.Add( line );
            }
        }

        internal int lineNumberOfMatch( int matchIndex, List<int> lineIndices )
        {
            int currentLineNumber = 1;
            foreach (int lineIndex in lineIndices)
            {
                if (matchIndex > lineIndex)
                {
                    currentLineNumber++;
                }
                else
                {
                    break;
                }
            }

            return currentLineNumber;
        }



        public virtual bool Matches( SourceFile file )
        {
            //  breakpoint for tracing how a change matches a file.
            bool matches = true;
            matches = matches && Language == FileLanguage.None || Language == file.Language;
            matches = matches && file.Name.StartsWith( Subpath );
            matches = matches && Regex.IsMatch( file.Name, NamePattern, RegexOptions.IgnoreCase );
            matches = matches && MatchesContent( file.Content );
            matches = matches && MatchesChildren( file );

            if (Operator == FilterOperator.Not) matches = !matches;

            return matches;
        }

        internal bool MatchesContent( string content )
        {
            generateLineIndices(content);
            identifyMatchLineNumbers( content, ContentPattern);
            return _matchList.Count > 0;
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

        internal void markFirstChildren()
        {
            markGeneration( this, true );
        }

        private void markGeneration( CompoundFilter filter, bool isFirstChild )
        {
            filter.FirstChild = isFirstChild;

            bool first = true;
            filter.Children.ForEach( 
                child => {
            		markGeneration(child, first);
            		first = false;
                }
            );
        }

        // TODO--0.3, DC: CompoundFilter.Equals( object )
        public bool Equals( CompoundFilter other )
        {
            if( other == null ) return false;

            if( ID != other.ID )
                return false;
            if( Description != other.Description )
                return false;
            if( Operator != other.Operator )
                return false;

            if( Subpath != other.Subpath )
                return false;
            if( NamePattern != other.NamePattern )
                return false;
            if( Language != other.Language )
                return false;
            
            if( ContentPattern != other.ContentPattern )
                return false;

            if (Children.Count != other.Children.Count)
                return false;

            for (int i = 0; i < Children.Count; i++)
            {
                if (!Children[i].Equals( other.Children[i] ))
                    return false;
            }

            return true;
        }

        public CompoundFilter Clone()
        {
            return CloneInto( new CompoundFilter() );
        }

        protected CompoundFilter CloneInto( CompoundFilter newFilter )
        {
            newFilter.ID = ID;
            newFilter.Description = Description;
            newFilter.Operator = Operator;

            newFilter.Subpath = Subpath;
            newFilter.NamePattern = NamePattern;
            newFilter.Language = Language;

            newFilter.ContentPattern = ContentPattern;
            newFilter.ManualCompletion = ManualCompletion;

            foreach (CompoundFilter child in Children)
            {
                newFilter.Children.Add( child.Clone() );
            }

            return newFilter;
        }

    }
}
