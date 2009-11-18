using System;
using NUnit.Framework;
using swept;
using System.Collections.Generic;

namespace swept.Tests
{
    [TestFixture]
    public class SeeAlsoTests
    {
        [Test]
        public void SeeAlso_holds_a_description()
        {
            SeeAlso seeAlso = new SeeAlso {
                Description = "See Spot run."
            };
        }

        [Test]
        public void SeeAlso_holds_only_a_URL()
        {
            SeeAlso seeAlso = new SeeAlso {
                Description = "See Spot run.",
                URL = "Dev2000:8080/WhyToStringIsEvil"
            };

            Assert.That( string.IsNullOrEmpty( seeAlso.ProjectFile ) );
            Assert.That( string.IsNullOrEmpty( seeAlso.SVN ) );
            Assert.That( string.IsNullOrEmpty( seeAlso.Commit ) );
            Assert.That( seeAlso.URL == "Dev2000:8080/WhyToStringIsEvil" );
        }

        [Test]
        public void SeeAlso_holds_only_a_ProjectFile()
        {
            SeeAlso seeAlso = new SeeAlso
            {
                Description = "See Spot run.",
                ProjectFile = @"utility\MegaToString.cs"
            };

            Assert.That( string.IsNullOrEmpty( seeAlso.URL ) );
            Assert.That( string.IsNullOrEmpty( seeAlso.SVN ) );
            Assert.That( string.IsNullOrEmpty( seeAlso.Commit ) );
            Assert.That( seeAlso.ProjectFile == @"utility\MegaToString.cs" );
        }

        [Test]
        public void SeeAlso_holds_only_a_SVN_and_Commit()
        {
            SeeAlso seeAlso = new SeeAlso
            {
                Description = "See Spot run.",
                SVN = "TextOutputAdapter.cs",
                Commit = "3427"
            };

            Assert.That( string.IsNullOrEmpty( seeAlso.URL ) );
            Assert.That( string.IsNullOrEmpty( seeAlso.ProjectFile ) );
            Assert.That( seeAlso.SVN == "TextOutputAdapter.cs" );
            Assert.That( seeAlso.Commit == "3427" );
        }
    }
}
