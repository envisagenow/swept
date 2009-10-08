//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using NUnit.Framework;
using swept;
using System;

namespace swept.Tests
{
    [TestFixture]
    public class ChangeTests
    {
        [Test]
        public void CanCreateChange()
        {
            const string newID = "NEW";
            const string newDescription = "Brand new";
            const FileLanguage newLanguage = FileLanguage.CSharp;

            Change change = new Change( newID, newDescription, newLanguage );

            Assert.AreEqual( newID, change.ID );
            Assert.AreEqual( newDescription, change.Description );
            Assert.AreEqual( newLanguage, change.Language );
        }

        [Test]
        public void language_criterion_passes_all_when_set_to_None()
        {
            Change change = new Change
            {
                ID = "no language",
                Description = "Relevant to files of all languages.",
                Language = FileLanguage.None
            };

            Assert.IsTrue( change.PassesFilter( new SourceFile( "my.cs" ) ) );
            Assert.IsTrue( change.PassesFilter( new SourceFile( "my.html" ) ) );
            Assert.IsTrue( change.PassesFilter( new SourceFile( "my.unknownextension" ) ) );
        }

        [Test]
        public void language_criterion_filters_when_set()
        {
            Change change = new Change
            {
                ID = "set language",
                Description = "Relevant to C# files.",
                Language = FileLanguage.CSharp
            };

            Assert.IsTrue(  change.PassesFilter( new SourceFile( "my.cs" ) ) );
            Assert.IsFalse( change.PassesFilter( new SourceFile( "my.html" ) ) );
            Assert.IsFalse( change.PassesFilter( new SourceFile( "my.unknownextension" ) ) );
        }

        [Test]
        public void subpath_criterion_passes_all_when_empty()
        {
            Change change = new Change
            {
                ID = "no subpath",
                Description = "Relevant to files in all locations.",
                Subpath = ""
            };

            Assert.IsTrue( change.PassesFilter( new SourceFile( @"my.cs" ) ) );
            Assert.IsTrue( change.PassesFilter( new SourceFile( @"specified\subpath\my.cs" ) ) );
            Assert.IsTrue( change.PassesFilter( new SourceFile( @"specified\subpath\and\deeper\my.cs" ) ) );
            Assert.IsTrue( change.PassesFilter( new SourceFile( @"another\subpath\my.cs" ) ) );
        }

        [Test]
        public void subpath_criterion_filters_when_set()
        {
            Change change = new Change
            {
                ID = "specified subpath",
                Description = "Relevant to files in one subtree.",
                Subpath = @"specified\subpath"
            };

            Assert.IsFalse( change.PassesFilter( new SourceFile( @"my.cs" ) ) );
            Assert.IsTrue(  change.PassesFilter( new SourceFile( @"specified\subpath\my.cs" ) ) );
            Assert.IsTrue(  change.PassesFilter( new SourceFile( @"specified\subpath\and\deeper\my.cs" ) ) );
            Assert.IsFalse( change.PassesFilter( new SourceFile( @"another\subpath\my.cs" ) ) );
        }

        [Test]
        public void name_pattern_criterion_passes_all_when_empty()
        {
            Change change = new Change
            {
                ID = "no name pattern",
                Description = "Relevant to files of all names.",
                NamePattern = ""
            };

            Assert.IsTrue( change.PassesFilter( new SourceFile( @"myCode.cs" ) ) );
            Assert.IsTrue( change.PassesFilter( new SourceFile( @"Tests.cs" ) ) );
            Assert.IsTrue( change.PassesFilter( new SourceFile( @"myTests.cs" ) ) );
            Assert.IsTrue( change.PassesFilter( new SourceFile( @"my_tests.js" ) ) );
        }

        [Test]
        public void name_pattern_criterion_filters_when_set()
        {
            Change change = new Change
            {
                ID = "no name pattern",
                Description = "Relevant to files of all names.",
                NamePattern = "tests"
            };

            Assert.IsFalse( change.PassesFilter( new SourceFile( @"my.cs" ) ) );
            Assert.IsTrue( change.PassesFilter( new SourceFile( @"Tests.cs" ) ) );
            Assert.IsTrue( change.PassesFilter( new SourceFile( @"myTests.cs" ) ) );
            Assert.IsTrue( change.PassesFilter( new SourceFile( @"my_tests.js" ) ) );
        }

        #region Equality tests
        [Test]
        public void Can_compare_equality()
        {
            Change change1 = new Change();
            Change change2 = new Change();

            Assert.IsTrue(change1.Equals( change2 ));
        }

        [Test]
        public void Can_compare_inequal_objects()
        {
            Change change1 = new Change( "101-443", "Remove all dinguses", FileLanguage.CSharp );
            Change change2 = new Change( "5987515", "Frob all wobbishes", FileLanguage.CSS );

            Assert.IsFalse( change1.Equals( change2 ) );

            change1.ID = change2.ID;
            Assert.IsFalse( change1.Equals( change2 ) );

            change1.Description = change2.Description;
            Assert.IsFalse( change1.Equals( change2 ) );

            change1.Language = change2.Language;
            Assert.IsTrue( change1.Equals( change2 ) );
        }

        [Test]
        public void Can_compare_to_null()
        {
            Change change1 = new Change();
            Assert.IsFalse( change1.Equals( null ) );
        }
        #endregion
    }
}
