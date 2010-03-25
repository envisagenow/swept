using System;
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
            Assert.That( filesFromTraverser[0], Is.EqualTo( "foo.cs" ) );
            Assert.That( filesFromTraverser[1], Is.EqualTo( "foo.html" ) );
        }

    }
}
