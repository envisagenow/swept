//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2011 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Linq;
using NUnit.Framework;
using System.IO;
using System.Collections.Generic;

namespace swept.Tests
{
    [TestFixture]
    public class ArgumentsTests
    {
        MockStorageAdapter mockStorageAdapter;

        [SetUp]
        public void SetUp()
        {
            mockStorageAdapter = new MockStorageAdapter();
            mockStorageAdapter.CWD = @"d:\code\project";
        }

        [Test]
        public void malformed_args_throw()
        {
            var argsArray = new string[] { "bad-argument", "library:unused" };
            TestDelegate createBadArgs = () => new Arguments( argsArray, mockStorageAdapter, Console.Out );

            var ex = Assert.Throws<Exception>( createBadArgs );

            Assert.That( ex.Message, Is.EqualTo( "Don't understand the input [bad-argument].  Try 'sweptconsole h' for help with arguments." ) );
        }

        [Test]
        public void unknown_args_throw()
        {
            var argsArray = new string[] { "oddity:unrecognized_arg_name", "library:unused" };
            TestDelegate createBadArgs = () => new Arguments( argsArray, mockStorageAdapter, Console.Out );

            var ex = Assert.Throws<Exception>( createBadArgs );
            
            Assert.That( ex.Message, Is.EqualTo( "Don't recognize the argument [oddity]." ) );
        }

        [TestCase( "usage" )]
        [TestCase( "help" )]
        [TestCase( "/?" )]
        public void Request_for_help_works( string help )
        {
            var argsArray = new string[] { help };

            string output;
            using (StringWriter writer = new StringWriter())
            {
                var args = new Arguments( argsArray, mockStorageAdapter, writer );
                writer.Close();
                output = writer.ToString();
            }
            Assert.That( output, Is.EqualTo( Arguments.UsageMessage ) );
        }

        [Test]
        public void Version_emits_version_and_copyright()
        {
            var argsArray = new string[] { "version" };

            string output;
            using (StringWriter writer = new StringWriter())
            {
                var args = new Arguments( argsArray, mockStorageAdapter, writer );
                writer.Close();
                output = writer.ToString();
            }
            Assert.That( output, Is.EqualTo( Arguments.VersionMessage ) );
        }

        [Test]
        public void Missing_Library_arg_will_throw_if_no_library_found_in_folder()
        {
            string searchFolder = "f:\\work\\search_here";
            mockStorageAdapter.FilesInFolder[searchFolder] = new List<string>();
            var argsArray = new string[] { "folder:" + searchFolder };

            var ex = Assert.Throws<Exception>( () => new Arguments( argsArray, mockStorageAdapter, Console.Out ) );
            
            Assert.That( ex.Message, Is.EqualTo( String.Format("No library found in folder [{0}].", searchFolder ) ) );
        }

        [Test]
        public void Missing_Library_arg_will_use_single_library_found_in_folder()
        {
            string searchFolder = "f:\\work\\search_here";
            List<string> foundFiles = new List<string>();
            string foundLibrary = "found.swept.library";
            foundFiles.Add( foundLibrary );
            mockStorageAdapter.FilesInFolder[searchFolder] = foundFiles;
            var argsArray = new string[] { "folder:" + searchFolder };

            var args = new Arguments( argsArray, mockStorageAdapter, Console.Out );

            Assert.That( args.Library, Is.EqualTo( Path.Combine( searchFolder, foundLibrary ) ) );
        }

        [Test]
        public void Missing_Library_arg_will_throw_on_multiple_libraries_found_in_folder()
        {
            string searchFolder = "f:\\work\\search_here";
            List<string> foundFiles = new List<string>();
            string foundLibrary = "found.swept.library";
            foundFiles.Add( foundLibrary );
            foundFiles.Add( "another.swept.Library" );
            mockStorageAdapter.FilesInFolder[searchFolder] = foundFiles;
            var argsArray = new string[] { "folder:" + searchFolder };

            var ex = Assert.Throws<Exception>( () => new Arguments( argsArray, mockStorageAdapter, Console.Out ) );

            Assert.That( ex.Message, Is.EqualTo( String.Format( "Too many libraries (*.swept.library) found in folder [{0}].", searchFolder ) ) );
        }

        [Test]
        public void args_recognize_base_folder()
        {
            var argsArray = new string[] { "folder:f:\\work\\project", "library:c:\\foo.library" };
            Arguments args = new Arguments( argsArray, mockStorageAdapter, Console.Out );
            Assert.That( args.Folder, Is.EqualTo( @"f:\work\project" ) );
        }

        [Test]
        public void folder_is_cwd_if_not_supplied_in_args()
        {
            var argsArray = new string[] { "library:fizzbuzz.swept.library" };
            Arguments args = new Arguments( argsArray, mockStorageAdapter, Console.Out );
            Assert.That( args.Folder, Is.EqualTo( "d:\\code\\project" ) );
        }

        [Test]
        public void library_with_relative_path_has_folder_prepended()
        {
            var argsArray = new string[] { "library:fizzbuzz.swept.library" };
            Arguments args = new Arguments( argsArray, mockStorageAdapter, Console.Out );
            Assert.That( args.Library, Is.EqualTo( "d:\\code\\project\\fizzbuzz.swept.library" ) );
        }

        [Test]
        public void library_with_absolute_path_is_unchanged()
        {
            var argsArray = new string[] { "folder:f:\\work\\project", "library:E:\\work_items\\fizzbuzz.swept.library" };
            Arguments args = new Arguments( argsArray, mockStorageAdapter, Console.Out );
            Assert.That( args.Library, Is.EqualTo( "E:\\work_items\\fizzbuzz.swept.library" ) );
        }

        [Test]
        public void args_can_exclude_multiple_folders()
        {
            var argsArray = new string[] { "exclude:lib,build,database", "library:unused" };
            Arguments args = new Arguments( argsArray, mockStorageAdapter, Console.Out );

            Assert.That( args.Exclude.Count(), Is.EqualTo( 3 ) );
        }

        [Test]
        public void svnin_is_unary_bool_false_by_default()
        {
            Arguments args = new Arguments( new string[] { "library:unused" }, mockStorageAdapter, Console.Out );
            Assert.That( args.Piping, Is.False );

            args = new Arguments( new string[] { "pipe:svn", "library:unused" }, mockStorageAdapter, Console.Out );
            Assert.That( args.Piping );
            Assert.That( args.PipeSource, Is.EqualTo( PipeSource.SVN ) );
        }
    }
}
