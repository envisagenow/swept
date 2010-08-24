using System;
using System.Collections.Generic;
using System.Linq;

namespace swept
{
    public class ScopedMatches
    {
        public ScopedMatches( MatchScope scope, List<int> matches )
        {
            Scope = scope;
            _matches = matches;
        }

        public virtual MatchScope Scope { get; set; }
        protected internal List<int> _matches;
        public virtual List<int> Matches
        {
            get { return _matches; }
        }

        public ScopedMatches Union( ScopedMatches other )
        {
            MatchScope resultScope = MatchScope.Line;
            if (Scope == MatchScope.File && other.Scope == MatchScope.File)
                resultScope = MatchScope.File;

            List<int> resultMatches = new List<int>();

            if (Scope == other.Scope)
            {
                resultMatches = Matches.Union( other.Matches ).ToList();
            }
            else
            {
                if (Scope == MatchScope.Line)
                    resultMatches = Matches.ToList();
                else
                    resultMatches = other.Matches.ToList();
            }

            return new ScopedMatches( resultScope, resultMatches );
        }

        public ScopedMatches Intersection( ScopedMatches other )
        {
            MatchScope resultScope = MatchScope.Line;
            if (Scope == MatchScope.File && other.Scope == MatchScope.File)
                resultScope = MatchScope.File;

            List<int> resultMatches = new List<int>();

            if (Scope == other.Scope)
            {
                resultMatches = Matches.Intersect( other.Matches ).ToList();
            }
            else
            {
                if (Scope == MatchScope.Line)
                    resultMatches = Matches.ToList();
                else
                    resultMatches = other.Matches.ToList();
            }

            return new ScopedMatches( resultScope, resultMatches );
        }

        public ScopedMatches Subtraction( ScopedMatches other )
        {
            List<int> resultMatches = Matches.ToList();

            if (Scope == MatchScope.Line && other.Scope == MatchScope.Line)
                resultMatches.RemoveAll( m => other.Matches.Contains( m ) );
            else
                if (other.Matches.Any())
                    resultMatches.Clear();
            
            return new ScopedMatches( Scope, resultMatches );
        }
    }
}
