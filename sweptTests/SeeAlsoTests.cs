//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using NUnit.Framework;
using swept;

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
        public void clone_duplicates_all_fields()
        {
            SeeAlso seeAlso = new SeeAlso
            {
                Description = "This is how we want to do it now!",
                Target = "svn://somedirectory/changedClass.cs",
                TargetType = TargetType.SVN,
                Commit = "1234"
            };

            SeeAlso duplicate = seeAlso.Clone();

            Assert.That( seeAlso.Description, Is.EqualTo( duplicate.Description ) );
            Assert.That( seeAlso.Target, Is.EqualTo( duplicate.Target ) );
            Assert.That( seeAlso.TargetType, Is.EqualTo( duplicate.TargetType ) );
            Assert.That( seeAlso.Commit, Is.EqualTo( duplicate.Commit ) );
        }

        [Test]
        public void Equals_compares_all_fields()
        {
            SeeAlso seeAlso = new SeeAlso
            {
                Description = "This is how we want to do it now!",
                Target = "svn://somedirectory/changedClass.cs",
                TargetType = TargetType.SVN,
                Commit = "1234"
            };

            var second = seeAlso.Clone();
            Assert.That( second, Is.EqualTo( seeAlso ) );

            second.Description = "x";
            Assert.That( second, Is.Not.EqualTo( seeAlso ) );

            second = seeAlso.Clone();
            second.Target = "x";
            Assert.That( second, Is.Not.EqualTo( seeAlso ) );

            second = seeAlso.Clone();
            second.TargetType = TargetType.File;
            Assert.That( second, Is.Not.EqualTo( seeAlso ) );

            second = seeAlso.Clone();
            second.Commit = "x";
            Assert.That( second, Is.Not.EqualTo( seeAlso ) );
        }
    }
}
