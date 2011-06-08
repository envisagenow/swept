using System;
using System.Linq;
using System.Collections.Generic;

namespace swept
{
    public class IssueSet
    {
        public IssueSet( Clause clause, SourceFile sourceFile, ClauseMatch match )
        {
            Clause = clause;
            SourceFile = sourceFile;
            Match = match;
        }

        // TODO: make MatchLineNumbers independent
        public IssueSet( IssueSet clone )
            : this( clone.Clause, clone.SourceFile, clone.Match )
        {
        }


        public Clause Clause { get; private set; }
        public SourceFile SourceFile { get; private set; }
        public ClauseMatch Match { get; private set; }

        public bool DoesMatch
        {
            get { return this.Match.DoesMatch; }
        }

        public IssueSet Intersection( IssueSet rhs )
        {
            ClauseMatch matches = Match.Intersection( rhs.Match );
            return new IssueSet( Clause, SourceFile, matches );
        }

        public IssueSet Union( IssueSet rhs )
        {
            ClauseMatch matches = Match.Union( rhs.Match );
            return new IssueSet( Clause, SourceFile, matches);
        }

        public IssueSet Subtraction( IssueSet rhs )
        {
            ClauseMatch matches = Match.Subtraction( rhs.Match );
            return new IssueSet( Clause, SourceFile, matches );
        }

    }
}
