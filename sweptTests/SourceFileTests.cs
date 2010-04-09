//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using NUnit.Framework;
using swept;
using System;

namespace swept.Tests
{
    [TestFixture]
    public class SourceFileTests
    {
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
    }
}
