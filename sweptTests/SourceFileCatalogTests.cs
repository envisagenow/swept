//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
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
        public void can_create()
        {
            fileCat = new SourceFileCatalog();
            changeCat = new ChangeCatalog();
            fileCat.ChangeCatalog = changeCat;
        }

        [Test]
        public void can_Clone_FileCatalog()
        {
            fileCat.Files.Add( new SourceFile( "flubber.cs" ) );

            SourceFileCatalog secondCat = SourceFileCatalog.Clone( fileCat );

            Assert.AreEqual( fileCat.Files.Count, secondCat.Files.Count );
            Assert.AreEqual( "flubber.cs", secondCat.Files[0].Name );
        }

        [Test]
        public void Clone_includes_SourceFile_Completions()
        {
            //  I have completed change 33 in file flubber.cs
            string fileName = "flubber.cs";
            SourceFile file = new SourceFile(fileName);
            file.Completions.Add(new Completion("33"));
            fileCat.Files.Add(file);

            SourceFileCatalog clonedCat = SourceFileCatalog.Clone(fileCat);

            SourceFile clonedFile = clonedCat.Fetch(fileName);
            Assert.AreEqual(file.Completions.Count, clonedFile.Completions.Count);
            Assert.AreEqual(file.Name, clonedFile.Name);

            Assert.AreNotSame(file, clonedFile);
        }

        [Test]
        public void Clone_does_not_duplicate_ChangeCatalog()
        {
            SourceFileCatalog secondCat = SourceFileCatalog.Clone( fileCat );
            Assert.AreSame( changeCat, secondCat.ChangeCatalog );
        }

        [Test]
        public void CanCollectSourceFiles()
        {
            SourceFile entry = new SourceFile( "foo.cs" );
            fileCat.Files.Add( entry );
            Assert.AreEqual( 1, fileCat.Files.Count );
        }

        [Test]
        public void Fetch_returns_SourceFile()
        {
            SourceFile entry = new SourceFile( "foo.cs" );
            fileCat.Files.Add( entry );

            SourceFile file = fileCat.Fetch( "foo.cs" );
            Assert.AreEqual( "foo.cs", file.Name );
        }

        [Test]
        public void valid_SourceFile_returned_from_Fetching_unknown_filename()
        {
            SourceFile file = fileCat.Fetch( "unknown.html" );
            Assert.AreEqual( "unknown.html", file.Name );
            Assert.AreEqual( 0, file.Completions.Count );
        }

        [Test]
        public void will_not_add_duplicate_files_twice()
        {
            SourceFile entry1 = new SourceFile("folder1\\thing.cs");
            SourceFile entry2 = new SourceFile("folder1\\thing.cs");

            fileCat.Add(entry1);
            fileCat.Add(entry2);

            Assert.AreEqual(1, fileCat.Files.Count);
        }

        [Test]
        public void files_that_differ_in_path_are_different_SourceFiles()
        {
            SourceFile entry1 = new SourceFile("folder1\\thing.cs");
            SourceFile entry2 = new SourceFile("folder2\\thing.cs");

            fileCat.Files.Add(entry1);
            fileCat.Files.Add(entry2);

            Assert.AreEqual(2, fileCat.Files.Count);
        }

        [Test]
        public void FileCatalog_remembers_Fetched_SourceFiles()
        {
            SourceFile blue = fileCat.Fetch( "blue.cs" );

            blue.Completions.Add( new Completion( "id11" ) );
            Assert.AreEqual( 1, fileCat.Files.Count );

            SourceFile alsoBlue = fileCat.Fetch( "blue.cs" );
            Assert.AreEqual( 1, alsoBlue.Completions.Count );
            Assert.AreSame( blue, alsoBlue );
        }

        [Test]
        public void CanDelete_ExistingFile()
        {
            string blueName = "blue.cs";
            SourceFile blue = fileCat.Fetch(blueName);

            blue.Completions.Add(new Completion("id11"));
            Assert.AreEqual(1, fileCat.Files.Count);

            fileCat.Delete(blueName);

            SourceFile alsoBlue = fileCat.Fetch(blueName);
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
