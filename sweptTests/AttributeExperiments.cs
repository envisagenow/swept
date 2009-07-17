using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
