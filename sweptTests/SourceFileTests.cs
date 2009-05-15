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
    public class SourceFileTests
    {
        private SourceFile file;

        [SetUp]
        public void SetUp()
        {
            file = new SourceFile( "foo.cs" );
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
    }
}
