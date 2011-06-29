//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2011 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using NUnit.Framework;
using Antlr.Runtime;

namespace swept.DSL.Tests
{
    public class ChangeRule_tests
    {
        protected ChangeRuleParser GetChangeRuleParser( string input )
        {
            var lexer = new ChangeRuleLexer( new ANTLRStringStream( input ) );
            return new ChangeRuleParser( new CommonTokenStream( lexer ) );
        }
    }
}
