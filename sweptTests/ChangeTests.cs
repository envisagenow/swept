//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
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

        }
    }
}
