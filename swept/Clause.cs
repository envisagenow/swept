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
            return new IssueSet( this, file, GetMatches( file ) );
        }

        public ClauseMatch GetMatches( SourceFile file )
        {
            ClauseMatch matches = new FileMatch( true );
            matches = MatchFileOnLanguage( matches, file, this.Language );
            matches = MatchFileOnNamePattern( matches, file, this.NamePattern );
            matches = MatchFileOnContentPattern( matches, file, this.ContentPattern );

            if (matches.DoesMatch && Children.Any())
            {
                ClauseMatch childMatches = GetChildMatches( file );

                if (Operator == ClauseOperator.And)
                    matches = matches.Intersection( childMatches );
                else if (Operator == ClauseOperator.Or)
                    matches = matches.Union( childMatches );
            }

            if (ForceFileScope && matches.DoesMatch)
                matches = new FileMatch( true );

            return matches;
        }

        /// <returns>FileMatch</returns>
        private ClauseMatch MatchFileOnLanguage( ClauseMatch matches, SourceFile file, FileLanguage language )
        {
            if (!matches.DoesMatch || language == FileLanguage.None)
                return matches;

            if (language == file.Language)
                return matches;
            else
                return new FileMatch( false );
        }

        /// <returns>FileMatch</returns>
        private ClauseMatch MatchFileOnNamePattern( ClauseMatch matches, SourceFile file, string namePattern )
        {
            if (!matches.DoesMatch || string.IsNullOrEmpty( namePattern ))
                return matches;

            if (Regex.IsMatch( file.Name, namePattern, RegexOptions.IgnoreCase ))
                return matches;
            else
                return new FileMatch( false );
        }

        private ClauseMatch MatchFileOnContentPattern( ClauseMatch matches, SourceFile file, string contentPattern )
        {
            if (!matches.DoesMatch || string.IsNullOrEmpty( contentPattern ))
                return matches;

            return identifyMatchList( file, contentPattern ); ;
        }

        /// <returns>FileMatch</returns>
        public ClauseMatch identifyMatchList( SourceFile file, string pattern )
        {
            var matchList = new LineMatch( new List<int>() );
            if (string.IsNullOrEmpty( pattern ))
                return new LineMatch( new List<int>() );

            // TODO: Add attribute to allow case sensitive matching
            Regex rx = new Regex( pattern, RegexOptions.IgnoreCase );
            MatchCollection matches = rx.Matches( file.Content );

            foreach (Match match in matches)
            {
                int line = lineNumberOfMatch( match.Index, file.LineIndices );
                matchList.Lines.Add( line );
            }

            return matchList;
        }

        public virtual ClauseMatch GetChildMatches( SourceFile file )
        {
            ClauseMatch workingMatches = new FileMatch( false );

            bool first = true;
            foreach (Clause child in Children)
            {
                ClauseMatch childMatches = child.GetMatches( file );

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
