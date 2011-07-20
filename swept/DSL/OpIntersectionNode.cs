//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2011 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;

namespace swept.DSL
{
    public class OpIntersectionNode : OpBinaryNode, ISubquery
    {
        public OpIntersectionNode( ISubquery lhs, string op, ISubquery rhs )
            : base( lhs, op, rhs )
        {
        }

        public ClauseMatch Answer( SourceFile file )
        {
            var leftAnswer = LHS.Answer( file );
            if (!leftAnswer.DoesMatch) return leftAnswer;
            //  This short-circuit is a surprisingly powerful optimization.
            //  In domain practice, we're finding many rules start with "^CSharp and"
            //  Which is a nice fast test, compared to regexing through a few K of source.
            
            return leftAnswer.Intersection( RHS.Answer( file ) );
        }
    }
}
