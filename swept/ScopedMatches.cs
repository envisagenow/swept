using System;
using System.Collections.Generic;
using System.Linq;

namespace swept
{
    public class ClauseMatch
    {
        // TODO: Remove this, and split the IssueSet ctors that delegate to this one, to delegate to the below ctors
        public ClauseMatch( MatchScope scope, List<int> matches )
        {
            Scope = scope;
            LinesWhichMatch = new List<int>( matches );
        }

        public ClauseMatch( bool matchesFile )
        {
            Scope = MatchScope.File;
            FileDoesMatch = matchesFile;
        }

        public ClauseMatch( List<int> matchedLines )
        {
            Scope = MatchScope.Line;
            LinesWhichMatch = new List<int>( matchedLines );
        }


        public bool FileDoesMatch { get; private set; }
        public List<int> LinesWhichMatch { get; private set; }
        public virtual MatchScope Scope { get; set; }

        public ClauseMatch Union( ClauseMatch other )
        {
            MatchScope resultScope = MatchScope.Line;
            if (Scope == MatchScope.File && other.Scope == MatchScope.File)
                resultScope = MatchScope.File;
            else if (Scope == MatchScope.Line && LinesWhichMatch.Count == 0 && other.Scope == MatchScope.File && other.FileDoesMatch)
                resultScope = MatchScope.File;
            else if (Scope == MatchScope.File && FileDoesMatch && other.Scope == MatchScope.Line && other.LinesWhichMatch.Count == 0)
                resultScope = MatchScope.File;

            IEnumerable<int> resultMatches = new List<int>();
            if (Scope == MatchScope.Line)
                resultMatches = resultMatches.Union( this.LinesWhichMatch );
            if (other.Scope == MatchScope.Line)
                resultMatches = resultMatches.Union( other.LinesWhichMatch );

            if (resultScope == MatchScope.File && resultMatches.Any())
                resultMatches = new List<int> { 1 };

            return new ClauseMatch( resultScope, resultMatches.ToList() );
        }

        public ClauseMatch Intersection( ClauseMatch other )
        {
            MatchScope resultScope = MatchScope.Line;
            if (Scope == MatchScope.File && other.Scope == MatchScope.File)
                resultScope = MatchScope.File;

            IEnumerable<int> resultMatches = new List<int>( this.LinesWhichMatch );

            if (Scope == other.Scope)
            {
                resultMatches = resultMatches.Intersect( other.LinesWhichMatch );
            }
            else
            {
                if (Scope == MatchScope.Line)
                    resultMatches = LinesWhichMatch;
                else
                    resultMatches = other.LinesWhichMatch;
            }

            return new ClauseMatch( resultScope, resultMatches.ToList() );
        }

        public ClauseMatch Subtraction( ClauseMatch other )
        {
            List<int> resultMatches = new List<int>( this.LinesWhichMatch );

            if (Scope == MatchScope.Line && other.Scope == MatchScope.Line)
            {
                resultMatches.RemoveAll( m => other.LinesWhichMatch.Contains( m ) );
            }
            else
            {
                if (other.LinesWhichMatch.Any())
                    resultMatches.Clear();
            }

            return new ClauseMatch( Scope, resultMatches );
        }



        public override bool Equals( object obj )
        {
            ClauseMatch other = obj as ClauseMatch;
            return Equals( other );
        }

        public bool Equals( ClauseMatch other )
        {
            if (other == null) return false;

            if (other.Scope != Scope)
                return false;

            if (other.LinesWhichMatch.Count != LinesWhichMatch.Count)
                return false;

            for (int i = 0; i < other.LinesWhichMatch.Count; i++)
            {
                if (!other.LinesWhichMatch[i].Equals( this.LinesWhichMatch[i] ))
                    return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            return LinesWhichMatch.Count;
        }

    }
}
