﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace swept.Tests
{

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

            string[] argsText = { "folder:c:\\foo", "exclude:c:\\foo", "library:foo.library" };
            var args = new Arguments( argsText, null );
            Traverser traverser = new Traverser( args, mockStorageAdapter );

            IEnumerable<string> files = traverser.GetProjectFiles();
            Assert.That( files.ToList().Count, Is.EqualTo( 0 ) );
        }

        // TODO: exclusions of part of the path, e.g., ".svn" folders everywhere.
        // TODO: recursion into subfolders

        [Test]
        public void traversal_returns_all_filenames_in_folder()
        {
            List<string> filesInFoo = new List<string> { "foo.cs", "foo.html" };
            mockStorageAdapter.FilesInFolder["c:\\foo"] = filesInFoo;

            string[] argsText = { "folder:c:\\foo", "library:foo.library" };
            var args = new Arguments( argsText, null );
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

            string[] argsText = { "folder:c:\\foo", "library:foo.library" };
            var args = new Arguments( argsText, null );
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

            string[] argsText = { "folder:c:\\foo", "library:foo.library" };
            var args = new Arguments( argsText, null );
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

            string[] argsText = { "folder:c:\\foo", "library:foo.library", "exclude:bar\\subsub" };
            var args = new Arguments( argsText, null );
            Traverser traverser = new Traverser( args, mockStorageAdapter );

            IEnumerable<string> files = traverser.GetProjectFiles();

            List<string> filesFromTraverser = files.ToList();
            Assert.That( filesFromTraverser.Count, Is.EqualTo( 2 ) );
            Assert.That( filesFromTraverser[0], Is.EqualTo( @"c:\foo\bar\bar.cs" ) );
            Assert.That( filesFromTraverser[1], Is.EqualTo( @"c:\foo\bar\bar.html" ) );
        }

    }
}
