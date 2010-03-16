using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using swept;

namespace swept.Tests
{
    [TestFixture]
    public class ArgumentsTests
    {
        [Test]
        public void args_split_on_colon()
        {
            var argsArray = new string[] { "library:fizzbuzz.swept.library" };
            Arguments args = new Arguments( argsArray );

            Assert.That( args.Library, Is.EqualTo( "fizzbuzz.swept.library" ) );
        }

        [Test, ExpectedException( ExpectedMessage = "Don't understand the argument [bad-argument]." )]
        public void malformed_args_throw()
        {
            var argsArray = new string[] { "bad-argument" };
            new Arguments( argsArray );
        }

        [Test, ExpectedException( ExpectedMessage = "Don't recognize the argument [oddity]." )]
        public void unknown_args_throw()
        {
            var argsArray = new string[] { "oddity:unrecognized_arg_name" };
            new Arguments( argsArray );
        }

        [Test]
        public void args_recognize_base_folder()
        {
            var argsArray = new string[] { "folder:f:\\work\\project" };
            Arguments args = new Arguments( argsArray );
            Assert.That( args.Folder, Is.EqualTo( @"f:\work\project" ) );
        }

        [Test]
        public void arguments_read_all_args()
        {
            var argsArray = new string[] { "folder:f:\\work\\project", "library:fizzbuzz.swept.library" };
            Arguments args = new Arguments( argsArray );
            Assert.That( args.Library, Is.EqualTo( "fizzbuzz.swept.library" ) );
            Assert.That( args.Folder, Is.EqualTo( @"f:\work\project" ) );
        }

        [Test, Ignore("until I've thought about it...")]
        public void all_args_are_in_dictionary()
        {
            var argsArray = new string[] { "folder:f:\\work\\project", "library:fizzbuzz.swept.library" };
            Arguments args = new Arguments( argsArray );

            // TODO: think about this... is there a use for a more flexible approach to arguments?
            //Assert.That( args.Arguments["library"], Is.EqualTo( "fizzbuzz.swept.library" ) );
        }

        [Test]
        public void args_can_exclude_multiple_folders()
        {
            var argsArray = new string[] { "exclude:lib,build,database" };
            Arguments args = new Arguments( argsArray );

            Assert.That( args.Exclude.Count(), Is.EqualTo( 3 ) );
        }
    }
}
