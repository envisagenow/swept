//  Swept:  Software Enhancement Progress Tracking.  Copyright 2008 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
using NUnit.Framework;
using swept;
using System.Collections.Generic;
using System.Text;
using System;

namespace swept.Tests
{
    [TestFixture]
    public class SourceFileCatalogTests
    {
        private SourceFileCatalog fileCat;
        private ChangeCatalog changeCat;

        [SetUp]
        public void CanCreate()
        {
            fileCat = new SourceFileCatalog();
            changeCat = new ChangeCatalog();
            fileCat.ChangeCatalog = changeCat;
        }

        [Test]
        public void OneArgConstructorClonesNewCatalog()
        {
            fileCat.Files.Add( new SourceFile( "flubber.cs" ) );

            SourceFileCatalog secondCat = new SourceFileCatalog( fileCat );

            Assert.AreEqual( fileCat.Files.Count, secondCat.Files.Count );
            Assert.AreEqual( "flubber.cs", secondCat.Files[0].Name );
        }

        [Test]
        public void CloneCopiesCompletions()
        {
            //check that completions are carried over
            SourceFile file = new SourceFile( "flubber.cs" );
            file.Completions.Add( new Completion( "33" ) );
            fileCat.Files.Add( file );
            SourceFileCatalog secondCat = new SourceFileCatalog( fileCat );

            SourceFile secondFile = secondCat.Files[0];

            Assert.AreEqual( 1, secondFile.Completions.Count );
        }

        [Test]
        public void ChangeCatalogComesOverWhenCloning()
        {
            SourceFileCatalog secondCat = new SourceFileCatalog( fileCat );
            Assert.AreEqual( changeCat, secondCat.ChangeCatalog );
        }

        [Test]
        public void CanCollectSourceFiles()
        {
            SourceFile entry = new SourceFile( "foo.cs" );
            fileCat.Files.Add( entry );
            Assert.AreEqual( 1, fileCat.Files.Count );
        }

        [Test]
        public void CanReturnSourceFile()
        {
            SourceFile entry = new SourceFile( "foo.cs" );
            fileCat.Files.Add( entry );

            SourceFile file = fileCat.FetchFile( "foo.cs" );
            Assert.AreEqual( "foo.cs", file.Name );
        }

        [Test]
        public void CanDistinguishFilesByPath()
        {
            SourceFile entry1 = new SourceFile( "folder1\\thing.cs" );
            SourceFile entry2 = new SourceFile( "folder2\\thing.cs" );

            fileCat.Files.Add( entry1 );
            fileCat.Files.Add( entry2 );

            Assert.AreEqual( 2, fileCat.Files.Count );
        }

        [Test]
        public void RecognizesSameFile()
        {
            SourceFile entry = new SourceFile( "folder1\\thing.cs" );

            fileCat.Add( entry );
            fileCat.Add( entry );

            Assert.AreEqual( 1, fileCat.Files.Count );
        }

        [Test]
        public void ValidSourceFileReturnedFromUnknownName()
        {
            SourceFile file = fileCat.FetchFile( "unknown.html" );
            Assert.AreEqual( "unknown.html", file.Name );
            Assert.AreEqual( 0, file.Completions.Count );
        }

        [Test]
        public void FileCatalogRemembersFetchedSourceFiles()
        {
            SourceFile blue = fileCat.FetchFile( "blue.cs" );

            blue.Completions.Add( new Completion( "id11" ) );
            Assert.AreEqual( 1, fileCat.Files.Count );

            SourceFile alsoBlue = fileCat.FetchFile( "blue.cs" );
            Assert.AreEqual( 1, alsoBlue.Completions.Count );
            Assert.AreSame( blue, alsoBlue );
        }

        [Test]
        public void CanDelete_ExistingFile()
        {
            string blueName = "blue.cs";
            SourceFile blue = fileCat.FetchFile(blueName);

            blue.Completions.Add(new Completion("id11"));
            Assert.AreEqual(1, fileCat.Files.Count);

            fileCat.Delete(blueName);

            SourceFile alsoBlue = fileCat.FetchFile(blueName);
            Assert.AreEqual(0, alsoBlue.Completions.Count);
        }

        [Test]
        public void CanDelete_NonExistingFile()
        {
            string blueName = "blue.cs";
            int fileCount = fileCat.Files.Count;

            fileCat.Delete(blueName);
            Assert.AreEqual(fileCount, fileCat.Files.Count);
        }
    }
}
