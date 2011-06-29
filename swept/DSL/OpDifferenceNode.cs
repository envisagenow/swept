//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2011 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;

namespace swept.DSL
{
    public class OpDifferenceNode : OpBinaryNode, ISubquery
    {
        public OpDifferenceNode( ISubquery lhs, string op, ISubquery rhs )
            : base( lhs, op, rhs )
        {
        }

        public ClauseMatch Answer( SourceFile file )
        {
            return LHS.Answer( file ).Subtraction( RHS.Answer( file ) );
        }
    }
}
