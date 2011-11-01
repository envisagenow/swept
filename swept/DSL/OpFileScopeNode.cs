//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2011 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;

namespace swept.DSL
{
    public class OpFileScopeNode : OpUnaryNode, ISubquery
    {
        public OpFileScopeNode( string op, ISubquery rhs ) : base( op, rhs ) { }

        public ClauseMatch Answer( SourceFile file )
        {
            return new FileMatch( (RHS.Answer( file ).DoesMatch) );
        }
    }
}
