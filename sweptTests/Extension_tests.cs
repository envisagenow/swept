//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace swept.Tests
{
    [TestFixture]
    public class Extension_tests
    {
        [Test]
        public void Plurs_pluralizes_with_s()
        {
            string converted = "thing".Plurs( 1 );
            Assert.That( converted, Is.EqualTo( "thing" ) );

            converted = "thing".Plurs( 2 );
            Assert.That( converted, Is.EqualTo( "things" ) );
        }

        [Test]
        public void Plures_pluralizes_with_es()
        {
            string converted = "church".Plures( 1 );
            Assert.That( converted, Is.EqualTo( "church" ) );

            converted = "church".Plures( 2 );
            Assert.That( converted, Is.EqualTo( "churches" ) );
        }

        [Test]
        public void Plur_takes_explicit_pluralizing_arg()
        {
            string converted = "child".Plur( 1, "children" );
            Assert.That( converted, Is.EqualTo( "child" ) );

            converted = "child".Plur( 2, "children" );
            Assert.That( converted, Is.EqualTo( "children" ) );
        }

        [TestCase( 1, "is 1 error" )]
        [TestCase( 22, "are 22 errors" )]
        public void PlurFormat_for_phrases( int count, string expected )
        {
            string converted = "is 1 error".PlurFormat( count, "are {0} errors" );
            Assert.That( converted, Is.EqualTo( expected ) );
        }
    }
}
