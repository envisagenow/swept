//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Linq;
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

        public IssueSet GetIssueSet( SourceFile file )
        {
            return new IssueSet( this, file, Scope, GetMatches( file ) );
        }

        public ScopedMatches GetMatches( SourceFile file )
        {
            var matches = new ScopedMatches( MatchScope.File, new List<int>() { 1 } );
            matches = MatchFileOnLanguage( matches, file, Language );
            matches = MatchFileOnNamePattern( matches, file, NamePattern );
            matches = MatchFileOnContentPattern( matches, file, ContentPattern );

            if (matches.Any() && Children.Any())
            {
                ScopedMatches childMatches = GetChildMatches( file );

                if (Operator == ClauseOperator.And)
                    matches = matches.Intersection( childMatches );
                else if (Operator == ClauseOperator.Or)
                    matches = matches.Union( childMatches );
            }

            if (ForceFileScope && matches.Any())
                matches = new ScopedMatches( MatchScope.File, new List<int> { 1 } );

            // TODO: determine if I ever use Not.  :D
            if (Operator == ClauseOperator.Not)
            {
                if (matches.Any())
                {
                    matches.Clear();
                }
                else
                {
                    matches.Add( 1 );
                }
            }

            return matches;
        }

        private ScopedMatches MatchFileOnLanguage( ScopedMatches matches, SourceFile file, FileLanguage language )
        {
            if (!matches.Any() || language == FileLanguage.None)
                return matches;

            if (language == file.Language)
                return matches;
            else
                return new ScopedMatches( MatchScope.File, new List<int>() );
        }

        private ScopedMatches MatchFileOnNamePattern( ScopedMatches matches, SourceFile file, string namePattern )
        {
            if (!matches.Any() || string.IsNullOrEmpty( namePattern ))
                return matches;

            if (Regex.IsMatch( file.Name, namePattern, RegexOptions.IgnoreCase ))
                return matches;
            else
                return new ScopedMatches( MatchScope.File, new List<int>() );
        }

        private ScopedMatches MatchFileOnContentPattern( ScopedMatches matches, SourceFile file, string contentPattern )
        {
            if (!matches.Any() || string.IsNullOrEmpty( contentPattern ))
                return matches;

            return identifyMatchList( file, contentPattern ); ;
        }

        public ScopedMatches identifyMatchList( SourceFile file, string pattern )
        {
            var matchList = new ScopedMatches( MatchScope.Line, new List<int>() );
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

            return matchList;
        }

        public virtual ScopedMatches GetChildMatches( SourceFile file )
        {
            ScopedMatches workingMatches = new ScopedMatches( MatchScope.File, new List<int>() );

            bool first = true;
            foreach (Clause child in Children)
            {
                ScopedMatches childMatches = child.GetMatches( file );

                if (first)
                {
                    workingMatches = childMatches;
                }
                else if (child.Operator == ClauseOperator.Or)
                {
                    workingMatches = workingMatches.Union( childMatches );
                }
                else if (child.Operator == ClauseOperator.And)
                {
                    workingMatches = workingMatches.Intersection( childMatches );
                }

                first = false;
            }

            return workingMatches;
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
