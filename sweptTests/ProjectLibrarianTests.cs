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
        public void SaveSolution_will_persist_all_unsaved_SourceFiles()
        {
            MockLibraryWriter writer = new MockLibraryWriter();
            Horace.persister = writer;

            string someFileName = "some_file.cs";
            SourceFile someFile = new SourceFile(someFileName);
            someFile.AddNewCompletion("007");
            Horace.unsavedSourceImage.Add(someFile);

            Horace.HearSolutionSaved(this, new EventArgs());

            string expectedXmlText =
@"<SweptProjectData>
<ChangeCatalog>
</ChangeCatalog>
<SourceFileCatalog>
    <SourceFile Name='some_file.cs'>
        <Completion ID='007' />
    </SourceFile>
</SourceFileCatalog>
</SweptProjectData>";

            Assert.AreEqual(expectedXmlText, writer.XmlText);
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

            Assert.IsNotNull( Horace.unsavedSourceImage );
            Assert.IsNotNull( Horace.savedSourceImage );
            Assert.AreNotSame( Horace.unsavedSourceImage, Horace.savedSourceImage );
        }

        [Test]
        public void CanSave()
        {
            MockLibraryWriter writer = new MockLibraryWriter();
            Horace.persister = writer;

            string someFileName = "some_file.cs";
            SourceFile someFile = new SourceFile(someFileName);
            someFile.AddNewCompletion("007");
            Horace.unsavedSourceImage.Add(someFile);

            FileEventArgs args = new FileEventArgs { Name = someFileName };
            Horace.HearFileSaved(this, args);

            string expectedXmlText =
@"<SweptProjectData>
<ChangeCatalog>
</ChangeCatalog>
<SourceFileCatalog>
    <SourceFile Name='some_file.cs'>
        <Completion ID='007' />
    </SourceFile>
</SourceFileCatalog>
</SweptProjectData>";

            Assert.AreEqual(expectedXmlText, writer.XmlText);
        }

        [Test]
        public void Can_Save_catalog_with_Change()
        {
            MockLibraryWriter writer = new MockLibraryWriter();
            Horace.persister = writer;

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

            Assert.AreEqual(expectedXmlText, writer.XmlText);
        }


        [Test]
        public void CanFetchWorkingFile()
        {
            SourceFile foo = Horace.FetchUnsavedFile( "foo.cs" );
        }

        [Test]
        public void CanAddChange()
        {
            Assert.AreEqual(0, Horace.changeCatalog.changes.Count);

            Change change = new Change("14", "here I am", FileLanguage.CSharp);
            Horace.AddChange(this, new ChangeEventArgs { change = change });

            Assert.AreEqual(1, Horace.changeCatalog.changes.Count);
        }

        [Test]
        public void CanAddChange_AndKeepHistoricalCompletions()
        {
            Change historicalChange = new Change("14", "here I am", FileLanguage.CSharp);
            Horace.AddChange(this, new ChangeEventArgs { change = historicalChange });
            SourceFile foo = new SourceFile("foo.cs");
            foo.Language = FileLanguage.CSharp;
            Horace.unsavedSourceImage.Add(foo);
            foo.Completions.Add(new Completion("14"));

            Horace.changeCatalog.Remove("14");

            //  In this case, the user chooses to keep history.
            MockDialogPresenter talker = new MockDialogPresenter();
            talker.KeepHistoricalResponse = true;
            Horace.showGUI = talker;

            Horace.AddChange(this, new ChangeEventArgs { change = historicalChange });

            Assert.AreEqual(1, foo.Completions.Count);
        }

        [Test]
        public void CanAddChange_AndDiscardHistoricalCompletions()
        {
            Change historicalChange = new Change("14", "here I am", FileLanguage.CSharp);
            Horace.AddChange(this, new ChangeEventArgs { change = historicalChange });
            SourceFile foo = new SourceFile("foo.cs");
            foo.Language = FileLanguage.CSharp;
            Horace.unsavedSourceImage.Add(foo);
            foo.Completions.Add(new Completion("14"));

            Horace.changeCatalog.Remove("14");

            //  In this case, the user chooses to discard history.
            MockDialogPresenter talker = new MockDialogPresenter();
            talker.KeepHistoricalResponse = false;
            Horace.showGUI = talker;

            Horace.AddChange(this, new ChangeEventArgs { change = historicalChange });

            Assert.AreEqual(0, foo.Completions.Count);
        }


    }
}
