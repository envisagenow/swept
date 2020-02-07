//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2013 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using swept;
using NUnit.Framework;

namespace swept.Tests
{
    [TestFixture]
    public class SourceFileTests
    {
        public static string _multiLineFile =
@"
axxxxxx
abxx
bcxxxx
cxxxxxxxxx
";
        private SourceFile foo;
        private SourceFile bargle;
        private SourceFile bargle2;
        private SourceFile subbargle;

        [SetUp]
        public void SetUp()
        {
            foo = new SourceFile( "foo.cs" );
            bargle = new SourceFile( "bargle.cs" );
            bargle2 = new SourceFile( "bargle.cs" );
            subbargle = new SourceFile( "subfolder\\bargle.cs" );
        }

        [Test]
        public void Name_is_remembered()
        {
            Assert.AreEqual( "foo.cs", foo.Name );
        }

        [Test]
        public void Known_extensions_set_SourceFile_Language()
        {
            Assert.AreEqual( FileLanguage.HTML, new SourceFile( "foo.aspx" ).Language );
            Assert.AreEqual( FileLanguage.CSharp, new SourceFile( "foo.cs" ).Language );
            Assert.AreEqual( FileLanguage.CSHTML, new SourceFile( "foo.cshtml" ).Language );
            Assert.AreEqual( FileLanguage.SQL, new SourceFile( "foo.sql" ).Language );
            Assert.AreEqual(FileLanguage.CSS, new SourceFile("foo.scss").Language);
            Assert.AreEqual(FileLanguage.CSS, new SourceFile("foo.css").Language);
        }

        [Test]
        public void Unknown_extensions_set_SourceFile_Language_Unknown()
        {
            Assert.AreEqual( FileLanguage.Unknown, new SourceFile( "foo.doc" ).Language );
            Assert.AreEqual( FileLanguage.Unknown, new SourceFile( "some.querulouscat" ).Language );
            Assert.AreEqual( FileLanguage.Unknown, new SourceFile( "foo.cs.template" ).Language );
        }

        [Test]
        public void No_extension_sets_SourceFile_Language_None()
        {
            Assert.AreEqual( FileLanguage.None, new SourceFile( "foo" ).Language );
        }

        [Test]
        public void Extensions_are_case_insensitive()
        {
            Assert.AreEqual( FileLanguage.HTML, new SourceFile( "foo.Master" ).Language );
        }

        [Test]
        public void Different_name_files_are_unequal()
        {
            Assert.IsFalse( foo.Equals( bargle ) );
        }

        [Test]
        public void Same_name_files_are_equal()
        {
            Assert.IsTrue( bargle.Equals( bargle2 ) );
        }

        [Test]
        public void Same_name_different_folder_files_are_unequal()
        {
            Assert.IsFalse( bargle.Equals( subbargle ) );
        }

        [Test]
        public void can_return_list_of_line_start_positions()
        {
            foo.Content = _multiLineFile;

            Assert.That( foo.LineIndices.Count, Is.EqualTo( 5 ) );
            Assert.That( foo.LineIndices[0], Is.EqualTo( 1 ) );
            Assert.That( foo.LineIndices[1], Is.EqualTo( 10 ) );
        }


        [TestCase("q:\\archives\\project")]
        [TestCase("q:\\archives\\project\\")]
        public void StorageAdapter_can_stem_filename(string folder)
        {
            var expected = "subfolder\\file.cs";

            var fullPath = "q:\\archives\\project\\subfolder\\file.cs";

            var adapter = new StorageAdapter();
            var stemmedName = adapter.StemFileName(folder, fullPath);

            Assert.That(stemmedName, Is.EqualTo(expected));
        }

        [TestCase("q:\\archives\\reject\\")]
        [TestCase("q:\\archives\\project\\")]
        public void Stemming_throws_when_file_not_in_folder(string folder)
        {
            var fullPath = "q:\\archives\\happy_land\\subfolder\\file.cs";

            var adapter = new StorageAdapter();
            
            var ex = Assert.Throws<Exception>( () => adapter.StemFileName(folder, fullPath) );

            Assert.That(ex.Message, Is.StringContaining(folder));
            Assert.That(ex.Message, Is.StringContaining("File not found in folder"));
            Assert.That(ex.Message, Is.StringContaining(fullPath));
            Assert.That(ex.Message, Is.StringContaining("File name is"));
        }
    }
}

