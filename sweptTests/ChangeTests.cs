//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2011 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using NUnit.Framework;
using swept;
using System;
using System.Collections.Generic;

namespace swept.Tests
{
    [TestFixture]
    public class ChangeTests
    {
        [Test]
        public void collects_SeeAlsos()
        {
            Change change = new Change();

            change.SeeAlsos = new List<SeeAlso>();
            change.SeeAlsos.Add( new SeeAlso { Description = "Go here", Target = "here.com", TargetType = TargetType.URL } );
            // TODO: finish this test
        }

        [Test]
        public void BuildFail_attribute_has_sane_default_and_settability()
        {
            Change change = new Change();

            Assert.That( change.BuildFail, Is.EqualTo( BuildFailMode.None ) );

            change.BuildFail = BuildFailMode.Any;

            Assert.That( change.BuildFail, Is.EqualTo( BuildFailMode.Any ) );
        }

    }
}
