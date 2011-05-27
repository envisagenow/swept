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

        // TODO: make MatchLineNumbers independent
        public IssueSet( IssueSet clone )
            : this( clone.Clause, clone.SourceFile, clone.Scope, clone.LinesWhichMatch )
        {
        }


        public Clause Clause { get; private set; }
        public SourceFile SourceFile { get; private set; }
        // TODO: Push down into ScopedMatches, along with the tests for DoesMatch
        public bool DoesMatch
        {
            get { return LinesWhichMatch.Any(); }
        }


        public IssueSet Intersection( IssueSet rhs )
        {
            ScopedMatches matches = base.Intersection( rhs );
            return new IssueSet( Clause, SourceFile, matches.Scope, matches.LinesWhichMatch );
        }

        public IssueSet Subtraction( IssueSet rhs )
        {
            ScopedMatches matches = base.Subtraction( rhs );
            return new IssueSet( Clause, SourceFile, matches.Scope, matches.LinesWhichMatch );
        }

        public IssueSet Union( IssueSet rhs )
        {
            ScopedMatches matches = base.Union( rhs );
            return new IssueSet( Clause, SourceFile, matches.Scope, matches.LinesWhichMatch );
        }

    }
}
