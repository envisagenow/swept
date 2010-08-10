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
    public enum ClauseOperator
    {
        And,
        Not,
        Or,
    }

    public enum ClauseMatchScope
    {
        File,
        Line,
    }

    public class Clause
    {
        public string ID                { get; internal set; }
        public string Description       { get; internal set; }
        public ClauseOperator Operator  { get; internal set; }
        public bool ForceFileScope      { get; internal set; }
        public FileLanguage Language    { get; internal set; }
        public string NamePattern       { get; internal set; }
        public string ContentPattern    { get; internal set; }
        public List<Clause> Children    { get; set; }

        public Clause()
        {
            ID = string.Empty;
            NamePattern = string.Empty;
            Language = FileLanguage.None;
            ContentPattern = string.Empty;
            Children = new List<Clause>();
            Operator = ClauseOperator.And;
            _matchList = new List<int>();
        }

        public IssueSet GetFileIssueSet( SourceFile file )
        {
            // TODO: fix the lame.
            bool doesMatch = DoesMatch( file );
            return new IssueSet( this, file, MatchScope, GetMatchList(), doesMatch );
        }

        public ClauseMatchScope MatchScope
        {
            get
            {
                if (ForceFileScope) return ClauseMatchScope.File;

                if (ContentPattern != string.Empty) return ClauseMatchScope.Line;

                return ClauseMatchScope.File;
            }
        }


        public bool FirstChild { get; set; }

        internal List<int> _matchList;

        public void identifyMatchLineNumbers( SourceFile file, string pattern )
        {
            _matchList = new List<int>();
            if (string.IsNullOrEmpty( pattern ))
                return;

            // TODO: Add attribute to allow case sensitive matching
            Regex rx = new Regex( pattern, RegexOptions.IgnoreCase );
            MatchCollection matches = rx.Matches( file.Content );

            foreach (Match match in matches)
            {
                int line = lineNumberOfMatch( match.Index, file.LineIndices );
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
            didMatch = didMatch && Regex.IsMatch( file.Name, NamePattern, RegexOptions.IgnoreCase );

            if (MatchScope == ClauseMatchScope.Line)
            {
                if (ContentPattern != string.Empty)
                    didMatch = didMatch && MatchesContent( file );
            }

            didMatch = didMatch && MatchesChildren( file );

            if (Operator == ClauseOperator.Not) didMatch = !didMatch;

            _matchList.Sort();
            return didMatch;
        }

        internal bool MatchesContent( SourceFile file )
        {
            identifyMatchLineNumbers( file, ContentPattern );
            return _matchList.Count > 0;
        }

        public virtual bool MatchesChildren( SourceFile file )
        {
            if (Children.Count == 0)
                return true;

            bool didMatch = true;
            List<int> workingMatches = null;
            foreach (Clause child in Children)
            {
                bool childMatched = child.DoesMatch( file );
                if (workingMatches == null)
                    workingMatches = child._matchList.ToList();

                if (child.Operator == ClauseOperator.Or)
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

        private void markGeneration( Clause filter, bool isFirstChild )
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
        public bool Equals( Clause other )
        {
            if( other == null ) return false;

            if( ID != other.ID )
                return false;
            if( Description != other.Description )
                return false;
            if( Operator != other.Operator )
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

        public Clause Clone()
        {
            return CloneInto( new Clause() );
        }

        protected Clause CloneInto( Clause newFilter )
        {
            newFilter.ID = ID;
            newFilter.Description = Description;
            newFilter.Operator = Operator;

            newFilter.NamePattern = NamePattern;
            newFilter.Language = Language;

            newFilter.ContentPattern = ContentPattern;

            foreach (Clause child in Children)
            {
                newFilter.Children.Add( child.Clone() );
            }

            return newFilter;
        }

                
    }
}
