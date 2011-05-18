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

            IEnumerable<int> resultMatches = new List<int>();

            if (Scope == other.Scope)
            {
                resultMatches = resultMatches.Union( this );
                resultMatches = resultMatches.Union( other );
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
                resultMatches.RemoveAll( m => other.Contains( m ) );
            else
                if (other.Any())
                    resultMatches.Clear();
            
            return new ScopedMatches( Scope, resultMatches );
        }
    }
}
