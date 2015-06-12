//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using NUnit.Framework;
using swept;
using System;
using System.Collections.Generic;

namespace swept.Tests
{
    [TestFixture]
    public class RuleTests
    {
        [Test]
        public void collects_SeeAlsos()
        {
            Rule rule = new Rule();
            rule.SeeAlsos.Add( new SeeAlso { Description = "Go here", Target = "here.com", TargetType = TargetType.URL } );
            // TODO: finish this test
        }

        [Test]
        public void BuildFail_Default_None_is_settable()
        {
            Rule rule = new Rule();
            Assert.That( rule.FailOn, Is.EqualTo( RuleFailOn.None ) );

            rule.FailOn = RuleFailOn.Any;
            Assert.That( rule.FailOn, Is.EqualTo( RuleFailOn.Any ) );
        }

    }
}
