//  Swept:  Software Enhancement Progress Tracking.  Copyright 2008 Envisage Technologies, all rights reserved.
//  This software is open source, under the terms of the MIT License.
using NUnit.Framework;
using swept;
using System.Collections.Generic;
using System.Text;
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
