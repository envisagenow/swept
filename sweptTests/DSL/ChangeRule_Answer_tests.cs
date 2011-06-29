//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2011 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using NUnit.Framework;

namespace swept.DSL.Tests
{
    [TestFixture]
    public class ChangeRule_Answer_tests : ChangeRule_tests
    {
        [Test]
        public void Language_Answers()
        {
            var parser = GetChangeRuleParser( "f.l CSharp" );
            var query = parser.expression();
            

            ClauseMatch answer = query.Answer( new SourceFile( "foo.cs" ) );

            Assert.That( answer.DoesMatch );

            answer = query.Answer( new SourceFile( "foo.csproj" ) );

            Assert.False( answer.DoesMatch );
        }
    }
}
