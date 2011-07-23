//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using NUnit.Framework;
using swept;
using System;

namespace swept.Tests
{
    [CoverageExclude]
    [TestFixture]
    public class SourceFileCatalogTests
    {
        private SourceFileCatalog fileCat;
        private ChangeCatalog changeCat;
        private SourceFileCatalog blackCat;
        private SourceFileCatalog whiteCat;


        [SetUp]
        public void within_the_context_of()
        {
            fileCat = new SourceFileCatalog();
            blackCat = new SourceFileCatalog();
            whiteCat = new SourceFileCatalog();
            
            changeCat = new ChangeCatalog();
            fileCat.ChangeCatalog = changeCat;
            fileCat.SolutionPath = @"c:\odd\location\for\my.sln";
        }

        [Test]
        public void SolutionRelativeName_trims_path_in_common_with_solution()
        {
            string newFilePath = @"c:\odd\location\for\project\holding\my.cs";
            Assert.That( fileCat.SolutionRelativeName( newFilePath ), Is.EqualTo( @"project\holding\my.cs" ) );
        }

        [Test]
        public void SolutionRelativeName_does_not_alter_paths_of_external_files()
        {
            fileCat.SolutionPath = @"c:\odd\location\for\my.sln";
            string newFilePath = @"c:\different\odd\location\for\project\holding\my.cs";
            Assert.That( fileCat.SolutionRelativeName( newFilePath ), Is.EqualTo( newFilePath ) );
        }

        [Test]
        public void Can_collect_SourceFiles()
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
            SourceFile alsoBlue = fileCat.Fetch( "blue.cs" );

            Assert.AreSame( blue, alsoBlue );
        }

        [Test]
        public void Empty_Catalogs_are_Equal()
        {
            Assert.IsTrue(whiteCat.Equals(blackCat));
        }

        [Test]
        public void One_SourceFile_difference_makes_Unequal()
        {
            whiteCat.Add(new SourceFile("meow.cs"));

            Assert.IsFalse(whiteCat.Equals(blackCat));
        }


        [Test]
        public void Different_FileNames_makes_Unequal()
        {
            whiteCat.Add(new SourceFile("meow.cs"));
            blackCat.Add(new SourceFile("meow1.cs"));

            Assert.IsFalse(whiteCat.Equals(blackCat));
        }

        [Test]
        public void Catalogs_With_Equal_Files_Equal()
        {
            whiteCat.Add(new SourceFile("meow.cs"));
            blackCat.Add(new SourceFile("meow.cs"));

            Assert.IsTrue(whiteCat.Equals(blackCat));
            
        }
    }
}
