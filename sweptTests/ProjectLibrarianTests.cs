//  Swept:  Software Enhancement Progress Tracking.  Copyright 2008 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
using NUnit.Framework;
using swept;
using System.Collections.Generic;
using System.Text;
using System;
using System.Xml;

namespace swept.Tests
{
    [TestFixture]
    public class ProjectLibrarianTests
    {
        private string _HerePath;
        private ProjectLibrarian Horace;

        [SetUp]
        public void Setup()
        {
            _HerePath = @"f:\over\here.sln";
            Horace = new ProjectLibrarian { SolutionPath = _HerePath };
        }

        [Test]
        public void CanSetSolutionPath()
        {
            Assert.AreEqual( _HerePath, Horace.SolutionPath );

            string myPath = @"c:\my\project.sln";
            Horace.SolutionPath = myPath;
            Assert.AreEqual( myPath, Horace.SolutionPath );
        }

        [Test]
        public void SeparateCatalogs_CreatedBy_SolutionPathChange()
        {
            Horace = new ProjectLibrarian();
            Horace.SolutionPath = "my/path";

            Assert.IsNotNull( Horace.InMemorySourceFiles );
            Assert.IsNotNull( Horace.LastSavedSourceFiles );
            Assert.AreNotSame( Horace.InMemorySourceFiles, Horace.LastSavedSourceFiles );
        }

        [Test]
        public void CanSave()
        {
            Horace.SaveFile( "some_file" );
        }

        [Test]
        public void CanFetchWorkingFile()
        {
            SourceFile foo = Horace.FetchWorkingFile( "foo.cs" );
        }
    }
}
