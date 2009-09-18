//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using NUnit.Framework;
using swept;
using System;

namespace swept.Tests
{
    [TestFixture]
    public class ProjectLibrarianTests
    {
        private string _HerePath;
        private ProjectLibrarian Horace;
        private MockLibraryPersister persister;

        [SetUp]
        public void Setup()
        {
            _HerePath = @"f:\over\here.sln";
            Horace = new ProjectLibrarian { SolutionPath = _HerePath };

            persister = new MockLibraryPersister();
            Horace.persister = persister;
        }

        private static FileEventArgs Get_testfile_FileEventArgs()
        {
            FileEventArgs args = new FileEventArgs();
            args.Name = @"d:\code\CoolProject\mySolution.sln";
            return args;
        }

        [Test]
        public void Swept_Library_sought_in_expected_location()
        {
            Assert.AreEqual(@"f:\over\here.swept.library", Horace.LibraryPath);
            persister.ThrowExceptionWhenLoadingLibrary = true;

            Horace.Hear_SolutionOpened(this, Get_testfile_FileEventArgs());
            Assert.AreEqual(@"d:\code\CoolProject\mySolution.swept.library", Horace.LibraryPath);
        }

        [Test]
        public void OpenSolution_finding_Swept_Library_will_load_SourceFiles()
        {
            persister.XmlText = TestProbe.SingleFileLibrary_text;
            Horace.Hear_SolutionOpened(this, Get_testfile_FileEventArgs());

            SourceFile someFile = Horace.savedSourceCatalog.Fetch("some_file.cs");

            Assert.AreEqual(1, someFile.Completions.Count);
        }

        [Test]
        public void OpenSolution_finding_Swept_Library_will_load_Changes()
        {
            persister.XmlText = TestProbe.SingleChangeLibrary_text;
            Horace.Hear_SolutionOpened(this, Get_testfile_FileEventArgs());

            Assert.AreEqual(1, Horace.changeCatalog.changes.Count);
            Change change = Horace.changeCatalog.changes["30-Persist"];
            Assert.AreEqual( "Update to use persister", change.Description );
            Assert.AreEqual(FileLanguage.CSharp, change.Language);
        }

        [Test]
        public void OpenSolution_with_no_Swept_Library_will_start_smoothly()
        {
            persister.ThrowExceptionWhenLoadingLibrary = true; 
            
            Horace.Hear_SolutionOpened(this, Get_testfile_FileEventArgs());

            Assert.AreEqual(0, Horace.changeCatalog.changes.Count);
            Assert.AreEqual(0, Horace.sourceCatalog.Files.Count);
        }

        [Test]
        public void SaveSolution_will_persist_all_unsaved_SourceFiles()
        {
            string someFileName = "some_file.cs";
            SourceFile someFile = new SourceFile(someFileName);
            someFile.AddNewCompletion("007");
            Horace.sourceCatalog.Add(someFile);

            Horace.Hear_SolutionSaved(this, new EventArgs());            

            Assert.AreEqual(TestProbe.SingleFileLibrary_text, persister.XmlText);
        }

        [Test]
        public void Can_set_SolutionPath()
        {
            Assert.AreEqual( _HerePath, Horace.SolutionPath );

            string myPath = @"c:\my\project.sln";
            Horace.SolutionPath = myPath;
            Assert.AreEqual( myPath, Horace.SolutionPath );
        }

        [Test]
        public void SeparateCatalogs_CreatedBy_SolutionPathChange()
        {
            Assert.IsNotNull( Horace.sourceCatalog );
            Assert.IsNotNull( Horace.savedSourceCatalog );
            Assert.AreNotSame( Horace.sourceCatalog, Horace.savedSourceCatalog );
        }

        [Test]
        public void SaveFile_persists_file_Completions()
        {
            string someFileName = "some_file.cs";
            SourceFile someFile = new SourceFile(someFileName);
            someFile.AddNewCompletion("007");
            Horace.sourceCatalog.Add(someFile);

            FileEventArgs args = new FileEventArgs { Name = someFileName };
            Horace.Hear_FileSaved(this, args);

            Assert.AreEqual(TestProbe.SingleFileLibrary_text, persister.XmlText);
        }

        [Test]
        public void Persist_saves_ChangeCatalog()
        {
            Horace.changeCatalog.Add(new Change("Uno", "Eliminate profanity from error messages.", FileLanguage.CSharp));
            Horace.Persist();

            string expectedXmlText =
@"<SweptProjectData>
<ChangeCatalog>
    <Change ID='Uno' Description='Eliminate profanity from error messages.' Language='CSharp' />
</ChangeCatalog>
<SourceFileCatalog>
</SourceFileCatalog>
</SweptProjectData>";

            Assert.AreEqual(expectedXmlText, persister.XmlText);
        }

        [Test]
        public void When_ChangeCatalog_altered_Librarian_reports_not_saved()
        {
            Assert.IsTrue( Horace.ChangeCatalogSaved );
            Assert.IsTrue( Horace.IsSaved );

            Horace.savedChangeCatalog.Add( new Change( "eek", "A mouse", FileLanguage.CSharp ) );

            Assert.IsFalse( Horace.ChangeCatalogSaved );
            Assert.IsFalse( Horace.IsSaved );
        }


        private static SourceFile newTestingFile( string someFileName )
        {
            SourceFile someFile = new SourceFile( someFileName );
            someFile.AddNewCompletion( "007" );
            return someFile;
        }

        [Test]
        public void When_SourceFileCatalog_altered_Librarian_reports_not_saved()
        {
            Assert.IsTrue( Horace.SourceFileCatalogSaved );
            Assert.IsTrue( Horace.IsSaved );

            Horace.sourceCatalog.Add( newTestingFile( "some_file.cs" ) );

            Assert.IsFalse( Horace.SourceFileCatalogSaved );
            Assert.IsFalse( Horace.IsSaved );
        }

        [Test]
        public void When_SaveSolution_fires_Librarian_reports_saved()
        {
            Horace.sourceCatalog.Add( newTestingFile( "some_file.cs" ) );
            Assert.IsFalse( Horace.IsSaved );

            Horace.Hear_SolutionSaved( this, new EventArgs() );

            Assert.IsTrue( Horace.SourceFileCatalogSaved );
            Assert.IsTrue( Horace.IsSaved );
        }

        [Test]
        public void Can_AddChange()
        {
            Assert.AreEqual(0, Horace.changeCatalog.changes.Count);

            Change change = new Change("14", "here I am", FileLanguage.CSharp);
            Horace.Hear_ChangeAdded(this, new ChangeEventArgs { change = change });

            Assert.AreEqual(1, Horace.changeCatalog.changes.Count);
        }

        [Test]
        public void AddChange_can_keep_historical_Completions()
        {
            Change historicalChange = new Change("14", "here I am", FileLanguage.CSharp);
            Horace.Hear_ChangeAdded(this, new ChangeEventArgs { change = historicalChange });
            SourceFile foo = new SourceFile("foo.cs");
            foo.Language = FileLanguage.CSharp;
            Horace.sourceCatalog.Add(foo);
            foo.Completions.Add(new Completion("14"));

            Horace.changeCatalog.Remove("14");

            //  In this case, the user chooses to keep history.
            MockDialogPresenter talker = new MockDialogPresenter();
            talker.KeepHistoricalResponse = true;
            Horace.showGUI = talker;

            Horace.Hear_ChangeAdded(this, new ChangeEventArgs { change = historicalChange });

            Assert.AreEqual(1, foo.Completions.Count);
        }

        [Test]
        public void AddChange_can_discard_historical_Completions()
        {
            Change historicalChange = new Change("14", "here I am", FileLanguage.CSharp);
            Horace.Hear_ChangeAdded(this, new ChangeEventArgs { change = historicalChange });
            SourceFile foo = new SourceFile("foo.cs");
            foo.Language = FileLanguage.CSharp;
            Horace.sourceCatalog.Add(foo);
            foo.Completions.Add(new Completion("14"));

            Horace.changeCatalog.Remove("14");

            //  In this case, the user chooses to discard history.
            MockDialogPresenter talker = new MockDialogPresenter();
            talker.KeepHistoricalResponse = false;
            Horace.showGUI = talker;

            Horace.Hear_ChangeAdded(this, new ChangeEventArgs { change = historicalChange });

            Assert.AreEqual(0, foo.Completions.Count);
        }


    }
}
