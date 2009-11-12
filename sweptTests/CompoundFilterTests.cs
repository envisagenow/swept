﻿//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using NUnit.Framework;
using swept;
using System.Collections.Generic;

namespace swept.Tests
{

    [TestFixture]
    public class CompoundFilterTests
    {
        #region Compound matching
        [Test]
        public void Children_get_matched()
        {
            var child1 = new CompoundFilter { Language = FileLanguage.CSharp };
            var child2 = new CompoundFilter { NamePattern = "blue" };

            var filter = new CompoundFilter { };
            filter.Children.Add( child1 );
            filter.Children.Add( child2 );

            Assert.IsFalse( filter.Matches( new SourceFile( "my.cs" ) ) );
            Assert.IsFalse( filter.Matches( new SourceFile( "blue.html" ) ) );
            Assert.IsTrue( filter.Matches( new SourceFile( "blue.cs" ) ) );
        }

        [Test]
        public void Or_filter()
        {
            var child1 = new CompoundFilter { Language = FileLanguage.CSharp };
            var child2 = new CompoundFilter { NamePattern = "blue", Operator = FilterOperator.Or };

            var filter = new CompoundFilter { };
            filter.Children.Add( child1 );
            filter.Children.Add( child2 );

            Assert.IsTrue( filter.Matches( new SourceFile( "my.cs" ) ) );
            Assert.IsTrue( filter.Matches( new SourceFile( "blue.html" ) ) );
            Assert.IsTrue( filter.Matches( new SourceFile( "blue.cs" ) ) );
            Assert.IsFalse( filter.Matches( new SourceFile( "bluuue.css" ) ) );
        }


        [Test]
        public void Not_language_filter_passes_MISmatches_only()
        {
            var filter = new CompoundFilter
            {
                ID = "Not language",
                Description = "Relevant to everything except C# files.",
                Language = FileLanguage.CSharp,
                Operator = FilterOperator.Not,
            };

            Assert.IsFalse( filter.Matches( new SourceFile( "my.cs" ) ) );
            Assert.IsTrue( filter.Matches( new SourceFile( "my.html" ) ) );
            Assert.IsTrue( filter.Matches( new SourceFile( "my.unknownextension" ) ) );
        }

        [Test]
        public void NotFilter_inverts_decision()
        {
            CompoundFilter filter = new CompoundFilter
            {
                ID = "no_tests",
                Description = "Skip the test files",
                NamePattern = "tests",
                Operator = FilterOperator.Not
            };

            Assert.IsTrue( filter.Matches( new SourceFile( @"my_test.cs" ) ) );
            Assert.IsFalse( filter.Matches( new SourceFile( @"Tests.cs" ) ) );
        }

        [Test]
        public void Filter_passes_depending_on_internal_filters()
        {
            CompoundFilter child = new CompoundFilter
            {
                ID = "Tests",
                Description = "Give me just the unit tests.",
                NamePattern = "tests"
            };

            CompoundFilter filter = new CompoundFilter
            {
                Children = new List<CompoundFilter> { child }
            };

            Assert.IsFalse( filter.Matches( new SourceFile( @"my_test.cs" ) ) );
            Assert.IsTrue( filter.Matches( new SourceFile( @"Tests.cs" ) ) );
        }

        [Test]
        public void either_of_or_filter_passing_passes()
        {
            CompoundFilter one = new CompoundFilter
            {
                ID = "one",
                Description = "files named one",
                NamePattern = "one"
            };

            CompoundFilter two = new CompoundFilter
            {
                ID = "two",
                Description = "files named two",
                NamePattern = "two",
                Operator = FilterOperator.Or,
            };

            CompoundFilter filter = new CompoundFilter
            {
                Children = new List<CompoundFilter> { one, two }
            };

            Assert.IsTrue( filter.Matches( new SourceFile( @"my_one.cs" ) ) );
            Assert.IsTrue( filter.Matches( new SourceFile( @"my_two.cs" ) ) );
            Assert.IsFalse( filter.Matches( new SourceFile( @"my_three.cs" ) ) );
            Assert.IsTrue( filter.Matches( new SourceFile( @"my_one_two.cs" ) ) );
        }

        #endregion

        #region Simple filter functionality

        [Test]
        public void empty_filter_matches_any_file()
        {
            var filter = new CompoundFilter();
            var file = new SourceFile( @"\path\file.ext" );
            Assert.That( filter.Matches( file ) );
        }

        [Test]
        public void language_filter_passes_all_when_set_to_None()
        {
            CompoundFilter filter = new CompoundFilter
            {
                ID = "no language",
                Description = "Relevant to files of all languages.",
                Language = FileLanguage.None
            };

            Assert.IsTrue( filter.Matches( new SourceFile( "my.cs" ) ) );
            Assert.IsTrue( filter.Matches( new SourceFile( "my.html" ) ) );
            Assert.IsTrue( filter.Matches( new SourceFile( "my.unknownextension" ) ) );
        }

        [Test]
        public void language_filter_passes_matches_only()
        {
            CompoundFilter filter = new CompoundFilter
            {
                ID = "set language",
                Description = "Relevant to C# files.",
                Language = FileLanguage.CSharp
            };

            Assert.IsTrue( filter.Matches( new SourceFile( "my.cs" ) ) );
            Assert.IsFalse( filter.Matches( new SourceFile( "my.html" ) ) );
            Assert.IsFalse( filter.Matches( new SourceFile( "my.unknownextension" ) ) );
        }

        [Test]
        public void subpath_filter_passes_all_when_empty()
        {
            CompoundFilter filter = new CompoundFilter
            {
                ID = "no subpath",
                Description = "Relevant to files in all locations.",
                Subpath = ""
            };

            Assert.IsTrue( filter.Matches( new SourceFile( @"my.cs" ) ) );
            Assert.IsTrue( filter.Matches( new SourceFile( @"specified\subpath\my.cs" ) ) );
            Assert.IsTrue( filter.Matches( new SourceFile( @"specified\subpath\and\deeper\my.cs" ) ) );
            Assert.IsTrue( filter.Matches( new SourceFile( @"another\subpath\my.cs" ) ) );
        }

        [Test]
        public void subpath_filter_passes_matches_only()
        {
            CompoundFilter filter = new CompoundFilter
            {
                ID = "specified subpath",
                Description = "Relevant to files in one subtree.",
                Subpath = @"specified\subpath"
            };

            Assert.IsFalse( filter.Matches( new SourceFile( @"my.cs" ) ) );
            Assert.IsTrue( filter.Matches( new SourceFile( @"specified\subpath\my.cs" ) ) );
            Assert.IsTrue( filter.Matches( new SourceFile( @"specified\subpath\and\deeper\my.cs" ) ) );
            Assert.IsFalse( filter.Matches( new SourceFile( @"another\subpath\my.cs" ) ) );
        }

        [Test]
        public void name_pattern_filter_passes_all_when_empty()
        {
            CompoundFilter filter = new CompoundFilter
            {
                ID = "no name pattern",
                Description = "Relevant to files of all names.",
                NamePattern = ""
            };

            Assert.IsTrue( filter.Matches( new SourceFile( @"myCode.cs" ) ) );
            Assert.IsTrue( filter.Matches( new SourceFile( @"Tests.cs" ) ) );
            Assert.IsTrue( filter.Matches( new SourceFile( @"myTests.cs" ) ) );
            Assert.IsTrue( filter.Matches( new SourceFile( @"my_tests.js" ) ) );
        }

        [Test]
        public void name_pattern_filter_passes_matches_only()
        {
            CompoundFilter filter = new CompoundFilter
            {
                ID = "no name pattern",
                Description = "Relevant to files of all names.",
                NamePattern = "tests"
            };

            Assert.IsFalse( filter.Matches( new SourceFile( @"my_test.cs" ) ) );
            Assert.IsTrue( filter.Matches( new SourceFile( @"Tests.cs" ) ) );
            Assert.IsTrue( filter.Matches( new SourceFile( @"myTests.cs" ) ) );
            Assert.IsTrue( filter.Matches( new SourceFile( @"my_tests.js" ) ) );
        }

        #endregion

        #region File Content Criteria

        [Test]
        public void content_pattern_misses_file_lacking_regex_match()
        {
            var filter = new CompoundFilter { ContentPattern = "(foo|bar)" };
            var file = new SourceFile( "foo.cs" );
            file.Content = "using System;";
            Assert.That( filter.Matches( file ), Is.False );
        }

        [Test]
        public void content_pattern_matches_file_containing_regex_match()
        {
            var filter = new CompoundFilter { ContentPattern = "(foo|bar)" };
            var file = new SourceFile( "foo.cs" );
            file.Content = "using System.foo;";
            Assert.That( filter.Matches( file ), Is.True );
        }

        #endregion

        #region Name reporting
        [Test]
        public void default_name_is_And()
        {
            CompoundFilter filter = new CompoundFilter();
            Assert.That( filter.Name, Is.EqualTo( "And" ) );
        }

        [Test]
        public void eldest_name_is_Change()
        {
            CompoundFilter filter = new CompoundFilter();
            filter.Eldest = true;
            Assert.That( filter.Name, Is.EqualTo( "Change" ) );
        }

        [Test]
        public void name_generally_matches_operator()
        {
            CompoundFilter filter = new CompoundFilter();

            filter.Operator = FilterOperator.And;
            Assert.That( filter.Name, Is.EqualTo( "And" ) );
            filter.Operator = FilterOperator.Not;
            Assert.That( filter.Name, Is.EqualTo( "AndNot" ) );
            filter.Operator = FilterOperator.Or;
            Assert.That( filter.Name, Is.EqualTo( "Or" ) );
        }

        [Test]
        public void first_child_name_is_different()
        {
            CompoundFilter filter = new CompoundFilter();
            filter.FirstChild = true;

            filter.Operator = FilterOperator.And;
            Assert.That( filter.Name, Is.EqualTo( "When" ) );
            filter.Operator = FilterOperator.Not;
            Assert.That( filter.Name, Is.EqualTo( "Not" ) );
            filter.Operator = FilterOperator.Or;
            Assert.That( filter.Name, Is.EqualTo( "Either" ) );
        }

        [Test, ExpectedException( ExpectedMessage = "Can't get Name for Operator [-1]." )]
        public void operator_out_of_range_is_caught()
        {
            CompoundFilter filter = new CompoundFilter { Operator = (FilterOperator)(-1) };
            string never_seen = filter.Name;
        }

        [Test]
        public void can_mark_all_first_children()
        {
            CompoundFilter filter = new CompoundFilter();
            filter.Children.Add( new CompoundFilter() );
            filter.Children.Add( new CompoundFilter() );
            filter.Children.Add( new CompoundFilter() );
            filter.Children[1].Children.Add( new CompoundFilter() );
            filter.Children[1].Children.Add( new CompoundFilter() );
            filter.markFirstChildren();

            Assert_first_children_correct( filter );

            interregnum_pretendership( filter );
            filter.markFirstChildren();

            Assert_first_children_correct( filter );
        }

        private static void Assert_first_children_correct( CompoundFilter filter )
        {
            Assert.True( filter.FirstChild );

            Assert.True( filter.Children[0].FirstChild );
            Assert.False( filter.Children[1].FirstChild );
            Assert.False( filter.Children[2].FirstChild );

            Assert.True( filter.Children[1].Children[0].FirstChild );
            Assert.False( filter.Children[1].Children[1].FirstChild );
        }

        private void interregnum_pretendership( CompoundFilter filter )
        {
            filter.FirstChild = true;
            filter.Children.ForEach( child => interregnum_pretendership( child ) );
        }
        #endregion

        #region Equality
        [Test]
        public void Can_compare_equality()
        {
            CompoundFilter filter1 = new Change();
            CompoundFilter filter2 = new Change();

            Assert.IsTrue( filter1.Equals( filter2 ) );
        }

        // TODO: relocate, expand attributes being compared
        [Test]
        public void Can_compare_inequal_objects()
        {
            CompoundFilter filter1 = new CompoundFilter { 
                ID = "101-443", Description = "Remove all dinguses", Operator = FilterOperator.And,
                Subpath = @"\here\", NamePattern = "this", Language = FileLanguage.CSharp,
                ContentPattern = "old_technology"
            };
            CompoundFilter filter2 = new CompoundFilter { 
                ID = "5987515", Description = "Frob all wobbishes", Operator = FilterOperator.Not,
                Subpath = @"\there\", NamePattern = "that", Language = FileLanguage.CSS,
                ContentPattern = "old_technique"
            };

            Assert.IsFalse( filter1.Equals( filter2 ) );

            filter1.ID = filter2.ID;
            Assert.IsFalse( filter1.Equals( filter2 ) );

            filter1.Description = filter2.Description;
            Assert.IsFalse( filter1.Equals( filter2 ) );

            filter1.Operator = filter2.Operator;
            Assert.IsFalse( filter1.Equals( filter2 ) );

            filter1.Subpath = filter2.Subpath;
            Assert.IsFalse( filter1.Equals( filter2 ) );

            filter1.NamePattern = filter2.NamePattern;
            Assert.IsFalse( filter1.Equals( filter2 ) );

            filter1.Language = filter2.Language;
            Assert.IsFalse( filter1.Equals( filter2 ) );

            filter1.ContentPattern = filter2.ContentPattern;
            Assert.IsTrue( filter1.Equals( filter2 ) );

        }

        [Test]
        public void Can_compare_to_null()
        {
            Change change1 = new Change();
            Assert.IsFalse( change1.Equals( null ) );
        }
        #endregion
    }

}
