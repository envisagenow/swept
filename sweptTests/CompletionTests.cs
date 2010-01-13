//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using NUnit.Framework;
using swept;
using System;

namespace swept.Tests
{
    [TestFixture]
    public class CompletionTests
    {
        [Test]
        public void TakesChangeID()
        {
            Completion comp = new Completion( "cr001" );
            Assert.AreEqual( "cr001", comp.ChangeID );
        }

        [Test]
        public void CanClone()
        {
            Completion original = new Completion( "finally" );
            Completion clone = original.Clone();
            Assert.AreNotSame( original, clone );
            Assert.AreEqual( original.ChangeID, clone.ChangeID );
        }
    }
}
