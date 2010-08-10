using System;
using System.Linq;
using System.Collections.Generic;

namespace swept
{
    public class IssueSet
    {
        public IssueSet( 
            Clause clause, 
            SourceFile sourceFile, 
            ClauseMatchScope matchScope, 
            IList<int> matchedLines )
        {
            Clause = clause;
            SourceFile = sourceFile;
            MatchScope = matchScope;
            MatchLineNumbers = matchedLines;
        }

        // TODO: delegate to the max-arg constructor
        public IssueSet( Clause clause, SourceFile file )
        {
            clause.DoesMatch( file );

            Clause = clause;
            SourceFile = file;

            MatchScope = clause.MatchScope;
            MatchLineNumbers = clause.GetMatchList();
        }

        // TODO: make MatchLineNumbers independent
        public IssueSet( IssueSet clone ) : this( clone.Clause, clone.SourceFile, clone.MatchScope, clone.MatchLineNumbers )
        {
        }


        public Clause Clause { get; private set; }
        public SourceFile SourceFile { get; private set; }
        public ClauseMatchScope MatchScope { get; private set; }
        public IList<int> MatchLineNumbers { get; private set; }
        public bool DoesMatch
        {
            get
            {
                return MatchLineNumbers.Any();
            }
        }

        public void UniteLines( IList<int> lines )
        {
            MatchLineNumbers = MatchLineNumbers.Union( lines ).ToList();
        }

        public void IntersectLines( IList<int> lines )
        {
            MatchLineNumbers = MatchLineNumbers.Intersect( lines ).ToList();
        }

        public void SubtractLines( IList<int> lines )
        {
            MatchLineNumbers = MatchLineNumbers.ToList();
            foreach (int line in lines)
            {
                if (MatchLineNumbers.Contains( line ))
                    MatchLineNumbers.Remove( line );
            }
        }

    }
}
