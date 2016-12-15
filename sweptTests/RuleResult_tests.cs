using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace swept.Tests
{
    [TestFixture]
    public class RuleResult_tests
    {
        [TestCase(17)]
        [TestCase(66)]
        public void Result_threshold_can_be_set_manually(int threshold)
        {
            var result = new RuleResult { Threshold = threshold };

            Assert.That(result.Threshold, Is.EqualTo(threshold));
        }

        [TestCase(RuleFailOn.Any, 0)]
        [TestCase(RuleFailOn.None, int.MaxValue)]
        [TestCase(RuleFailOn.Increase, int.MaxValue)]
        public void Result_threshold_defaults_depend_on_FailMode(RuleFailOn failMode, int expectedThreshold)
        {
            var result = new RuleResult { FailOn = failMode };

            Assert.That(result.Threshold, Is.EqualTo(expectedThreshold));
        }

        [Test]
        public void OutOfRange_fails_descriptively()
        {
            var result = new RuleResult {FailOn = ((RuleFailOn)(-1))};
            var ex = Assert.Throws<Exception>(() => { var x = result.Threshold; });
            Assert.That(ex.Message, Is.EqualTo("Don't know the case [-1]."));
        }

    }
}
