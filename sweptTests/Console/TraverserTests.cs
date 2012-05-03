//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using System.IO;

namespace swept.Tests
{
    [CoverageExclude]
    [TestFixture]
    public class TraverserTests
    {
        MockStorageAdapter mockStorageAdapter;

        [SetUp]
        public void given_a_StorageAdapter()
        {
            mockStorageAdapter = new MockStorageAdapter();
        }

        [Test]
        public void excluding_base_folder_leads_to_null_traversal()
        {
            List<string> filesInFoo = new List<string> { "foo.cs", "foo.html" };
            mockStorageAdapter.FilesInFolder["c:\\foo"] = filesInFoo;

            string[] argsText = { "folder:c:\\foo", "exclude:c:\\foo", "library:foo.library", "history:foo.history" };
            var args = new Arguments( argsText, null, Console.Out );
            Traverser traverser = new Traverser( args, mockStorageAdapter );

            IEnumerable<string> files = traverser.GetProjectFiles();
            Assert.That( files.ToList().Count, Is.EqualTo( 0 ) );
        }

        [Test]
        public void traversal_returns_all_filenames_in_folder()
        {
            List<string> filesInFoo = new List<string> { "foo.cs", "foo.html" };
            mockStorageAdapter.FilesInFolder["c:\\foo"] = filesInFoo;

            string[] argsText = { "folder:c:\\foo", "library:foo.library", "history:foo.history" };
            var args = new Arguments( argsText, null, null );
            Traverser traverser = new Traverser( args, mockStorageAdapter );

            IEnumerable<string> files = traverser.GetProjectFiles();
            List<string> filesFromTraverser = files.ToList();
            Assert.That( filesFromTraverser.Count, Is.EqualTo( 2 ) );
            Assert.That( filesFromTraverser[0], Is.EqualTo( @"c:\foo\foo.cs" ) );
            Assert.That( filesFromTraverser[1], Is.EqualTo( @"c:\foo\foo.html" ) );
        }

        [Test]
        public void traversal_returns_files_in_subfolders()
        {
            mockStorageAdapter.FoldersInFolder["c:\\foo"] = new List<string> { "bar" };

            List<string> filesInFooBar = new List<string> { "bar.cs", "bar.html" };
            mockStorageAdapter.FilesInFolder["c:\\foo\\bar"] = filesInFooBar;

            string[] argsText = { "folder:c:\\foo", "library:foo.library", "history:foo.history" };
            var args = new Arguments( argsText, null, null );
            Traverser traverser = new Traverser( args, mockStorageAdapter );

            IEnumerable<string> files = traverser.GetProjectFiles();

            List<string> filesFromTraverser = files.ToList();
            Assert.That( filesFromTraverser.Count, Is.EqualTo( 2 ) );
            Assert.That( filesFromTraverser[0], Is.EqualTo( @"c:\foo\bar\bar.cs" ) );
            Assert.That( filesFromTraverser[1], Is.EqualTo( @"c:\foo\bar\bar.html" ) );
        }

        [Test]
        public void traversal_returns_files_in_subsubfolders()
        {
            mockStorageAdapter.FoldersInFolder["c:\\foo"] = new List<string> { "bar" };
            mockStorageAdapter.FoldersInFolder["c:\\foo\\bar"] = new List<string> { "subsub" };

            List<string> filesInFooBar = new List<string> { "bar.cs", "bar.html" };
            List<string> filesInSubSub = new List<string> { "sub1.cs", "sub2.html" };
            mockStorageAdapter.FilesInFolder["c:\\foo\\bar"] = filesInFooBar;
            mockStorageAdapter.FilesInFolder["c:\\foo\\bar\\subsub"] = filesInSubSub;

            string[] argsText = { "folder:c:\\foo", "library:foo.library", "history:foo.history" };
            var args = new Arguments( argsText, null, null );
            Traverser traverser = new Traverser( args, mockStorageAdapter );

            IEnumerable<string> files = traverser.GetProjectFiles();

            List<string> filesFromTraverser = files.ToList();
            //Assert.That( filesFromTraverser.Count, Is.EqualTo( 4 ) );
            Assert.That( filesFromTraverser[0], Is.EqualTo( @"c:\foo\bar\bar.cs" ) );
            Assert.That( filesFromTraverser[1], Is.EqualTo( @"c:\foo\bar\bar.html" ) );
            Assert.That( filesFromTraverser[2], Is.EqualTo( @"c:\foo\bar\subsub\sub1.cs" ) );
            Assert.That( filesFromTraverser[3], Is.EqualTo( @"c:\foo\bar\subsub\sub2.html" ) );
        }

        [Test]
        public void traversal_omits_excluded_subsubfolders()
        {
            mockStorageAdapter.FoldersInFolder["c:\\foo"] = new List<string> { "bar" };
            mockStorageAdapter.FoldersInFolder["c:\\foo\\bar"] = new List<string> { "subsub" };

            List<string> filesInFooBar = new List<string> { "bar.cs", "bar.html" };
            List<string> filesInSubSub = new List<string> { "sub1.cs", "sub2.html" };
            mockStorageAdapter.FilesInFolder["c:\\foo\\bar"] = filesInFooBar;
            mockStorageAdapter.FilesInFolder["c:\\foo\\bar\\subsub"] = filesInSubSub;

            string[] argsText = { "folder:c:\\foo", "library:foo.library", "history:foo.history", "exclude:.*ubs.*" };
            var args = new Arguments( argsText, null, null );
            Traverser traverser = new Traverser( args, mockStorageAdapter );

            IEnumerable<string> files = traverser.GetProjectFiles();

            List<string> filesFromTraverser = files.ToList();
            Assert.That( filesFromTraverser.Count, Is.EqualTo( 2 ) );
            Assert.That( filesFromTraverser[0], Is.EqualTo( @"c:\foo\bar\bar.cs" ) );
            Assert.That( filesFromTraverser[1], Is.EqualTo( @"c:\foo\bar\bar.html" ) );
        }

        [Test]
        public void traversal_omits_file_extensions_not_whitelisted()
        {
            List<string> filesInFoo = new List<string> { "foo.cs", "foo.html", "foo.schnob", "foo.dll", "foo.pdb" };
            mockStorageAdapter.FilesInFolder["c:\\foo"] = filesInFoo;

            string[] argsText = { "folder:c:\\foo", "library:foo.library", "history:foo.history" };
            var args = new Arguments( argsText, null, null );
            Traverser traverser = new Traverser( args, mockStorageAdapter );
            traverser.WhiteListPattern = @"\.(cs|html)$";

            IEnumerable<string> files = traverser.GetProjectFiles();
            List<string> filesFromTraverser = files.ToList();
            Assert.That( filesFromTraverser.Count, Is.EqualTo( 2 ) );
            Assert.That( filesFromTraverser[0], Is.EqualTo( @"c:\foo\foo.cs" ) );
            Assert.That( filesFromTraverser[1], Is.EqualTo( @"c:\foo\foo.html" ) );
        }

        [Test]
        public void Exception_path_wrong_upgraded_message()
        {
            var ioex = new IOException( "Could not find a part of the path 'C:\\missing\\folder'." );
            mockStorageAdapter.GetFilesInFolder_Throw( ioex );

            string[] argsText = { "folder:c:\\foo", "library:foo.library", "history:foo.history" };
            var args = new Arguments( argsText, null, null );
            Traverser traverser = new Traverser( args, mockStorageAdapter );

            var ex = Assert.Throws<Exception>( () => traverser.GetProjectFiles() );
            Assert.That( ex.Message.Contains( "C:\\missing\\folder'" ) );
            Assert.That( ex.Message.Contains( "Perhaps you expected a different current dir." ) );
            Assert.That( ex.Message.Contains( "specify a different 'folder:' argument" ) );
        }
    }
}
