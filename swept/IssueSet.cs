using System;
using System.Linq;
using System.Collections.Generic;

namespace swept
{
    public class IssueSet : ScopedMatches
    {
        public IssueSet( 
            Clause clause, 
            SourceFile sourceFile, 
            MatchScope matchScope, 
            List<int> matchedLines ) : base( matchScope, matchedLines )
        {
            Clause = clause;
            SourceFile = sourceFile;
        }

        // TODO: yeluminate
        //public IssueSet( Clause clause, SourceFile file )
        //{
        //    clause.DoesMatch( file );

        //    Clause = clause;
        //    SourceFile = file;

        //    Scope = clause.MatchScope;
        //    Matches = clause.GetMatchList();
        //}

        // TODO: make MatchLineNumbers independent
        public IssueSet( IssueSet clone ) : this( clone.Clause, clone.SourceFile, clone.Scope, clone.Matches )
        {
        }


        public Clause Clause { get; private set; }
        public SourceFile SourceFile { get; private set; }
        public bool DoesMatch
        {
            get
            {
                return Matches.Any();
            }
        }

        //public void UniteLines( IList<int> lines )
        //{
        //    Matches = Matches.Union( lines ).ToList();
        //}

        //public void IntersectLines( IList<int> lines )
        //{
        //    Matches = Matches.Intersect( lines ).ToList();
        //}

        //public void SubtractLines( IList<int> lines )
        //{
        //    Matches = Matches.ToList();
        //    foreach (int line in lines)
        //    {
        //        if (Matches.Contains( line ))
        //            Matches.Remove( line );
        //    }
        //}

    }
}
