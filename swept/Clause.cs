﻿//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace swept
{
    public enum ClauseOperator
    {
        And,
        Not,
        Or,
    }

    public enum MatchScope
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

        // TODO: eliminate
        public List<int> _matches = new List<int>();


        public Clause()
        {
            ID = string.Empty;
            NamePattern = string.Empty;
            Language = FileLanguage.None;
            ContentPattern = string.Empty;
            Children = new List<Clause>();
            Operator = ClauseOperator.And;
        }

        public virtual MatchScope Scope
        {
            get
            {
                if (ForceFileScope) return MatchScope.File;

                if (ContentPattern != string.Empty) return MatchScope.Line;

                return MatchScope.File;
            }
        }

        // TODO: eliminate this wart.
        public bool FirstChild { get; set; }

        public ScopedMatches identifyMatchLineNumbers( SourceFile file, string pattern )
        {
            var matchList = new List<int>();
            if (string.IsNullOrEmpty( pattern ))
                return new ScopedMatches( MatchScope.Line, new List<int>() );

            // TODO: Add attribute to allow case sensitive matching
            Regex rx = new Regex( pattern, RegexOptions.IgnoreCase );
            MatchCollection matches = rx.Matches( file.Content );

            foreach (Match match in matches)
            {
                int line = lineNumberOfMatch( match.Index, file.LineIndices );
                matchList.Add( line );
            }

            return new ScopedMatches( MatchScope.Line, matchList );
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

        //public virtual bool DoesMatch( SourceFile file )
        //{
        //    //  breakpoint for tracing how a change matches a file.
        //    bool didMatch = true;
        //    didMatch = didMatch && Language == FileLanguage.None || Language == file.Language;
        //    didMatch = didMatch && Regex.IsMatch( file.Name, NamePattern, RegexOptions.IgnoreCase );

        //    if (Scope == MatchScope.Line)
        //    {
        //        if (ContentPattern != string.Empty)
        //            didMatch = didMatch && MatchesContent( file );
        //    }

        //    didMatch = didMatch && MatchesChildren( file );

        //    if (Operator == ClauseOperator.Not) didMatch = !didMatch;

        //    return didMatch;
        //}


        public IssueSet GetIssueSet( SourceFile file )
        {
            return new IssueSet( this, file, Scope, GetMatchList( file ) );
        }

        public List<int> GetMatchList( SourceFile file )
        {
            //  breakpoint for tracing how a change matches a file.
            bool didMatch = true;
            didMatch = didMatch && Language == FileLanguage.None || Language == file.Language;
            didMatch = didMatch && Regex.IsMatch( file.Name, NamePattern, RegexOptions.IgnoreCase );

            if (Scope == MatchScope.Line)
            {
                if (ContentPattern != string.Empty)
                    didMatch = didMatch && MatchesContent( file );
            }

            didMatch = didMatch && MatchesChildren( file );

            if (Operator == ClauseOperator.Not) didMatch = !didMatch;

            return GetMatchList();
        }

        // TODO: fix
        public List<int> GetMatchList()
        {
            return new List<int>();
        }

        internal bool MatchesContent( SourceFile file )
        {
            var matches = identifyMatchLineNumbers( file, ContentPattern );
            return matches.Matches.Count > 0;
        }

        public virtual bool MatchesChildren( SourceFile file )
        {
            if (Children.Count == 0)
                return true;

            bool didMatch = true;
            //List<int> workingMatches = null;

            //foreach (Clause child in Children)
            //{
            //    bool childMatched = child.DoesMatch( file );
            //    if (workingMatches == null)
            //        workingMatches = child._matches.ToList();

            //    if (child.Operator == ClauseOperator.Or)
            //    {
            //        didMatch = didMatch || childMatched;
            //        workingMatches = workingMatches.Union( child._matches ).ToList();
            //    }
            //    else
            //    {
            //        didMatch = didMatch && childMatched;
            //        workingMatches = workingMatches.Intersect( child._matches ).ToList();
            //    }
            //}

            //_matches = workingMatches;
            
            ////  roll child changes appropriately up to the parent
            //if (ContentPattern != String.Empty)
            //{
            //    _matchList = _matchList.Intersect( workingMatches ).ToList();
            //}
            //else
            //{
            //    _matchList = workingMatches;
            //}

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
