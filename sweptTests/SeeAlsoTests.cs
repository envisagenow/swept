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
        public void SeeAlso_holds_a_URLTarget()
        {
            SeeAlso seeAlso = new SeeAlso
            {
                Description = "See Spot run.",
                Target = "http://Dev2000:8080/WhyToStringIsEvil"
            };

            Assert.That( seeAlso.Target == "http://Dev2000:8080/WhyToStringIsEvil" );
            Assert.That( seeAlso.TargetType == TargetType.URL );

            seeAlso.Target = "https://Dev2000:8080/WhyToStringIsEvil";
            Assert.That( seeAlso.TargetType == TargetType.URL );
        }

        [Test, ExpectedException( ExpectedMessage = "Swept doesn't understand the TargetType of [git]." )]
        public void SeeAlso_unknown_protocol_throws()
        {
            SeeAlso seeAlso = new SeeAlso
            {
                Description = "super-l33t source control.",
                Target = "git://totally/gplled/code"
            };
        }

        [Test]
        public void SeeAlso_holds_a_FileTarget()
        {
            SeeAlso seeAlso = new SeeAlso
            {
                Description = "This is how we want to do it now!",
                Target = "file://./new_technology.cs"
            };

            Assert.That( seeAlso.Target == "file://./new_technology.cs" );
            Assert.That( seeAlso.TargetType == TargetType.File );
        }

        [Test]
        public void SeeAlso_holds_a_SVNTarget()
        {
            SeeAlso seeAlso = new SeeAlso
            {
                Description = "This is how we want to do it now!",
                Target = "svn://somedirectory/changedClass.cs"
            };

            Assert.That( seeAlso.Target == "svn://somedirectory/changedClass.cs" );
            Assert.That( seeAlso.TargetType == TargetType.SVN );
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
