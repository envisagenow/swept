//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Linq;
using NUnit.Framework;
using swept;
using System.IO;

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

        [Test, ExpectedException( ExpectedMessage = "Don't understand the input [bad-argument]." )]
        public void malformed_args_throw()
        {
            var argsArray = new string[] { "bad-argument", "library:unused" };
            new Arguments( argsArray, mockStorageAdapter, Console.Out );
        }

        [Test, ExpectedException( ExpectedMessage = "Don't recognize the argument [oddity]." )]
        public void unknown_args_throw()
        {
            var argsArray = new string[] { "oddity:unrecognized_arg_name", "library:unused" };
            new Arguments( argsArray, mockStorageAdapter, Console.Out );
        }

        [Test]
        public void Empty_args_emits_usage_message()
        {
            var argsArray = new string[] { };

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
        public void Library_is_required()
        {
            var argsArray = new string[] { "folder:f:\\work\\project" };
            var ex = Assert.Throws<Exception>( () => new Arguments( argsArray, mockStorageAdapter, Console.Out ) );
            Assert.That( ex.Message, Is.EqualTo( "The [library] argument is required." ) );
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

        // TODO: exclude every instance of ".svn", vs. exclude only a particular "images" folder.
        // probably by specifying a path...  do relative paths provide enough info, or must they be absolute?
    }
}
