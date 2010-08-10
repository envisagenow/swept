//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
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
        public static string _multiLineFile =
@"
axxxxxx
abxx
bcxxxx
cxxxxxxxxx
";
        private Clause filter;
        [SetUp]
        public void SetUp()
        {
            filter = new Clause();
        }

        #region Match lines
        [Test]
        public void can_return_list_of_matched_line_numbers()
        {
            const int number_of_Bs = 2;

            SourceFile file = new SourceFile( "foo.cs" );

            file.Content = _multiLineFile;
            filter.identifyMatchLineNumbers( file, "b" );
            List<int> matches = filter.GetMatchList();

            Assert.That( matches.Count, Is.EqualTo( number_of_Bs ) );
            Assert.That( matches[0], Is.EqualTo( 3 ) );
            Assert.That( matches[1], Is.EqualTo( 4 ) );
        }

        [Test]
        public void MatchesContent_returns_bool_of_match()
        {
            SourceFile file = new SourceFile( "foo.cs" ) { Content = "using Foo;" };
            filter.ContentPattern = "Foo";
            Assert.That( filter.MatchesContent( file ) );
            filter.ContentPattern = "Bar";
            Assert.That( filter.MatchesContent( file ), Is.False );
        }

        [Test]
        public void MatchesContent_sets_list_of_match_line_indexes()
        {
            SourceFile file = new SourceFile( "foo.cs" ) { Content = "using Foo;" };
            filter.ContentPattern = "Foo";
            Assert.That( filter.MatchesContent( file ) );
            Assert.That( filter._matchList.Count, Is.EqualTo( 1 ) );
            Assert.That( filter._matchList[0], Is.EqualTo( 1 ) );
        }

        [Test]
        public void MatchesContent_sets_list_of_match_line_indexes_empty_when_unmatched()
        {
            SourceFile file = new SourceFile( "foo.cs" ) { Content = "using Foo;" };
            filter.ContentPattern = "Bar";
            Assert.That( filter.MatchesContent( file ), Is.False );
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

        [Test]
        public void matchlist_is_populated_by_Matches_call()
        {
            SourceFile file = new SourceFile( "bs.cs" );
            file.Content = _multiLineFile;

            filter.ContentPattern = "b";

            filter.DoesMatch( file );
            List<int> matches = filter.GetMatchList();

            Assert.That( matches.Count, Is.EqualTo( 2 ) );
            Assert.That( matches[0], Is.EqualTo( 3 ) );
            Assert.That( matches[1], Is.EqualTo( 4 ) );
        }

        #region Compound line matching
        //[Test]
        //public void when_parent_filter_is_not_content_filter_child_matches_replace_parent_matches()
        //{
        //    SourceFile file = new SourceFile( "bs.cs" );
        //    file.Content = _multiLineFile;

        //    CompoundFilter child = new CompoundFilter();
        //    CompoundFilter parent = new CompoundFilter();
        //    parent.Children.Add( child );

        //    child.ContentPattern = "b";

        //    parent.DoesMatch( file );

        //    Assert.That( parent._matchList.Count, Is.EqualTo( 2 ) );
        //    Assert.That( parent._matchList[0], Is.EqualTo( 3 ) );
        //    Assert.That( parent._matchList[1], Is.EqualTo( 4 ) );
        //}

        //[Test]
        //public void when_parent_filter_is_content_filter_child_matches_intersect_with_parent_matches()
        //{
        //    SourceFile file = new SourceFile( "bs.cs" );
        //    file.Content = _multiLineFile;

        //    CompoundFilter parent = new CompoundFilter();
        //    CompoundFilter child = new CompoundFilter();

        //    parent.Children.Add( child );
        //    child.ContentPattern = "b";
        //    parent.ContentPattern = "c";

        //    parent.DoesMatch( file );

        //    Assert.That( parent._matchList.Count, Is.EqualTo( 1 ) );
        //    Assert.That( parent._matchList[0], Is.EqualTo( 4 ) );
        //}

        //[Test]
        //public void line_start_positions_come_up_from_child_filters_with_nested_child_filter()
        //{
        //    SourceFile file = new SourceFile( "bs.cs" );
        //    file.Content = _multiLineFile;

        //    CompoundFilter parent = new CompoundFilter();
        //    CompoundFilter child = new CompoundFilter();
        //    CompoundFilter grandchild = new CompoundFilter();

        //    parent.Children.Add( child );
        //    child.Children.Add( grandchild );
        //    grandchild.ContentPattern = "b";
        //    
        //    parent.DoesMatch( file );

        //    Assert.That( parent._matchList.Count, Is.EqualTo( 2 ) );
        //    Assert.That( parent._matchList[0], Is.EqualTo( 3 ) );
        //    Assert.That( parent._matchList[1], Is.EqualTo( 4 ) );
        //}

        [Test]
        public void sibling_with_And_operator_will_intersect_matches()
        {
            SourceFile file = new SourceFile( "bs.cs" );
            file.Content = _multiLineFile;

            Clause child = new Clause();
            Clause and_sibling = new Clause { Operator = ClauseOperator.And };
            Clause parent = new Clause();
            parent.Children.Add( child );
            parent.Children.Add( and_sibling );

            child.ContentPattern = "b";
            and_sibling.ContentPattern = "a";
            parent.DoesMatch( file );

            Assert.That( parent._matchList.Count, Is.EqualTo( 1 ) );
            Assert.That( parent._matchList[0], Is.EqualTo( 3 ) );
        }

        // TODO: goal
        //[Test]
        //public void Clause_sibling_with_And_operator_will_intersect_matches()
        //{
        //    SourceFile file = new SourceFile( "bs.cs" );
        //    file.Content = _multiLineFile;

        //    Clause child = new Clause();
        //    Clause and_sibling = new Clause { Operator = FilterOperator.And };
        //    Clause parent = new Clause();
        //    parent.Children.Add( child );
        //    parent.Children.Add( and_sibling );

        //    child.ContentPattern = "b";
        //    and_sibling.ContentPattern = "a";
        //    var issues = parent.GetIssueSet( file );

        //    Assert.That( parent._matchList.Count, Is.EqualTo( 1 ) );
        //    Assert.That( parent._matchList[0], Is.EqualTo( 3 ) );
        //}

        [Test]
        public void sibling_with_Or_operator_will_union_matches()
        {
            SourceFile file = new SourceFile( "bs.cs" );
            file.Content = _multiLineFile;

            Clause child = new Clause();
            Clause or_sibling = new Clause { Operator = ClauseOperator.Or };
            Clause parent = new Clause();
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
        
        #endregion



        #region Compound matching
        [Test]
        public void Children_get_matched()
        {
            var child1 = new Clause { Language = FileLanguage.CSharp };
            var child2 = new Clause { NamePattern = "blue" };

            var filter = new Clause { };
            filter.Children.Add( child1 );
            filter.Children.Add( child2 );

            Assert.IsFalse( filter.DoesMatch( new SourceFile( "my.cs" ) ) );
            Assert.IsFalse( filter.DoesMatch( new SourceFile( "blue.html" ) ) );
            Assert.IsTrue( filter.DoesMatch( new SourceFile( "blue.cs" ) ) );
        }

        [Test]
        public void Or_filter()
        {
            var child1 = new Clause { Language = FileLanguage.CSharp };
            var child2 = new Clause { NamePattern = "blue", Operator = ClauseOperator.Or };

            var filter = new Clause { };
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
            var filter = new Clause
            {
                ID = "Not language",
                Description = "Relevant to everything except C# files.",
                Language = FileLanguage.CSharp,
                Operator = ClauseOperator.Not,
            };

            Assert.IsFalse( filter.DoesMatch( new SourceFile( "my.cs" ) ) );
            Assert.IsTrue( filter.DoesMatch( new SourceFile( "my.html" ) ) );
            Assert.IsTrue( filter.DoesMatch( new SourceFile( "my.unknownextension" ) ) );
        }

        [Test]
        public void NotFilter_inverts_decision()
        {
            Clause filter = new Clause
            {
                ID = "no_tests",
                Description = "Skip the test files",
                NamePattern = "tests",
                Operator = ClauseOperator.Not
            };

            Assert.IsTrue( filter.DoesMatch( new SourceFile( @"my_test.cs" ) ) );
            Assert.IsFalse( filter.DoesMatch( new SourceFile( @"Tests.cs" ) ) );
        }

        [Test]
        public void Filter_passes_depending_on_internal_filters()
        {
            Clause child = new Clause
            {
                ID = "Tests",
                Description = "Give me just the unit tests.",
                NamePattern = "tests"
            };

            Clause filter = new Clause
            {
                Children = new List<Clause> { child }
            };

            Assert.IsFalse( filter.DoesMatch( new SourceFile( @"my_test.cs" ) ) );
            Assert.IsTrue( filter.DoesMatch( new SourceFile( @"Tests.cs" ) ) );
        }

        [Test]
        public void either_of_or_filter_passing_passes()
        {
            Clause one = new Clause
            {
                ID = "one",
                Description = "files named one",
                NamePattern = "one"
            };

            Clause two = new Clause
            {
                ID = "two",
                Description = "files named two",
                NamePattern = "two",
                Operator = ClauseOperator.Or,
            };

            Clause filter = new Clause
            {
                Children = new List<Clause> { one, two }
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
            var filter = new Clause();
            var file = new SourceFile( @"\path\file.ext" );
            Assert.That( filter.DoesMatch( file ) );
        }

        [Test]
        public void language_filter_passes_all_when_set_to_None()
        {
            Clause filter = new Clause
            {
                ID = "no language",
                Description = "Relevant to files of all languages.",
                Language = FileLanguage.None
            };

            Assert.That( filter.DoesMatch( new SourceFile( "my.cs" ) ) );
            Assert.That( filter.DoesMatch( new SourceFile( "my.html" ) ) );
            Assert.That( filter.DoesMatch( new SourceFile( "my.unknownextension" ) ) );
        }

        [Test]
        public void language_filter_passes_matches_only()
        {
            Clause filter = new Clause
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
        public void subpath_filter_passes_matches_only()
        {
            Clause filter = new Clause
            {
                ID = "specified subpath",
                Description = "Relevant to files in one subtree.",
                NamePattern = @"^specified\\subpath\\.*"
            };

            Assert.IsFalse( filter.DoesMatch( new SourceFile( @"my.cs" ) ) );
            Assert.IsTrue( filter.DoesMatch( new SourceFile( @"specified\subpath\my.cs" ) ) );
            Assert.IsTrue( filter.DoesMatch( new SourceFile( @"specified\subpath\and\deeper\my.cs" ) ) );
            Assert.IsFalse( filter.DoesMatch( new SourceFile( @"another\subpath\my.cs" ) ) );
        }

        [Test]
        public void name_pattern_filter_passes_all_when_empty()
        {
            Clause filter = new Clause
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
            Clause filter = new Clause
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
            var filter = new Clause { ContentPattern = "(foo|bar)" };
            var file = new SourceFile( "foo.cs" );
            file.Content = "using System;";
            Assert.That( filter.DoesMatch( file ), Is.False );
        }

        [Test]
        public void content_pattern_matches_file_containing_regex_match()
        {
            var filter = new Clause { ContentPattern = "(foo|bar)" };
            var file = new SourceFile( "foo.cs" );
            file.Content = "using System.foo;";
            Assert.That( filter.DoesMatch( file ), Is.True );
        }

        #endregion

        #region Name reporting
        [Test]
        public void can_mark_all_first_children()
        {
            Clause filter = new Clause();
            filter.Children.Add( new Clause() );
            filter.Children.Add( new Clause() );
            filter.Children.Add( new Clause() );
            filter.Children[1].Children.Add( new Clause() );
            filter.Children[1].Children.Add( new Clause() );
            filter.markFirstChildren();

            Assert_first_children_correct( filter );

            interregnum_pretendership( filter );
            filter.markFirstChildren();

            Assert_first_children_correct( filter );
        }

        private static void Assert_first_children_correct( Clause filter )
        {
            Assert.True( filter.FirstChild );

            Assert.True( filter.Children[0].FirstChild );
            Assert.False( filter.Children[1].FirstChild );
            Assert.False( filter.Children[2].FirstChild );

            Assert.True( filter.Children[1].Children[0].FirstChild );
            Assert.False( filter.Children[1].Children[1].FirstChild );
        }

        private void interregnum_pretendership( Clause filter )
        {
            filter.FirstChild = true;
            filter.Children.ForEach( child => interregnum_pretendership( child ) );
        }
        #endregion

        #region Equality
        [Test]
        public void Can_compare_equality()
        {
            Clause filter1 = new Change();
            Clause filter2 = new Change();

            Assert.IsTrue( filter1.Equals( filter2 ) );
        }

        [Test]
        public void Can_compare_inequal_objects()
        {
            Clause filter1 = new Clause { 
                ID = "101-443", Description = "Remove all dinguses", Operator = ClauseOperator.And,
                NamePattern = "this", Language = FileLanguage.CSharp,
                ContentPattern = "old_technology"
            };
            Clause filter2 = new Clause { 
                ID = "5987515", Description = "Frob all wobbishes", Operator = ClauseOperator.Not,
                NamePattern = "that", Language = FileLanguage.CSS,
                ContentPattern = "old_technique"
            };

            Assert.IsFalse( filter1.Equals( filter2 ) );

            filter1.ID = filter2.ID;
            Assert.IsFalse( filter1.Equals( filter2 ) );

            filter1.Description = filter2.Description;
            Assert.IsFalse( filter1.Equals( filter2 ) );

            filter1.Operator = filter2.Operator;
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

            Clause filter1 = new Clause
            {
                ID = "near duplicate",
                Children = new List<Clause> { new Clause { ID = "Thing 1" }, new Clause { ID = "Thing 2" }, }
            };

            Clause filter2 = new Clause
            {
                ID = "near duplicate",
                Children = new List<Clause> { new Clause { ID = "Thing 1" }, new Clause { ID = "Thing 3" }, }
            };

            Assert.That( filter1.Equals( filter2 ), Is.False );
        }

        [Test]
        public void CompoundFilter_Equals_considers_child_filters_count()
        {

            Clause filter1 = new Clause
            {
                ID = "near duplicate",
                Children = new List<Clause> { new Clause { ID = "Thing 1" } }
            };

            Clause filter2 = new Clause
            {
                ID = "near duplicate",
                Children = new List<Clause> { new Clause { ID = "Thing 1" }, new Clause { ID = "Thing 2" }, new Clause { ID = "Thing 3" } }
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
            Clause filter = new Clause {
                ID = "A",
                Description = "B",
                Operator = ClauseOperator.Not,

                NamePattern = "E",
                Language = FileLanguage.CSharp,

                ContentPattern = "F"
            };

            Clause clone = filter.Clone();
            
            Assert.That( clone.ID, Is.EqualTo( "A" ) );
            Assert.That( clone.Description, Is.EqualTo( "B" ) );
            Assert.That( clone.Operator, Is.EqualTo( ClauseOperator.Not ) );
            
            Assert.That( clone.NamePattern, Is.EqualTo( "E" ) );
            Assert.That( clone.Language, Is.EqualTo( FileLanguage.CSharp ) );

            Assert.That( clone.ID, Is.EqualTo( "A" ) );
        }
        #endregion

    }

}