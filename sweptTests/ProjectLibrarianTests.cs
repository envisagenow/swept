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
            FileEventArgs args = new FileEventArgs { Name = "some_file" };
            Horace.SaveFile( this, args );
        }

        [Test]
        public void CanFetchWorkingFile()
        {
            SourceFile foo = Horace.FetchWorkingFile( "foo.cs" );
        }

        [Test]
        public void CanAddChange()
        {
            Assert.AreEqual(0, Horace.changeCatalog.changes.Count);

            Horace.AddChange(new Change("14", "here I am", FileLanguage.CSharp));

            Assert.AreEqual(1, Horace.changeCatalog.changes.Count);
        }

        [Test]
        public void CanAddChange_AndKeepHistoricalCompletions()
        {
            Change historicalChange = new Change("14", "here I am", FileLanguage.CSharp);
            Horace.AddChange(historicalChange);
            SourceFile foo = new SourceFile("foo.cs");
            foo.Language = FileLanguage.CSharp;
            Horace.InMemorySourceFiles.Add(foo);
            foo.Completions.Add(new Completion("14"));

            Horace.changeCatalog.Remove("14");

            //  In this case, the user chooses to keep history...somehow.
            Horace.AddChange(historicalChange);

            Assert.AreEqual(1, foo.Completions.Count);
        }

        [Test]
        public void CanAddChange_AndDiscardHistoricalCompletions()
        {
            Change historicalChange = new Change("14", "here I am", FileLanguage.CSharp);
            Horace.AddChange(historicalChange);
            SourceFile foo = new SourceFile("foo.cs");
            foo.Language = FileLanguage.CSharp;
            Horace.InMemorySourceFiles.Add(foo);
            foo.Completions.Add(new Completion("14"));

            Horace.changeCatalog.Remove("14");

            //  In this case, the user chooses to discard history...somehow.
            Horace._keepHistory = false;
            Horace.AddChange(historicalChange);

            Assert.AreEqual(0, foo.Completions.Count);
        }


    }
}
