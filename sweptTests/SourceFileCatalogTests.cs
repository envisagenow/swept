//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using NUnit.Framework;
using swept;
using System;
//using NUnit.Framework.SyntaxHelpers;

namespace swept.Tests
{
    [TestFixture]
    public class SourceFileCatalogTests
    {
        private SourceFile bariFile;
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
        public void can_remove_File()
        {
            var foo = fileCat.Fetch( "foo.cs" );
            Assert.AreEqual( 1, fileCat.Files.Count );
            Assert.IsFalse( foo.IsRemoved );

            fileCat.Remove( foo );
            Assert.AreEqual( 1, fileCat.Files.Count );
            Assert.IsTrue( foo.IsRemoved );
        }

        [Test]
        public void When_SourceFile_readded_user_can_choose_to_keep_history()
        {
            changeCat.Add( new Change { ID = "77" } );

            bariFile = new SourceFile( "bari.cs" );
            fileCat.Add( bariFile );
            bariFile.AddNewCompletion( "77" );
            
            fileCat.Remove( bariFile );

            MockUserAdapter mockGUI = new MockUserAdapter();
            fileCat.UserAdapter = mockGUI;

            //  When the dialog is presented, the 'user' responds 'keep', for this test
            mockGUI.KeepHistoricalResponse = true;

            // Bari has kept the completion of Change 77
            SourceFile savedBari = fileCat.Fetch( "bari.cs" );
            Assert.IsFalse( savedBari.IsRemoved );

            Assert.AreEqual( 1, savedBari.Completions.Count );
            Assert.AreEqual( "77", savedBari.Completions[0].ChangeID );
        }

        [Test]
        public void When_SourceFile_readded_user_can_choose_to_discard_history()
        {
            changeCat.Add( new Change { ID = "77" } );

            bariFile = new SourceFile( "bari.cs" );
            fileCat.Add( bariFile );
            bariFile.AddNewCompletion( "77" );

            fileCat.Remove( bariFile );

            MockUserAdapter mockGUI = new MockUserAdapter();
            fileCat.UserAdapter = mockGUI;

            //  When the dialog is presented, the 'user' responds 'discard', for this test
            mockGUI.KeepHistoricalResponse = false;

            // Bari has kept the completion of Change 77
            SourceFile savedBari = fileCat.Fetch( "bari.cs" );
            Assert.IsFalse( savedBari.IsRemoved );

            Assert.AreEqual( 0, savedBari.Completions.Count );
        }

        [Test]
        public void can_Clone_FileCatalog()
        {
            fileCat.Files.Add( new SourceFile( "flubber.cs" ) );

            SourceFileCatalog secondCat = fileCat.Clone();

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

            SourceFileCatalog clonedCat = fileCat.Clone();

            SourceFile clonedFile = clonedCat.Fetch(fileName);
            Assert.AreEqual(file.Completions.Count, clonedFile.Completions.Count);
            Assert.AreEqual(file.Name, clonedFile.Name);

            Assert.AreNotSame(file, clonedFile);
        }

        [Test]
        public void Clone_does_not_duplicate_ChangeCatalog()
        {
            SourceFileCatalog secondCat = fileCat.Clone();
            Assert.AreSame( changeCat, secondCat.ChangeCatalog );
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

        // TODO--0.2, DC: validate it's going away (#&?)
        [Test]
        public void CanRemove_ExistingFile()
        {
            var adapter = new MockUserAdapter();
            adapter.KeepHistoricalResponse = false;
            fileCat.UserAdapter = adapter;

            string blueName = "blue.cs";
            SourceFile blue = fileCat.Fetch(blueName);

            blue.Completions.Add(new Completion("id11"));
            Assert.AreEqual(1, fileCat.Files.Count);

            fileCat.Remove(blueName);

            SourceFile alsoBlue = fileCat.Fetch(blueName);
            Assert.AreEqual(0, alsoBlue.Completions.Count);
        }

        [Test]
        public void CanRemove_NonExistingFile()
        {
            int fileCount = fileCat.Files.Count;

            fileCat.Remove("blue.cs");
            Assert.AreEqual(fileCount, fileCat.Files.Count);
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

        [Test]
        public void Catalogs_with_synonymous_Files_with_different_Completions_are_Unequal()
        {
            SourceFile meow1 = new SourceFile("meow.cs");
            SourceFile meow2 = new SourceFile("meow.cs");
            meow1.AddNewCompletion("101");
            meow2.AddNewCompletion("102");

            whiteCat.Add(meow1);
            blackCat.Add(meow2);

            Assert.IsFalse(whiteCat.Equals(blackCat));
        }
    }
}
