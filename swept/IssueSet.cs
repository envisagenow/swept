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
            List<int> matchedLines, 
            bool doesMatch )
        {
            Clause = clause;
            SourceFile = sourceFile;
            MatchScope = matchScope;
            MatchLineNumbers = matchedLines;
            DoesMatch = doesMatch;
        }

        public IssueSet( Clause clause, SourceFile file )
        {
            Clause = clause;
            SourceFile = file;

            MatchScope = clause.MatchScope;
            MatchLineNumbers = clause.GetMatchList();

            DoesMatch = clause.DoesMatch( file );
        }

        internal IssueSet( Clause clause, SourceFile file, IList<int> matchLineNumbers )
        {
            Clause = clause;
            SourceFile = file;

            MatchScope = ClauseMatchScope.Line;
            MatchLineNumbers = matchLineNumbers;
        }

        internal IssueSet( Clause clause, SourceFile file, bool matchesAtFileScope )
        {
            Clause = clause;
            SourceFile = file;

            MatchScope = ClauseMatchScope.File;
            DoesMatch = matchesAtFileScope;
        }

        public Clause Clause { get; private set; }
        public SourceFile SourceFile { get; private set; }
        public ClauseMatchScope MatchScope { get; private set; }
        public IList<int> MatchLineNumbers { get; private set; }
        public bool DoesMatch { get; private set; }

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
