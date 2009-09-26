//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using NUnit.Framework;
using swept;
using System;

namespace swept.Tests
{
    [TestFixture]
    public class CompletionTests
    {
        [Test]
        public void SetUp()
        {
            Completion comp = new Completion();
        }

        [Test]
        public void TakesEnhancementID()
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
