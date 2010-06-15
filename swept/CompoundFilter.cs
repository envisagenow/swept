//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System.Text;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

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
            // TODO: cache results per sourcefile
            //if( mySourceFile.LineIndices != null )

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
            _matchList = new List<int>();
            if (string.IsNullOrEmpty( pattern ))
                return;
            Regex rx = new Regex( pattern );
            MatchCollection matches = rx.Matches( multiLineFile );

            foreach (Match match in matches)
            {
                int line = lineNumberOfMatch( match.Index, _lineIndices );
                _matchList.Add( line );
            }
        }

        public List<int> GetMatchList()
        {
            return _matchList;
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



        public virtual bool DoesMatch( SourceFile file )
        {
            //  breakpoint for tracing how a change matches a file.
            bool didMatch = true;
            didMatch = didMatch && Language == FileLanguage.None || Language == file.Language;
            didMatch = didMatch && file.Name.StartsWith( Subpath );
            didMatch = didMatch && Regex.IsMatch( file.Name, NamePattern, RegexOptions.IgnoreCase );
            
            if( ContentPattern != string.Empty )
                didMatch = didMatch && MatchesContent( file.Content );
            
            didMatch = didMatch && MatchesChildren( file );

            if (Operator == FilterOperator.Not) didMatch = !didMatch;

            _matchList.Sort();
            return didMatch;
        }

        internal bool MatchesContent( string content )
        {
            generateLineIndices( content );
            identifyMatchLineNumbers( content, ContentPattern );
            return _matchList.Count > 0;
        }

        public virtual bool MatchesChildren( SourceFile file )
        {
            if (Children.Count == 0)
                return true;

            bool didMatch = true;
            List<int> workingMatches = null;
            foreach (CompoundFilter child in Children)
            {
                bool childMatched = child.DoesMatch( file );
                if (workingMatches == null)
                    workingMatches = child._matchList.ToList();

                if (child.Operator == FilterOperator.Or)
                {
                    didMatch = didMatch || childMatched;
                    workingMatches = workingMatches.Union( child._matchList ).ToList();
                }
                else
                {
                    didMatch = didMatch && childMatched;
                    workingMatches = workingMatches.Intersect( child._matchList ).ToList();
                }
            }

            //  roll child changes appropriately up to the parent
            if (ContentPattern != String.Empty)
            {
                _matchList = _matchList.Intersect( workingMatches ).ToList();
            }
            else
            {
                _matchList = workingMatches;
            }

            return didMatch;
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
