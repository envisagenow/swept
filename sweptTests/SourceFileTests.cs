//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using NUnit.Framework;
using swept;
using System;

namespace swept.Tests
{
    [TestFixture]
    public class SourceFileTests
    {
        private SourceFile file;
        private SourceFile bargle;
        private SourceFile bargle2;

        [SetUp]
        public void SetUp()
        {
            file = new SourceFile( "foo.cs" );
            bargle = new SourceFile("bargle.cs");
            bargle2 = new SourceFile("bargle.cs");
        }

        [Test]
        public void NameIsRemembered()
        {
            Assert.AreEqual( "foo.cs", file.Name );
        }

        [Test]
        public void ExtensionsAreKnown()
        {
            Assert.AreEqual( FileLanguage.HTML, new SourceFile( "foo.aspx" ).Language );
            Assert.AreEqual( FileLanguage.CSharp, new SourceFile( "foo.cs" ).Language );

            Assert.AreEqual( FileLanguage.None, new SourceFile( "foo" ).Language );
            Assert.AreEqual( FileLanguage.Unknown, new SourceFile( "foo.doc" ).Language );
        }

        [Test]
        public void CanCompleteChange_ExactlyOnce()
        {
            Assert.AreEqual( 0, file.Completions.Count );

            file.MarkCompleted( "id1" );
            Assert.AreEqual( 1, file.Completions.Count );

            file.MarkCompleted( "id1" );
            Assert.AreEqual( 1, file.Completions.Count );
        }

        [Test]
        public void CanCopyCompletions()
        {
            SourceFile working = new SourceFile( "work.cs" );
            working.MarkCompleted( "boo" );
            working.MarkCompleted( "eek" );
            file.CopyCompletionsFrom( working );

            //  file should have separate copy of working's completions
            Assert.AreEqual( 2, file.Completions.Count );
            Completion workingBoo = working.Completions[0];
            Completion fileBoo = file.Completions[0];
            Assert.AreNotSame( workingBoo, fileBoo );
            Assert.AreEqual( workingBoo.ChangeID, fileBoo.ChangeID );
        }

        [Test]
        public void CanAddNewCompletion()
        {
            Assert.AreEqual( 0, file.Completions.Count );

            string arbitraryID = "arbitrary_id";
            file.AddNewCompletion( arbitraryID );

            Assert.AreEqual( 1, file.Completions.Count );
            Assert.AreEqual( arbitraryID, file.Completions[0].ChangeID );
        }

        [Test]
        public void Files_with_different_Names_Unequal()
        {
            Assert.IsFalse(file.Equals(bargle));
        }

        [Test]
        public void Same_Name_Files_are_Equal()
        {
            Assert.IsTrue(bargle.Equals(bargle2));
        }

        [Test]
        public void One_Completion_difference_makes_Unequal()
        {
            bargle.AddNewCompletion("789");
            Assert.IsFalse(bargle.Equals(bargle2));
        }

        [Test]
        public void Equal_Completions_make_Equal_Files()
        {
            bargle.AddNewCompletion("789");
            bargle2.AddNewCompletion("789");
            Assert.IsTrue(bargle.Equals(bargle2));
        }

        [Test]
        public void Unequal_Completions_make_Unequal_Files()
        {
            bargle.AddNewCompletion("456");
            bargle2.AddNewCompletion("344");
            Assert.IsFalse(bargle.Equals(bargle2));
        }
    }
}
