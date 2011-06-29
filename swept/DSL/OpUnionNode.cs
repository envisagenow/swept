//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2011 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;

namespace swept.DSL
{
    public class OpUnionNode : OpBinaryNode, ISubquery
    {
        public OpUnionNode( ISubquery lhs, string op, ISubquery rhs )
            : base( lhs, op, rhs )
        {
        }

        public ClauseMatch Answer( SourceFile file )
        {
            return LHS.Answer( file ).Union( RHS.Answer( file ) );
        }
    }
}
