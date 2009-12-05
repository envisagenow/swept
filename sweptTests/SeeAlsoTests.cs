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

        string fileLocation = "file://./new_technology.cs";

        [Test]
        public void SeeAlso_holds_a_FileTarget()
        {
            SeeAlso seeAlso = new SeeAlso
            {
                Description = "This is how we want to do it now!",
                Target = fileLocation,
                TargetType = TargetType.File
            };

            Assert.That( seeAlso.Target == fileLocation );
            Assert.That( seeAlso.TargetType == TargetType.File );
        }

        [Test]
        public void SeeAlso_FileTarget_might_be_svn_protocol()
        {
            SeeAlso seeAlso = new SeeAlso
            {
                Description = "This is how we want to do it now!",
                Target = fileLocation,
                TargetType = TargetType.SVN
            };

            Assert.That( seeAlso.Target == fileLocation );
            Assert.That( seeAlso.TargetType == TargetType.SVN );
        }

        [Test]
        public void SeeAlso_holds_a_SVNTarget()
        {
            SeeAlso seeAlso = new SeeAlso
            {
                Description = "This is how we want to do it now!",
                Target = "svn://somedirectory/changedClass.cs",
                TargetType = TargetType.SVN
            };

            Assert.That( seeAlso.Target == "svn://somedirectory/changedClass.cs" );
            Assert.That( seeAlso.TargetType == TargetType.SVN );
        }

        [Test]
        public void SeeAlso_holds_only_a_SVN_and_Commit()
        {
            SeeAlso seeAlso = new SeeAlso
            {
                Description = "See Spot run.",
                Target = "TextOutputAdapter.cs",
                TargetType = TargetType.SVN,
                Commit = "3427"
            };

            Assert.That( seeAlso.Target == "TextOutputAdapter.cs" );
            Assert.That( seeAlso.Commit == "3427" );
        }
    }
}
