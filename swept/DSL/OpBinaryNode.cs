//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2011 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;

namespace swept.DSL
{
    public class OpBinaryNode
    {
        public OpBinaryNode( ISubquery lhs, string op, ISubquery rhs )
        {
            LHS = lhs;
            Operator = op;
            RHS = rhs;
        }
        public ISubquery LHS { get; set; }
        public ISubquery RHS { get; set; }
        public string Operator { get; set; }
    }
}
