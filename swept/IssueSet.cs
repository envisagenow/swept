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
        public IssueSet( IssueSet clone ) : this( clone.Clause, clone.SourceFile, clone.Scope, clone )
        {
        }


        public Clause Clause { get; private set; }
        public SourceFile SourceFile { get; private set; }
        public bool DoesMatch
        {
            get { return this.Any(); }
        }


        public IssueSet Intersection( IssueSet rhs )
        {
            ScopedMatches matches = base.Intersection( rhs );
            return new IssueSet( Clause, SourceFile, matches.Scope, matches );
        }

        public IssueSet Subtraction( IssueSet rhs )
        {
            ScopedMatches matches = base.Subtraction( rhs );
            return new IssueSet( Clause, SourceFile, matches.Scope, matches );
        }

        public IssueSet Union( IssueSet rhs )
        {
            ScopedMatches matches = base.Union( rhs );
            return new IssueSet( Clause, SourceFile, matches.Scope, matches );
        }

    }
}
