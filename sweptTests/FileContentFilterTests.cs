//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace swept.Tests
{
    [TestFixture]
    public class FileContentFilterTests
    {
        [Test]
        public void Match_nothing_in_an_empty_file()
        {
            bool matches = Regex.IsMatch( "", "old_technology" );
            Assert.IsFalse( matches );
        }

        [Test]
        public void Match_content_in_first_line()
        {
            bool matches = Regex.IsMatch( "using old_technology;", "old_technology" );
            Assert.IsTrue( matches );
        }

        [Test]
        public void Match_content_in_later_lines()
        {
            string file = string.Format( "using System;{0}using old_technology;", Environment.NewLine );
            bool matches = Regex.IsMatch( file, "old_technology" );
            Assert.IsTrue( matches );
        }

        [Test]
        public void multiline_content_with_no_match()
        {
            string file = string.Format( "using System;{0}using old_technology;", Environment.NewLine );
            bool matches = Regex.IsMatch( file, "different_old_technology" );
            Assert.IsFalse( matches );
        }

    }
}
