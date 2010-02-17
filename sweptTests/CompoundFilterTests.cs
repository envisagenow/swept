//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using NUnit.Framework;
using swept;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace swept.Tests
{

    [TestFixture]
    public class CompoundFilterTests
    {
        private string _multiLineFile =
@"
axxxxxx
abxx
bcxxxx
cxxxxxxxxx
";
        private CompoundFilter filter;
        [SetUp]
        public void SetUp()
        {
            filter = new CompoundFilter();
        }


        #region Compound line matching
        [Test]
        public void when_parent_filter_is_not_content_filter_child_matches_replace_parent_matches()
        {
            SourceFile file = new SourceFile( "bs.cs" );
            file.Content = _multiLineFile;

            CompoundFilter child = new CompoundFilter();
            CompoundFilter parent = new CompoundFilter();
            parent.Children.Add( child );

            child.ContentPattern = "b";

            parent.DoesMatch( file );

            Assert.That( parent._matchList.Count, Is.EqualTo( 2 ) );
            Assert.That( parent._matchList[0], Is.EqualTo( 3 ) );
            Assert.That( parent._matchList[1], Is.EqualTo( 4 ) );
        }

        [Test]
        public void sibling_with_And_operator_will_intersect_matches()
        {
            SourceFile file = new SourceFile( "bs.cs" );
            file.Content = _multiLineFile;

            CompoundFilter child = new CompoundFilter();
            CompoundFilter and_sibling = new CompoundFilter { Operator = FilterOperator.And };
            CompoundFilter parent = new CompoundFilter();
            parent.Children.Add( child );
            parent.Children.Add( and_sibling );

            child.ContentPattern = "b";
            and_sibling.ContentPattern = "a";
            parent.DoesMatch( file );

            Assert.That( parent._matchList.Count, Is.EqualTo( 1 ) );
            Assert.That( parent._matchList[0], Is.EqualTo( 3 ) );
        }

        [Test]
        public void sibling_with_Or_operator_will_intersect_matches()
        {
            SourceFile file = new SourceFile( "bs.cs" );
            file.Content = _multiLineFile;

            CompoundFilter child = new CompoundFilter();
            CompoundFilter or_sibling = new CompoundFilter { Operator = FilterOperator.Or };
            CompoundFilter parent = new CompoundFilter();
            parent.Children.Add( child );
            parent.Children.Add( or_sibling );

            child.ContentPattern = "b";
            or_sibling.ContentPattern = "a";
            parent.DoesMatch( file );

            Assert.That( parent._matchList.Count, Is.EqualTo( 3 ) );
            Assert.That( parent._matchList[0], Is.EqualTo( 2 ) );
            Assert.That( parent._matchList[1], Is.EqualTo( 3 ) );
            Assert.That( parent._matchList[2], Is.EqualTo( 4 ) );
        }

        [Test]
        public void when_parent_filter_is_content_filter_child_matches_combine_with_parent_matches()
        {
            SourceFile file = new SourceFile( "bs.cs" );
            file.Content = _multiLineFile;

            CompoundFilter parent = new CompoundFilter();
            CompoundFilter child = new CompoundFilter();

            parent.Children.Add( child );
            child.ContentPattern = "b";
            parent.ContentPattern = "c";

            parent.DoesMatch( file );

            Assert.That( parent._matchList.Count, Is.EqualTo( 1 ) );
            Assert.That( parent._matchList[0], Is.EqualTo( 4 ) );
        }

        [Test]
        public void line_start_positions_come_up_from_child_filters_with_nested_child_filter()
        {
            SourceFile file = new SourceFile( "bs.cs" );
            file.Content = _multiLineFile;

            CompoundFilter parent = new CompoundFilter();
            CompoundFilter child = new CompoundFilter();
            CompoundFilter grandchild = new CompoundFilter();

            parent.Children.Add( child );
            child.Children.Add( grandchild );
            grandchild.ContentPattern = "b";
            
            parent.DoesMatch( file );

            Assert.That( parent._matchList.Count, Is.EqualTo( 2 ) );
            Assert.That( parent._matchList[0], Is.EqualTo( 3 ) );
            Assert.That( parent._matchList[1], Is.EqualTo( 4 ) );
        }
        
        #endregion



        #region Line matching
        [Test]
        public void can_return_list_of_line_start_positions()
        {
            filter.generateLineIndices( _multiLineFile );

            Assert.That( filter._lineIndices.Count, Is.EqualTo( 5 ) );
            Assert.That( filter._lineIndices[0], Is.EqualTo( 1 ) );
            Assert.That( filter._lineIndices[1], Is.EqualTo( 10 ) );
        }


        [Test]
        public void can_return_list_of_matched_line_numbers()
        {
            const int number_of_Bs = 2;

            filter.generateLineIndices( _multiLineFile );
            filter.identifyMatchLineNumbers( _multiLineFile, "b" );

            Assert.That( filter._matchList.Count, Is.EqualTo( number_of_Bs ) );
            Assert.That( filter._matchList[0], Is.EqualTo( 3 ) );
            Assert.That( filter._matchList[1], Is.EqualTo( 4 ) );
        }

        [Test]
        public void matchlist_is_populated_by_Matches_call()
        {
            SourceFile file = new SourceFile( "bs.cs" );
            file.Content = _multiLineFile;

            filter.ContentPattern = "b";

            filter.DoesMatch( file );

            Assert.That( filter._matchList.Count, Is.EqualTo( 2 ) );
            Assert.That( filter._matchList[0], Is.EqualTo( 3 ) );
            Assert.That( filter._matchList[1], Is.EqualTo( 4 ) );
        }


        [Test]
        public void MatchesContent_returns_bool_of_match()
        {
            filter.ContentPattern = "Foo";
            Assert.That( filter.MatchesContent( "using Foo;" ) );
            filter.ContentPattern = "Bar";
            Assert.That( filter.MatchesContent( "using Foo;" ), Is.False );
        }

        [Test]
        public void MatchesContent_sets_list_of_match_line_indexes()
        {
            filter.ContentPattern = "Foo";
            filter.MatchesContent( "using Foo;" );
            Assert.That( filter._matchList.Count, Is.EqualTo( 1 ) );
            Assert.That( filter._matchList[0], Is.EqualTo( 1 ) );
        }

        [Test]
        public void MatchesContent_sets_list_of_match_line_indexes_empty_when_unmatched()
        {
            filter.ContentPattern = "Bar";
            filter.MatchesContent( "using Foo;" );
            Assert.That( filter._matchList.Count, Is.EqualTo( 0 ) );
        }


        [Test]
        public void match_first_character_is_line_1()
        {
            Assert.That( filter.lineNumberOfMatch( 1, new List<int> { 2, 17 } ), Is.EqualTo( 1 ) );
        }

        [Test]
        public void match_after_index_of_first_newline_is_line_2()
        {
            Assert.That( filter.lineNumberOfMatch( 4, new List<int> { 2, 5 } ), Is.EqualTo( 2 ) );
        }

        [Test]
        public void match_first_newline_is_line_1()
        {
            Assert.That( filter.lineNumberOfMatch( 2, new List<int> { 2, 5 } ), Is.EqualTo( 1 ) );
        }

        [Test]
        public void match_after_last_newline_is_last_line()
        {
            Assert.That( filter.lineNumberOfMatch( 40, new List<int> { 2, 5 } ), Is.EqualTo( 3 ) );
        }

        #endregion

        #region Compound matching
        [Test]
        public void Children_get_matched()
        {
            var child1 = new CompoundFilter { Language = FileLanguage.CSharp };
            var child2 = new CompoundFilter { NamePattern = "blue" };

            var filter = new CompoundFilter { };
            filter.Children.Add( child1 );
            filter.Children.Add( child2 );

            Assert.IsFalse( filter.DoesMatch( new SourceFile( "my.cs" ) ) );
            Assert.IsFalse( filter.DoesMatch( new SourceFile( "blue.html" ) ) );
            Assert.IsTrue( filter.DoesMatch( new SourceFile( "blue.cs" ) ) );
        }

        [Test]
        public void Or_filter()
        {
            var child1 = new CompoundFilter { Language = FileLanguage.CSharp };
            var child2 = new CompoundFilter { NamePattern = "blue", Operator = FilterOperator.Or };

            var filter = new CompoundFilter { };
            filter.Children.Add( child1 );
            filter.Children.Add( child2 );

            Assert.IsTrue( filter.DoesMatch( new SourceFile( "my.cs" ) ) );
            Assert.IsTrue( filter.DoesMatch( new SourceFile( "blue.html" ) ) );
            Assert.IsTrue( filter.DoesMatch( new SourceFile( "blue.cs" ) ) );
            Assert.IsFalse( filter.DoesMatch( new SourceFile( "bluuue.css" ) ) );
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

            Assert.IsFalse( filter.DoesMatch( new SourceFile( "my.cs" ) ) );
            Assert.IsTrue( filter.DoesMatch( new SourceFile( "my.html" ) ) );
            Assert.IsTrue( filter.DoesMatch( new SourceFile( "my.unknownextension" ) ) );
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

            Assert.IsTrue( filter.DoesMatch( new SourceFile( @"my_test.cs" ) ) );
            Assert.IsFalse( filter.DoesMatch( new SourceFile( @"Tests.cs" ) ) );
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

            Assert.IsFalse( filter.DoesMatch( new SourceFile( @"my_test.cs" ) ) );
            Assert.IsTrue( filter.DoesMatch( new SourceFile( @"Tests.cs" ) ) );
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

            Assert.IsTrue( filter.DoesMatch( new SourceFile( @"my_one.cs" ) ) );
            Assert.IsTrue( filter.DoesMatch( new SourceFile( @"my_two.cs" ) ) );
            Assert.IsFalse( filter.DoesMatch( new SourceFile( @"my_three.cs" ) ) );
            Assert.IsTrue( filter.DoesMatch( new SourceFile( @"my_one_two.cs" ) ) );
        }

        #endregion

        #region Simple filter functionality

        [Test]
        public void empty_filter_matches_any_file()
        {
            var filter = new CompoundFilter();
            var file = new SourceFile( @"\path\file.ext" );
            Assert.That( filter.DoesMatch( file ) );
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

            Assert.IsTrue( filter.DoesMatch( new SourceFile( "my.cs" ) ) );
            Assert.IsTrue( filter.DoesMatch( new SourceFile( "my.html" ) ) );
            Assert.IsTrue( filter.DoesMatch( new SourceFile( "my.unknownextension" ) ) );
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

            Assert.IsTrue( filter.DoesMatch( new SourceFile( "my.cs" ) ) );
            Assert.IsFalse( filter.DoesMatch( new SourceFile( "my.html" ) ) );
            Assert.IsFalse( filter.DoesMatch( new SourceFile( "my.unknownextension" ) ) );
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

            Assert.IsTrue( filter.DoesMatch( new SourceFile( @"my.cs" ) ) );
            Assert.IsTrue( filter.DoesMatch( new SourceFile( @"specified\subpath\my.cs" ) ) );
            Assert.IsTrue( filter.DoesMatch( new SourceFile( @"specified\subpath\and\deeper\my.cs" ) ) );
            Assert.IsTrue( filter.DoesMatch( new SourceFile( @"another\subpath\my.cs" ) ) );
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

            Assert.IsFalse( filter.DoesMatch( new SourceFile( @"my.cs" ) ) );
            Assert.IsTrue( filter.DoesMatch( new SourceFile( @"specified\subpath\my.cs" ) ) );
            Assert.IsTrue( filter.DoesMatch( new SourceFile( @"specified\subpath\and\deeper\my.cs" ) ) );
            Assert.IsFalse( filter.DoesMatch( new SourceFile( @"another\subpath\my.cs" ) ) );
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

            Assert.IsTrue( filter.DoesMatch( new SourceFile( @"myCode.cs" ) ) );
            Assert.IsTrue( filter.DoesMatch( new SourceFile( @"Tests.cs" ) ) );
            Assert.IsTrue( filter.DoesMatch( new SourceFile( @"myTests.cs" ) ) );
            Assert.IsTrue( filter.DoesMatch( new SourceFile( @"my_tests.js" ) ) );
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

            Assert.IsFalse( filter.DoesMatch( new SourceFile( @"my_test.cs" ) ) );
            Assert.IsTrue( filter.DoesMatch( new SourceFile( @"Tests.cs" ) ) );
            Assert.IsTrue( filter.DoesMatch( new SourceFile( @"myTests.cs" ) ) );
            Assert.IsTrue( filter.DoesMatch( new SourceFile( @"my_tests.js" ) ) );
        }

        #endregion

        #region File Content Criteria

        [Test]
        public void content_pattern_misses_file_lacking_regex_match()
        {
            var filter = new CompoundFilter { ContentPattern = "(foo|bar)" };
            var file = new SourceFile( "foo.cs" );
            file.Content = "using System;";
            Assert.That( filter.DoesMatch( file ), Is.False );
        }

        [Test]
        public void content_pattern_matches_file_containing_regex_match()
        {
            var filter = new CompoundFilter { ContentPattern = "(foo|bar)" };
            var file = new SourceFile( "foo.cs" );
            file.Content = "using System.foo;";
            Assert.That( filter.DoesMatch( file ), Is.True );
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
        public void CompoundFilter_Equals_considers_child_filters()
        {

            CompoundFilter filter1 = new CompoundFilter
            {
                ID = "near duplicate",
                Children = new List<CompoundFilter> { new CompoundFilter { ID = "Thing 1" }, new CompoundFilter { ID = "Thing 2" }, }
            };

            CompoundFilter filter2 = new CompoundFilter
            {
                ID = "near duplicate",
                Children = new List<CompoundFilter> { new CompoundFilter { ID = "Thing 1" }, new CompoundFilter { ID = "Thing 3" }, }
            };

            Assert.That( filter1.Equals( filter2 ), Is.False );
        }

        [Test]
        public void CompoundFilter_Equals_considers_child_filters_count()
        {

            CompoundFilter filter1 = new CompoundFilter
            {
                ID = "near duplicate",
                Children = new List<CompoundFilter> { new CompoundFilter { ID = "Thing 1" } }
            };

            CompoundFilter filter2 = new CompoundFilter
            {
                ID = "near duplicate",
                Children = new List<CompoundFilter> { new CompoundFilter { ID = "Thing 1" }, new CompoundFilter { ID = "Thing 2" }, new CompoundFilter { ID = "Thing 3" } }
            };

            Assert.That( filter1.Equals( filter2 ), Is.False );
        }
        [Test]
        public void Can_compare_to_null()
        {
            Change change1 = new Change();
            Assert.IsFalse( change1.Equals( null ) );
        }
        #endregion

        #region Cloning
        [Test]
        public void Clone_copies_core_attributes()
        {
            CompoundFilter filter = new CompoundFilter {
                ID = "A",
                Description = "B",
                Operator = FilterOperator.Not,

                Subpath = @"D:\",
                NamePattern = "E",
                Language = FileLanguage.CSharp,

                ContentPattern = "F"
            };

            CompoundFilter clone = filter.Clone();
            
            Assert.That( clone.ID, Is.EqualTo( "A" ) );
            Assert.That( clone.Description, Is.EqualTo( "B" ) );
            Assert.That( clone.Operator, Is.EqualTo( FilterOperator.Not ) );
            
            Assert.That( clone.Subpath, Is.EqualTo( @"D:\" ) );
            Assert.That( clone.NamePattern, Is.EqualTo( "E" ) );
            Assert.That( clone.Language, Is.EqualTo( FileLanguage.CSharp ) );

            Assert.That( clone.ID, Is.EqualTo( "A" ) );
        }
        #endregion

    }

}