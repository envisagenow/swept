//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using NUnit.Framework;
using swept;
using System;
using System.Collections.Generic;

namespace swept.Tests
{
    [CoverageExclude]
    [TestFixture]
    public class SourceFileTests
    {
        public static string _multiLineFile =
@"
axxxxxx
abxx
bcxxxx
cxxxxxxxxx
";
        private SourceFile file;
        private SourceFile bargle;
        private SourceFile bargle2;

        [SetUp]
        public void SetUp()
        {
            file = new SourceFile( "foo.cs" );
            bargle = new SourceFile("bargle.cs");
            bargle2 = new SourceFile("bargle.cs");
        }

        [Test]
        public void NameIsRemembered()
        {
            Assert.AreEqual( "foo.cs", file.Name );
        }

        [Test]
        public void ExtensionsAreKnown()
        {
            Assert.AreEqual( FileLanguage.HTML, new SourceFile( "foo.aspx" ).Language );
            Assert.AreEqual( FileLanguage.CSharp, new SourceFile( "foo.cs" ).Language );

            Assert.AreEqual( FileLanguage.None, new SourceFile( "foo" ).Language );
            Assert.AreEqual( FileLanguage.Unknown, new SourceFile( "foo.doc" ).Language );
        }

        [Test]
        public void Files_with_different_Names_Unequal()
        {
            Assert.IsFalse(file.Equals(bargle));
        }

        [Test]
        public void Same_Name_Files_are_Equal()
        {
            Assert.IsTrue(bargle.Equals(bargle2));
        }

        #region Line matching
        [Test]
        public void can_return_list_of_line_start_positions()
        {
            file.Content = _multiLineFile;

            Assert.That( file.LineIndices.Count, Is.EqualTo( 5 ) );
            Assert.That( file.LineIndices[0], Is.EqualTo( 1 ) );
            Assert.That( file.LineIndices[1], Is.EqualTo( 10 ) );
        }

        #endregion

    }
}
