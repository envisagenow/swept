//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2013 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using System.IO;

namespace swept.Tests
{
    [TestFixture]
    public class TraverserTests
    {
        MockStorageAdapter _storage;

        [SetUp]
        public void given_a_StorageAdapter()
        {
            _storage = new MockStorageAdapter();
            _storage.CWD = "c:\\";
        }

        private void store_foo()
        {
            _storage.FilesInFolder["foo"] = new List<string> { "foo.cs", "foo.html" };
        }
        private void store_foobar()
        {
            store_foo();
            _storage.FoldersInFolder["foo"] = new List<string> { "bar" };
            _storage.FilesInFolder["foo\\bar"] = new List<string> { "bar.cs", "bar.html" };
        }
        private void store_foobarsubsub()
        {
            store_foobar();
            _storage.FoldersInFolder["foo\\bar"] = new List<string> { "subsub" };
            _storage.FilesInFolder["foo\\bar\\subsub"] = new List<string> { "sub1.cs", "sub2.html" };
        }

        [TestCase("foo")]
        [TestCase("c:\\foo")]
        public void excluding_base_folder_leads_to_null_traversal( string exclude )
        {
            store_foo();
            var argsText = new string[] { "folder:c:\\foo", "exclude:" + exclude, "library:foo.library", "history:foo.history" };
            var args = new Arguments(argsText, _storage);
            var traverser = new Traverser(args, _storage);

            var files = traverser.GetFilesToScan();

            Assert.That(files.Count(), Is.EqualTo(0));
        }

        //[Test]
        //public void excluded_folder_only_matches_complete_folder_name()
        //{
        //    var allFolders = { "bin", "combin", "binary" };
        //    //var folders = ???
        //    //Assert.That(folders, Has.Count.EqualTo(2));
        //    //Assert.That(folders[0], Is.EqualTo("combination"));
        //    //Assert.That(folders[1], Is.EqualTo("binary"));
        //}

        [Test]
        public void traversal_returns_all_filenames_in_folder()
        {
            store_foo();
            var argsText = new string[] { "folder:c:\\foo", "library:foo.library", "history:foo.history" };
            var args = new Arguments( argsText, _storage );
            var traverser = new Traverser(args, _storage);

            List<string> files = traverser.GetFilesToScan().ToList();
            Assert.That( files.Count, Is.EqualTo( 2 ) );
            Assert.That( files[0], Is.EqualTo( @"foo\foo.cs" ) );
            Assert.That( files[1], Is.EqualTo( @"foo\foo.html" ) );
        }

        [Test]
        public void traversal_returns_files_in_subfolders()
        {
            store_foobar();
            var argsText = new string[] { "folder:c:\\foo", "library:foo.library", "history:foo.history" };
            var args = new Arguments( argsText, _storage );
            var traverser = new Traverser(args, _storage);

            List<string> files = traverser.GetFilesToScan().ToList();
            Assert.That( files.Count, Is.EqualTo( 4 ) );
            Assert.That( files[2], Is.EqualTo( @"foo\bar\bar.cs" ) );
            Assert.That( files[3], Is.EqualTo( @"foo\bar\bar.html" ) );
        }

        [Test]
        public void traversal_returns_files_in_subsubfolders()
        {
            store_foobarsubsub();
            var argsText = new string[] { "folder:c:\\foo", "library:foo.library", "history:foo.history" };
            var args = new Arguments( argsText, _storage );
            var traverser = new Traverser(args, _storage);

            var files = traverser.GetFilesToScan().ToList();
            Assert.That( files[4], Is.EqualTo( @"foo\bar\subsub\sub1.cs" ) );
            Assert.That( files[5], Is.EqualTo( @"foo\bar\subsub\sub2.html" ) );
        }

        [Test]
        public void traversal_omits_excluded_subsubfolders()
        {
            store_foobarsubsub();
            var argsText = new string[] { "folder:foo", "library:foo.library", "history:foo.history", "exclude:.*ubs.*" };
            var args = new Arguments( argsText, _storage );
            var traverser = new Traverser(args, _storage);

            var files = traverser.GetFilesToScan().ToList();

            Assert.That( files.Count, Is.EqualTo( 4 ) );
            Assert.That( files[2], Is.EqualTo( @"foo\bar\bar.cs" ) );
            Assert.That( files[3], Is.EqualTo( @"foo\bar\bar.html" ) );
        }

        [Test]
        public void traversal_robust_across_arbitrary_miscapitalization()
        {
            store_foobarsubsub();
            var argsText = new string[] { "folder:foo", "library:foo.library", "history:foo.history", "exclude:.*ubs.*" };
            var args = new Arguments( argsText, _storage );
            var traverser = new Traverser(args, _storage);

            var files = traverser.GetFilesToScan().ToList();

            Assert.That( files.Count, Is.EqualTo( 4 ) );
            Assert.That( files[2], Is.EqualTo( @"foo\bar\bar.cs" ) );
            Assert.That( files[3], Is.EqualTo( @"foo\bar\bar.html" ) );
        }


        [Test]
        public void traversal_omits_file_extensions_not_whitelisted()
        {
            List<string> filesInFoo = new List<string> { "foo.cs", "foo.html", "foo.schnob", "foo.dll", "foo.pdb" };
            _storage.FilesInFolder["foo"] = filesInFoo;

            var argsText = new string[] { "folder:foo", "library:foo.library", "history:foo.history" };
            var args = new Arguments( argsText, _storage );
            var traverser = new Traverser(args, _storage);

            traverser.WhiteListPattern = @"\.(cs|html)$";
            List<string> files = traverser.GetFilesToScan().ToList();

            Assert.That( files.Count, Is.EqualTo( 2 ) );
            Assert.That( files[0], Is.EqualTo( @"foo\foo.cs" ) );
            Assert.That( files[1], Is.EqualTo( @"foo\foo.html" ) );
        }

        [Test]
        public void Exception_path_wrong_has_upgraded_message()
        {
            var ioex = new IOException( "Could not find a part of the path 'missing\\folder'." );
            _storage.GetFilesInFolder_Throw( ioex );

            var argsText = new string[] { "folder:c:\\foo", "library:foo.library", "history:foo.history" };
            var args = new Arguments( argsText, _storage );
            var traverser = new Traverser(args, _storage);

            var ex = Assert.Throws<Exception>( () => traverser.GetFilesToScan() );
            Assert.That( ex.Message.Contains( "missing\\folder'" ) );
            Assert.That( ex.Message.Contains( "Perhaps you expected a different working folder" ) );
            Assert.That( ex.Message.Contains( "'folder:' argument" ) );
        }

        [Test]
        public void Unrecognized_IOException_passed_unchanged()
        {
            string unexpectedMessage = "NOBODY expects the Spanish IOException!";
            var ioex = new IOException( unexpectedMessage );
            _storage.GetFilesInFolder_Throw( ioex );

            var argsText = new string[] { "folder:c:\\foo", "library:foo.library", "history:foo.history" };
            var args = new Arguments( argsText, _storage );
            var traverser = new Traverser(args, _storage);

            var ex = Assert.Throws<IOException>( () => traverser.GetFilesToScan() );
            Assert.That( ex.Message, Is.EqualTo( unexpectedMessage ) );
        }
    }
}
