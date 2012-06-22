//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using Antlr.Runtime;

namespace swept.DSL.Tests
{
    public class DSL_tests_base
    {
        protected ChangeRuleParser GetChangeRuleParser( string input )
        {
            var lexer = new ChangeRuleLexer( new ANTLRStringStream( input ) );
            return new ChangeRuleParser( new CommonTokenStream( lexer ) );
        }
    }
}
