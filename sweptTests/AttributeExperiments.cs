//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using NUnit.Framework;

namespace swept.Tests
{
    [TestFixture]
    public class AttributeExperiments
    {
        [Test]
        public void AttributeUsage_is_inherited_by_default()
        {
            var attr = new AttributeUsageAttribute(AttributeTargets.Method);
            Assert.IsTrue(attr.Inherited);
        }
    }
}
