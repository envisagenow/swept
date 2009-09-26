//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
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
