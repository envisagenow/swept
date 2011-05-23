using System;
using System.Collections.Generic;
using System.Linq;

namespace swept
{
    public class ScopedMatches : List<int>
    {
        public ScopedMatches( MatchScope scope, List<int> matches ) : base( matches )
        {
            Scope = scope;
        }

        public virtual MatchScope Scope { get; set; }

        public ScopedMatches Union( ScopedMatches other )
        {
            MatchScope resultScope = MatchScope.Line;
            if (Scope == MatchScope.File && other.Scope == MatchScope.File)
                resultScope = MatchScope.File;
            else if (Scope == MatchScope.Line && this.Count == 0 && other.Scope == MatchScope.File && other.Count == 1)
                resultScope = MatchScope.File;
            else if (Scope == MatchScope.File && this.Count == 1 && other.Scope == MatchScope.Line && other.Count == 0)
                resultScope = MatchScope.File;

            IEnumerable<int> resultMatches = new List<int>();
            resultMatches = resultMatches.Union( this );
            resultMatches = resultMatches.Union( other );

            if (resultScope == MatchScope.File && resultMatches.Any())
                resultMatches = new List<int> { 1 };

            return new ScopedMatches( resultScope, resultMatches.ToList() );
        }

        public ScopedMatches Intersection( ScopedMatches other )
        {
            MatchScope resultScope = MatchScope.Line;
            if (Scope == MatchScope.File && other.Scope == MatchScope.File)
                resultScope = MatchScope.File;

            IEnumerable<int> resultMatches = new List<int>( this );

            if (Scope == other.Scope)
            {
                resultMatches = resultMatches.Intersect( other );
            }
            else
            {
                if (Scope == MatchScope.Line)
                    resultMatches = this;
                else
                    resultMatches = other;
            }

            return new ScopedMatches( resultScope, resultMatches.ToList() );
        }

        public ScopedMatches Subtraction( ScopedMatches other )
        {
            List<int> resultMatches = new List<int>(this);

            if (Scope == MatchScope.Line && other.Scope == MatchScope.Line)
            {
                resultMatches.RemoveAll( m => other.Contains( m ) );
            }
            else
            {
                if (other.Any())
                    resultMatches.Clear();
            }

            return new ScopedMatches( Scope, resultMatches );
        }



        public override bool Equals( object obj )
        {
            ScopedMatches other = obj as ScopedMatches;
            return Equals( other );
        }

        public bool Equals( ScopedMatches other )
        {
            if (other == null) return false;

            if (other.Scope != Scope)
                return false;

            if (other.Count != Count)
                return false;

            for (int i = 0; i < other.Count; i++)
            {
                if (!other[i].Equals( this[i] ))
                    return false;
            }

            return true;
        }
    }
}
