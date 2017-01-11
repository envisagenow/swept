//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2017 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace swept.Tests
{
    [TestFixture]
    public class RunChangesDigest_tests
    {
        [Test]
        public void Empty_RunChanges_creates_empty_digest()
        {
            RunChanges empRunChanges = new RunChanges { RunNumber = 17 };

            var digest = empRunChanges.CreateDigest();
            Assert.That(digest, Is.Not.Null);
            Assert.That(digest, Is.Not.SameAs(empRunChanges));
            Assert.That(digest.RunNumber, Is.EqualTo(17));
        }

        [Test]
        public void Digest_does_not_get_unchanged_rule_results()
        {

        }

        [Test]
        public void Digest_does_get_changed_rule_results()
        {

        }
    }
}
