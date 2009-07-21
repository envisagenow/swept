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

            Horace.HearSolutionOpened(this, Get_testfile_FileEventArgs());
            Assert.AreEqual(@"d:\code\CoolProject\mySolution.swept.library", Horace.LibraryPath);
        }

        [Test]
        public void OpenSolution_finding_Swept_Library_will_load_SourceFiles()
        {
            persister.XmlText = TestProbe.SingleFileLibrary_text;
            Horace.HearSolutionOpened(this, Get_testfile_FileEventArgs());

            SourceFile someFile = Horace.savedSourceCatalog.Fetch("some_file.cs");

            Assert.AreEqual(1, someFile.Completions.Count);
        }

        [Test]
        public void OpenSolution_finding_Swept_Library_will_load_Changes()
        {
            persister.XmlText = TestProbe.SingleChangeLibrary_text;
            Horace.HearSolutionOpened(this, Get_testfile_FileEventArgs());

            Assert.AreEqual(1, Horace.changeCatalog.changes.Count);
            Change change = Horace.changeCatalog.changes["30-Persist"];
            Assert.AreEqual( "Update to use persister", change.Description );
            Assert.AreEqual(FileLanguage.CSharp, change.Language);
        }

        [Test]
        public void OpenSolution_with_no_Swept_Library_will_start_smoothly()
        {
            persister.ThrowExceptionWhenLoadingLibrary = true; 
            
            Horace.HearSolutionOpened(this, Get_testfile_FileEventArgs());

            Assert.AreEqual(0, Horace.changeCatalog.changes.Count);
            Assert.AreEqual(0, Horace.unsavedSourceCatalog.Files.Count);
        }

        [Test]
        public void SaveSolution_will_persist_all_unsaved_SourceFiles()
        {
            string someFileName = "some_file.cs";
            SourceFile someFile = new SourceFile(someFileName);
            someFile.AddNewCompletion("007");
            Horace.unsavedSourceCatalog.Add(someFile);

            Horace.HearSolutionSaved(this, new EventArgs());            

            Assert.AreEqual(TestProbe.SingleFileLibrary_text, persister.XmlText);
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
            Assert.IsNotNull( Horace.unsavedSourceCatalog );
            Assert.IsNotNull( Horace.savedSourceCatalog );
            Assert.AreNotSame( Horace.unsavedSourceCatalog, Horace.savedSourceCatalog );
        }

        [Test]
        public void CanSave()
        {
            string someFileName = "some_file.cs";
            SourceFile someFile = new SourceFile(someFileName);
            someFile.AddNewCompletion("007");
            Horace.unsavedSourceCatalog.Add(someFile);

            FileEventArgs args = new FileEventArgs { Name = someFileName };
            Horace.HearFileSaved(this, args);

            Assert.AreEqual(TestProbe.SingleFileLibrary_text, persister.XmlText);
        }

        [Test]
        public void Can_Save_catalog_with_Change()
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
        public void When_SourceFileCatalog_Changed_it_reports_unsaved_Changes()
        {
            string someFileName = "some_file.cs";
            SourceFile someFile = new SourceFile(someFileName);
            someFile.AddNewCompletion("007");

            Assert.IsFalse(Horace.SourceFileCatalogUnsaved);

            Horace.unsavedSourceCatalog.Add(someFile);
            Assert.IsTrue(Horace.SourceFileCatalogUnsaved);
        }


        [Test]
        public void When_SourceFileCatalog_Saved_it_reports_no_unsaved_Changes()
        {
            string someFileName = "some_file.cs";
            SourceFile someFile = new SourceFile(someFileName);
            someFile.AddNewCompletion("007");
            
            Horace.unsavedSourceCatalog.Add(someFile);

            Horace.HearSolutionSaved(this, new EventArgs());
            Assert.IsFalse(Horace.SourceFileCatalogUnsaved);
        }

        [Test]
        public void CanAddChange()
        {
            Assert.AreEqual(0, Horace.changeCatalog.changes.Count);

            Change change = new Change("14", "here I am", FileLanguage.CSharp);
            Horace.HearChangeAdded(this, new ChangeEventArgs { change = change });

            Assert.AreEqual(1, Horace.changeCatalog.changes.Count);
        }

        [Test]
        public void CanAddChange_AndKeepHistoricalCompletions()
        {
            Change historicalChange = new Change("14", "here I am", FileLanguage.CSharp);
            Horace.HearChangeAdded(this, new ChangeEventArgs { change = historicalChange });
            SourceFile foo = new SourceFile("foo.cs");
            foo.Language = FileLanguage.CSharp;
            Horace.unsavedSourceCatalog.Add(foo);
            foo.Completions.Add(new Completion("14"));

            Horace.changeCatalog.Remove("14");

            //  In this case, the user chooses to keep history.
            MockDialogPresenter talker = new MockDialogPresenter();
            talker.KeepHistoricalResponse = true;
            Horace.showGUI = talker;

            Horace.HearChangeAdded(this, new ChangeEventArgs { change = historicalChange });

            Assert.AreEqual(1, foo.Completions.Count);
        }

        [Test]
        public void CanAddChange_AndDiscardHistoricalCompletions()
        {
            Change historicalChange = new Change("14", "here I am", FileLanguage.CSharp);
            Horace.HearChangeAdded(this, new ChangeEventArgs { change = historicalChange });
            SourceFile foo = new SourceFile("foo.cs");
            foo.Language = FileLanguage.CSharp;
            Horace.unsavedSourceCatalog.Add(foo);
            foo.Completions.Add(new Completion("14"));

            Horace.changeCatalog.Remove("14");

            //  In this case, the user chooses to discard history.
            MockDialogPresenter talker = new MockDialogPresenter();
            talker.KeepHistoricalResponse = false;
            Horace.showGUI = talker;

            Horace.HearChangeAdded(this, new ChangeEventArgs { change = historicalChange });

            Assert.AreEqual(0, foo.Completions.Count);
        }


    }
}
