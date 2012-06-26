//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2012 Jason Cole and Envisage Technologies Corp.
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
        MockStorageAdapter _storage;

        [SetUp]
        public void SetUp()
        {
            _storage = new MockStorageAdapter() { CWD = @"d:\code\project" };
        }

        [Test]
        public void malformed_args_throw()
        {
            var argsText = new string[] { "bad-argument", "library:unused" };
            var ex = Assert.Throws<Exception>( () => new Arguments( argsText, _storage, Console.Out ) );

            Assert.That( ex.Message, Is.EqualTo( "Don't understand the input [bad-argument].  Try 'swept h' for help with arguments." ) );
        }

        [Test]
        public void unknown_args_throw()
        {
            var argsText = new string[] { "oddity:unrecognized_arg_name", "library:unused" };
            var ex = Assert.Throws<Exception>( () => new Arguments(argsText, _storage, Console.Out) );
            
            Assert.That( ex.Message, Is.EqualTo( "Don't recognize the argument [oddity]." ) );
        }

        [TestCase( "usage" )]
        [TestCase( "help" )]
        [TestCase( "/?" )]
        public void Request_for_help_works( string help )
        {
            string output;
            using (StringWriter writer = new StringWriter())
            {
                new Arguments( new string[] { help }, _storage, writer );
                writer.Close();
                output = writer.ToString();
            }
            Assert.That( output, Is.EqualTo( Arguments.UsageMessage ) );
        }

        [Test]
        public void Version_emits_version_and_copyright()
        {
            string output;
            using (var writer = new StringWriter())
            {
                new Arguments( new string[] { "version" }, _storage, writer );
                writer.Close();
                output = writer.ToString();
            }
            Assert.That( output, Is.EqualTo( Arguments.VersionMessage ) );
        }

        //[Test]
        //public void Check_emits_line_describing_folder_library_and_datetime()
        //{
        //    string output;
        //    using (var writer = new StringWriter())
        //    {
        //        new Arguments( new string[] { "check" }, _storage, writer );
        //        writer.Close();
        //        output = writer.ToString();
        //    }
        //    // TODO: set 'now' in the arguments, and extend the message to have the date/time
        //    Assert.That( output, Is.StringStarting( "Swept checking [somewhere] with rules in [some file] on" ) );
        //}

        [Test]
        public void Missing_History_arg_will_use_single_history_found_in_folder()
        {
            var searchFolder = "f:\\work\\search_here";
            var foundHistory = "found.swept.history";
            var foundFiles = new List<string> { foundHistory };
            _storage.FilesInFolder[searchFolder] = foundFiles;

            var argsText = new string[] { "folder:" + searchFolder };
            var args = new Arguments( argsText, _storage, Console.Out );

            Assert.That( args.History, Is.EqualTo( Path.Combine( searchFolder, foundHistory ) ) );
        }

        [TestCase( "foo.custom.library.name", "foo.custom.history.name" )]
        [TestCase( "foo.custom.odd", "foo.custom.odd.history" )]
        public void Missing_History_arg_will_assume_name_from_library_name( string libName, string expectedHistoryName )
        {
            var searchFolder = "f:\\work\\search_here";
            var foundFiles = new List<string>();
            _storage.FilesInFolder[searchFolder] = foundFiles;

            var argsText = new string[] { "library:" + libName, "folder:" + searchFolder };
            var args = new Arguments( argsText, _storage, Console.Out );

            Assert.That( args.History, Is.EqualTo( Path.Combine( searchFolder, expectedHistoryName ) ) );
        }

        [Test]
        public void Missing_Library_arg_will_throw_when_no_library_found_in_folder()
        {
            var searchFolder = "f:\\work\\search_here";
            _storage.FilesInFolder[searchFolder] = new List<string>();
            var argsText = new string[] { "folder:" + searchFolder };

            var ex = Assert.Throws<Exception>( () => new Arguments( argsText, _storage, Console.Out ) );
            
            Assert.That( ex.Message, Is.EqualTo( String.Format("No library found in folder [{0}].", searchFolder ) ) );
        }

        [Test]
        public void Omitting_Library_arg_will_use_single_library_found_in_folder()
        {
            var searchFolder = "f:\\work\\search_here";
            var foundLibrary = "found.swept.library";
            var foundFiles = new List<string> { foundLibrary };
            _storage.FilesInFolder[searchFolder] = foundFiles;

            var argsText = new string[] { "folder:" + searchFolder };
            var args = new Arguments( argsText, _storage, Console.Out );

            Assert.That( args.Library, Is.EqualTo( Path.Combine( searchFolder, foundLibrary ) ) );
        }

        [Test]
        public void Omitting_Library_arg_will_throw_when_multiple_libraries_found_in_folder()
        {
            string searchFolder = "f:\\work\\search_here";
            string foundLibrary = "found.swept.library";
            var foundFiles = new List<string> { foundLibrary, "another.swept.Library" };
            _storage.FilesInFolder[searchFolder] = foundFiles;
            var argsText = new string[] { "folder:" + searchFolder };

            var ex = Assert.Throws<Exception>( () => new Arguments( argsText, _storage, Console.Out ) );

            Assert.That( ex.Message, Is.EqualTo( String.Format( "Too many libraries (*.swept.library) found in folder [{0}].", searchFolder ) ) );
        }

        [Test]
        public void args_recognize_base_folder()
        {
            var argsText = new string[] { "folder:f:\\work\\project", "library:c:\\foo.library" };
            var args = new Arguments( argsText, _storage, Console.Out );
            Assert.That( args.Folder, Is.EqualTo( @"f:\work\project" ) );
        }

        [Test]
        public void folder_is_cwd_if_not_supplied_in_args()
        {
            var argsText = new string[] { "library:fizzbuzz.swept.library" };
            var args = new Arguments( argsText, _storage, Console.Out );
            Assert.That( args.Folder, Is.EqualTo( "d:\\code\\project" ) );
        }

        [Test]
        public void library_with_relative_path_has_project_folder_prepended()
        {
            var argsText = new string[] { "library:fizzbuzz.swept.library" };
            var args = new Arguments( argsText, _storage, Console.Out );
            Assert.That( args.Library, Is.EqualTo( "d:\\code\\project\\fizzbuzz.swept.library" ) );
        }

        [Test]
        public void library_with_absolute_path_is_unchanged()
        {
            var argsText = new string[] { "folder:f:\\work\\project", "library:E:\\work_items\\fizzbuzz.swept.library" };
            var args = new Arguments( argsText, _storage, Console.Out );
            Assert.That( args.Library, Is.EqualTo( "E:\\work_items\\fizzbuzz.swept.library" ) );
        }

        [Test]
        public void args_can_exclude_multiple_folders()
        {
            var argsText = new string[] { "exclude:lib,build,database", "library:unused" };
            var args = new Arguments( argsText, _storage, Console.Out );

            Assert.That( args.Exclude.Count(), Is.EqualTo( 3 ) );
        }

        [Test]
        public void svnin_is_unary_bool_false_by_default()
        {
            var args = new Arguments( new string[] { "library:unused" }, _storage, Console.Out );
            Assert.That( args.Piping, Is.False );

            args = new Arguments( new string[] { "pipe:svn", "library:unused" }, _storage, Console.Out );
            Assert.That( args.Piping );
            Assert.That( args.PipeSource, Is.EqualTo( PipeSource.SVN ) );
        }
    }
}
