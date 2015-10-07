//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2013 Jason Cole and Envisage Technologies Corp.
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
            var ex = Assert.Throws<Exception>( () => new Arguments( argsText, _storage ) );

            Assert.That( ex.Message, Is.EqualTo( "Don't understand the input [bad-argument].  Try 'swept h' for help with arguments." ) );
        }

        [Test]
        public void BreakOnDeltaDrop_default_false()
        {
            var args = new Arguments( new string[] { "library:foo" }, _storage );

            Assert.That( args.BreakOnDeltaDrop, Is.False );

            args = new Arguments( new string[] { "library:foo", "BreakOnDeltaDrop" }, _storage );

            Assert.That( args.BreakOnDeltaDrop );
        }

        [Test]
        public void parse_tags_from_args()
        {
            var args = new Arguments(new string[] { "tag:foo", "tag:bar", "library:foo" }, _storage);

            Assert.That(args.Tags.Count, Is.EqualTo(2));
            Assert.That(args.Tags[0], Is.EqualTo("foo"));
            Assert.That(args.Tags[1], Is.EqualTo("bar"));
        }

        [Test]
        public void parse_excluded_tags_from_args()
        {
            var args = new Arguments(new string[] { "tag:foo", "tag:-bar", "library:foo" }, _storage);

            Assert.That(args.Tags.Count, Is.EqualTo(2));
            Assert.That(args.Tags[0], Is.EqualTo("foo"));
            Assert.That(args.Tags[1], Is.EqualTo("-bar"));
        }

        [Test]
        public void unknown_args_throw()
        {
            var argsText = new string[] { "oddity:unrecognized_arg_name" };
            var ex = Assert.Throws<Exception>( () => new Arguments(argsText, _storage) );
            
            Assert.That( ex.Message, Is.EqualTo( "Don't recognize the argument [oddity]." ) );
        }

        [TestCase( "usage" )]
        [TestCase( "help" )]
        [TestCase( "/?" )]
        public void Help_arg_works_even_without_library_arg( string help )
        {
            var args = new Arguments( new string[] { help }, _storage );

            string output;
            using (StringWriter writer = new StringWriter())
            {
                args.DisplayMessages( writer );
                writer.Close();
                output = writer.ToString();
            }
            Assert.That( output, Is.EqualTo( Arguments.UsageMessage ) );
        }

        [Test]
        public void Version_arg_works_even_without_library_arg()
        {
            var args = new Arguments( new string[] { "version" }, _storage );
            string output;
            using (var writer = new StringWriter())
            {
                args.DisplayMessages( writer );
                writer.Close();
                output = writer.ToString();
            }
            Assert.That( output, Is.EqualTo( Arguments.VersionMessage ) );
        }

        [Test]
        public void History_arg_will_default_to_single_history_found_in_folder()
        {
            var searchFolder = "f:\\work\\search_here";
            var foundHistory = "found.swept.history";
            var foundFiles = new List<string> { foundHistory };
            _storage.FilesInFolder[searchFolder] = foundFiles;

            var argsText = new string[] { "folder:" + searchFolder };
            var args = new Arguments( argsText, _storage );

            Assert.That( args.History, Is.EqualTo( Path.Combine( searchFolder, foundHistory ) ) );
        }

        [TestCase( "foo.custom.library.name", "foo.custom.history.name" )]
        [TestCase( "foo.custom.odd", "foo.custom.odd.history" )]
        public void History_arg_will_default_to_name_from_library_name( string libName, string expectedHistoryName )
        {
            var searchFolder = "f:\\work\\search_here";
            var foundFiles = new List<string>();
            _storage.FilesInFolder[searchFolder] = foundFiles;

            var argsText = new string[] { "library:" + libName, "folder:" + searchFolder };
            var args = new Arguments( argsText, _storage );

            Assert.That( args.History, Is.EqualTo( Path.Combine( searchFolder, expectedHistoryName ) ) );
        }
        [Test]  //  same as above?
        public void Omitting_Library_arg_will_use_single_library_found_in_folder()
        {
            var searchFolder = "f:\\work\\search_here";
            var foundLibrary = "found.swept.library";
            var foundFiles = new List<string> { foundLibrary };
            _storage.FilesInFolder[searchFolder] = foundFiles;

            var argsText = new string[] { "folder:" + searchFolder };
            var args = new Arguments(argsText, _storage);

            Assert.That(args.Library, Is.EqualTo(Path.Combine(searchFolder, foundLibrary)));
        }


        [Test]
        public void Missing_Library_arg_throws_when_no_library_found_in_folder()
        {
            _storage.FilesInFolder["f:\\work\\search_here"] = new List<string>();
            var argsText = new string[] { "folder:f:\\work\\search_here" };

            var ex = Assert.Throws<Exception>( () => new Arguments( argsText, _storage ) );

            Assert.That( ex.Message, Is.EqualTo( "A library is required for Swept to run.  No library found in folder [f:\\work\\search_here]." ) );
        }

        [Test]
        public void Omitting_Library_arg_will_throw_when_multiple_libraries_found_in_folder()
        {
            string searchFolder = "f:\\work\\search_here";
            string foundLibrary = "found.swept.library";
            var foundFiles = new List<string> { foundLibrary, "another.swept.Library" };
            _storage.FilesInFolder[searchFolder] = foundFiles;
            var argsText = new string[] { "folder:" + searchFolder };

            var ex = Assert.Throws<Exception>( () => new Arguments( argsText, _storage ) );

            Assert.That( ex.Message, Is.EqualTo( String.Format( "Too many libraries (*.swept.library) found in folder [{0}].", searchFolder ) ) );
        }

        [Test]
        public void args_track_history_defaults_false()
        {
            var argsText = new string[] { "library:c:\\foo.library" };
            var args = new Arguments(argsText, _storage);
            Assert.That(args.TrackHistory, Is.False);
        }

        [Test]
        public void args_track_history_when_requested()
        {
            var argsText = new string[] { "trackHistory", "library:c:\\foo.library" };
            var args = new Arguments(argsText, _storage);
            Assert.That(args.TrackHistory, Is.True);
        }

        [Test]
        public void args_tattle_defaults_false()
        {
            var argsText = new string[] { "library:c:\\foo.library" };
            var args = new Arguments(argsText, _storage);
            Assert.That(args.Tattle, Is.False);
        }

        [Test]
        public void args_tattle_when_requested()
        {
            var argsText = new string[] { "tattle", "library:c:\\foo.library" };
            var args = new Arguments(argsText, _storage);
            Assert.That(args.Tattle, Is.True);
        }

        [Test]
        public void args_recognize_base_folder()
        {
            var argsText = new string[] { "folder:f:\\work\\project", "library:c:\\foo.library" };
            var args = new Arguments( argsText, _storage );
            Assert.That( args.Folder, Is.EqualTo( @"f:\work\project" ) );
        }

        [Test]
        public void folder_is_cwd_if_not_supplied_in_args()
        {
            var argsText = new string[] { "library:fizzbuzz.swept.library" };
            var args = new Arguments( argsText, _storage );
            Assert.That( args.Folder, Is.EqualTo( "d:\\code\\project" ) );
        }

        [Test]
        public void folder_has_cwd_prefixed_if_relative()
        {
            var argsText = new string[] { "folder:project", "library:c:\\foo.library" };
            _storage.CWD = "c:\\fun_code";
            var args = new Arguments( argsText, _storage );
            Assert.That( args.Folder, Is.EqualTo( @"c:\fun_code\project" ) );
        }

        [Test]
        public void folder_is_unchanged_if_absolute()
        {
            var argsText = new string[] { "folder:c:\\project", "library:c:\\foo.library" };
            _storage.CWD = "f:\\fun_code";
            var args = new Arguments( argsText, _storage );
            Assert.That( args.Folder, Is.EqualTo( @"c:\project" ) );
        }

        [Test]
        public void path_combine_discards_first_arg_if_second_is_fully_qualified()
        {
            string folder = Path.Combine( "f:\\fun_code", "c:\\project" );
            Assert.That( folder, Is.EqualTo( @"c:\project" ) );
        }

        [Test]
        public void library_with_relative_path_has_project_folder_prepended()
        {
            var argsText = new string[] { "library:fizzbuzz.swept.library" };
            var args = new Arguments( argsText, _storage );
            Assert.That( args.Library, Is.EqualTo( "d:\\code\\project\\fizzbuzz.swept.library" ) );
        }

        [Test]
        public void library_with_absolute_path_is_unchanged()
        {
            var argsText = new string[] { "folder:f:\\work\\project", "library:E:\\work_items\\fizzbuzz.swept.library" };
            var args = new Arguments( argsText, _storage );
            Assert.That( args.Library, Is.EqualTo( "E:\\work_items\\fizzbuzz.swept.library" ) );
        }

        [Test]
        public void args_can_exclude_multiple_folders()
        {
            var argsText = new string[] { "exclude:lib,build,database", "library:unused" };
            var args = new Arguments( argsText, _storage );

            Assert.That( args.Exclude.Count(), Is.EqualTo( 3 ) );
        }

        [Test]
        public void args_have_empty_exclude_list_if_none_specified()
        {
            var argsText = new string[] { "library:unused" };
            var args = new Arguments( argsText, _storage );

            Assert.That( args.Exclude.Count(), Is.EqualTo( 0 ) );
        }

        [Test]
        public void PipeSource_None_by_default_set_by_Arg_pipe()
        {
            var args = new Arguments( new string[] { "library:unused" }, _storage );
            Assert.That( args.PipeSource, Is.EqualTo( PipeSource.None ) );

            args = new Arguments( new string[] { "pipe:svn", "library:unused" }, _storage );
            Assert.That( args.PipeSource, Is.EqualTo( PipeSource.SVN ) );
        }

        [Test]
        public void changeset_is_filename_new_commits_by_default()
        {
            var args = new Arguments( new string[] { "library:unused" }, _storage );
            Assert.That( args.ChangeSet, Is.EqualTo( "new_commits.xml" ) );

            args = new Arguments( new string[] { "changeset:foo_changes.xml", "library:unused" }, _storage );
            Assert.That( args.ChangeSet, Is.EqualTo( "foo_changes.xml" ) );
        }

        [Test]
        public void Can_set_filename_for_Details_xml_output()
        {
            var args = new Arguments( new string[] { "library:unused" }, _storage );
            Assert.That( args.DetailsFileName, Is.Empty );

            args = new Arguments( new string[] { "details:flahnam.out", "library:unused" }, _storage );
            Assert.That( args.DetailsFileName, Is.EqualTo( "flahnam.out" ) );
        }

        [Test]
        public void Can_specify_rule_to_run()
        {
            var args = new Arguments( new string[] { "library:unused" }, _storage );
            Assert.That( args.SpecifiedRules.Count, Is.EqualTo( 0 ) );

            args = new Arguments( new string[] { "rule:INT-002", "library:unused" }, _storage );
            Assert.That( args.SpecifiedRules[0], Is.EqualTo( "INT-002" ) );
        }

        [Test]
        public void Can_specify_rules_to_run()
        {
            var args = new Arguments( new string[] { "library:unused" }, _storage );
            Assert.That( args.SpecifiedRules.Count, Is.EqualTo( 0 ) );

            args = new Arguments( new string[] { "rule:INT-002,INT-003", "library:unused" }, _storage );
            Assert.That( args.SpecifiedRules.Count, Is.EqualTo( 2 ) );
            Assert.That( args.SpecifiedRules[0], Is.EqualTo( "INT-002" ) );
            Assert.That( args.SpecifiedRules[1], Is.EqualTo( "INT-003" ) );
        }

        [Test]
        public void Can_set_filename_for_Delta_xml_output()
        {
            var args = new Arguments( new string[] { "library:unused" }, _storage );
            Assert.That( args.DeltaFileName, Is.Empty );

            args = new Arguments( new string[] { "delta:flahnam.out", "library:unused" }, _storage );
            Assert.That( args.DeltaFileName, Is.EqualTo( "flahnam.out" ) );
        }

        [Test]
        public void FillExclusions_sets_folder_exclusion_list()
        {
            var args = new Arguments( new string[] { "library:unused" }, _storage );
            Assert.That( args.Exclude, Is.Empty );

            args.FillExclusions( new List<string> { "bin", ".gitignore" } );

            Assert.That( args.Exclude.Count(), Is.EqualTo( 2 ) );
        }

        [Test]
        public void FillExclusions_will_not_set_folder_exclusion_list_if_already_set_on_commandline( )
        {
            var args = new Arguments( new string[] { "library:unused", "exclude:onlyOneFolder" } , _storage );
            Assert.That( args.Exclude.Count(), Is.EqualTo( 1 ) );

            args.FillExclusions( new List<string> { "bin", "gitignore" } );

            Assert.That( args.Exclude.Count(), Is.EqualTo( 1 ) );
        }

        [Test]
        public void AdHoc_argument_is_recognized()
        {
            var args = new Arguments(new string[] { "library:unused", "adhoc:\"^CSharp and @'Test' and ~'ExpectedException'\"" }, _storage);

            Assert.That(args.AdHoc, Is.EqualTo("^CSharp and @'Test' and ~'ExpectedException'"));
        }

        [Test]
        public void Show_argument_has_proper_unary_default()
        {
            var args = new Arguments(new string[] { "library:unused", "show" }, _storage);

            Assert.That(args.Show, Is.EqualTo("*"));
        }
        
        [TestCase("web*")]
        [TestCase("web-011")]
        [TestCase("etc-411")]
        public void Show_argument_has_proper_binary_form(string showValue)
        {
            string showArg = "show:" + showValue;
            var args = new Arguments(new string[] { "library:unused", showArg }, _storage);

            Assert.That(args.Show, Is.EqualTo(showValue));
        }

        [Test]
        public void args_file_count_limit_defaults_off()
        {
            var argsText = new string[] { "library:c:\\foo.library" };
            var args = new Arguments(argsText, _storage);
            Assert.That(args.FileCountLimit, Is.EqualTo(-1));
        }

        [Test]
        public void args_file_count_limit_is_understood()
        {
            var argsText = new string[] { "library:c:\\foo.library", "filelimit:10" };
            var args = new Arguments(argsText, _storage);
            Assert.That(args.FileCountLimit, Is.EqualTo(10));
        }

        [Test]
        public void args_are_recognized_case_insensitive()
        {
            var argsText = new string[] { "LIbrarY:c:\\foo.library" };
            var args = new Arguments(argsText, _storage);
            Assert.That(args.Library, Is.EqualTo("c:\\foo.library"));
        }


    }
}
