//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Linq;
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
            List<int> matches = filter.identifyMatchList( file, "b" );

            Assert.That( matches.Count, Is.EqualTo( number_of_Bs ) );
            Assert.That( matches[0], Is.EqualTo( 3 ) );
            Assert.That( matches[1], Is.EqualTo( 4 ) );
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

            List<int> matches = filter.GetMatches( file );

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
            var file = new SourceFile( "bs.cs" );
            file.Content = _multiLineFile;

            Clause child = new Clause();
            Clause or_sibling = new Clause { Operator = ClauseOperator.Or };
            Clause parent = new Clause { ContentPattern = "xx" };
            parent.Children.Add( child );
            parent.Children.Add( or_sibling );

            child.ContentPattern = "b";
            or_sibling.ContentPattern = "a";
            List<int> matches = parent.GetMatches( file );

            Assert.That( matches.Count, Is.EqualTo( 3 ) );
            Assert.That( matches[0], Is.EqualTo( 2 ) );
            Assert.That( matches[1], Is.EqualTo( 3 ) );
            Assert.That( matches[2], Is.EqualTo( 4 ) );
        }
        
        #endregion



        #region Compound matching
        [Test]
        public void And_children()
        {
            var child1 = new Clause { Language = FileLanguage.CSharp };
            var child2 = new Clause { NamePattern = "blue", Operator = ClauseOperator.And };

            var filter = new Clause { };
            filter.Children.Add( child1 );
            filter.Children.Add( child2 );

            Assert.IsEmpty( filter.GetMatches( new SourceFile( "my.cs" ) ) );
            Assert.IsEmpty( filter.GetMatches( new SourceFile( "blue.html" ) ) );
            Assert.IsNotEmpty( filter.GetMatches( new SourceFile( "blue.cs" ) ) );
        }

        [Test]
        public void Or_children()
        {
            var child1 = new Clause { Language = FileLanguage.CSharp };
            var child2 = new Clause { NamePattern = "blue", Operator = ClauseOperator.Or };

            var filter = new Clause { };
            filter.Children.Add( child1 );
            filter.Children.Add( child2 );

            Assert.IsNotEmpty( filter.GetMatches( new SourceFile( "my.cs" ) ) );
            Assert.IsNotEmpty( filter.GetMatches( new SourceFile( "blue.html" ) ) );
            Assert.IsNotEmpty( filter.GetMatches( new SourceFile( "blue.cs" ) ) );
            Assert.IsEmpty( filter.GetMatches( new SourceFile( "bluuue.css" ) ) );
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

            Assert.IsEmpty( filter.GetMatches( new SourceFile( "my.cs" ) ) );
            Assert.IsNotEmpty( filter.GetMatches( new SourceFile( "my.html" ) ) );
            Assert.IsNotEmpty( filter.GetMatches( new SourceFile( "my.unknownextension" ) ) );
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

            Assert.IsNotEmpty( filter.GetMatches( new SourceFile( @"my_test.cs" ) ) );
            Assert.IsEmpty( filter.GetMatches( new SourceFile( @"Tests.cs" ) ) );
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

            Assert.IsEmpty( filter.GetMatches( new SourceFile( @"my_test.cs" ) ) );
            Assert.IsNotEmpty( filter.GetMatches( new SourceFile( @"Tests.cs" ) ) );
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

            Assert.IsNotEmpty( filter.GetMatches( new SourceFile( @"my_one.cs" ) ) );
            Assert.IsNotEmpty( filter.GetMatches( new SourceFile( @"my_two.cs" ) ) );
            Assert.IsEmpty( filter.GetMatches( new SourceFile( @"my_three.cs" ) ) );
            Assert.IsNotEmpty( filter.GetMatches( new SourceFile( @"my_one_two.cs" ) ) );
        }

        #endregion

        #region Simple filter functionality

        [Test]
        public void empty_filter_matches_any_file()
        {
            var clause = new Clause();
            var file = new SourceFile( @"\path\file.ext" );
            Assert.That( clause.GetMatches( file ), Is.Not.Empty );
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

            Assert.That( filter.GetMatches( new SourceFile( "my.cs" ) ), Is.Not.Empty );
            Assert.That( filter.GetMatches( new SourceFile( "my.html" ) ), Is.Not.Empty );
            Assert.That( filter.GetMatches( new SourceFile( "my.unknownextension" ) ), Is.Not.Empty );
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

            Assert.IsNotEmpty( filter.GetMatches( new SourceFile( "my.cs" ) ) );
            Assert.IsEmpty( filter.GetMatches( new SourceFile( "my.html" ) ) );
            Assert.IsEmpty( filter.GetMatches( new SourceFile( "my.unknownextension" ) ) );
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

            Assert.IsEmpty( filter.GetMatches( new SourceFile( @"my.cs" ) ) );
            Assert.IsNotEmpty( filter.GetMatches( new SourceFile( @"specified\subpath\my.cs" ) ) );
            Assert.IsNotEmpty( filter.GetMatches( new SourceFile( @"specified\subpath\and\deeper\my.cs" ) ) );
            Assert.IsEmpty( filter.GetMatches( new SourceFile( @"another\subpath\my.cs" ) ) );
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

            Assert.IsNotEmpty( filter.GetMatches( new SourceFile( @"myCode.cs" ) ) );
            Assert.IsNotEmpty( filter.GetMatches( new SourceFile( @"Tests.cs" ) ) );
            Assert.IsNotEmpty( filter.GetMatches( new SourceFile( @"myTests.cs" ) ) );
            Assert.IsNotEmpty( filter.GetMatches( new SourceFile( @"my_tests.js" ) ) );
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

            Assert.IsEmpty( filter.GetMatches( new SourceFile( @"my_test.cs" ) ) );
            Assert.IsNotEmpty( filter.GetMatches( new SourceFile( @"Tests.cs" ) ) );
            Assert.IsNotEmpty( filter.GetMatches( new SourceFile( @"myTests.cs" ) ) );
            Assert.IsNotEmpty( filter.GetMatches( new SourceFile( @"my_tests.js" ) ) );
        }

        #endregion

        #region File Content Criteria

        [Test]
        public void content_pattern_misses_file_lacking_regex_match()
        {
            var filter = new Clause { ContentPattern = "(foo|bar)" };
            var file = new SourceFile( "foo.cs" );
            file.Content = "using System;";
            Assert.That( filter.GetMatches( file ), Is.Empty );
        }

        [Test]
        public void content_pattern_matches_file_containing_regex_match()
        {
            var filter = new Clause { ContentPattern = "(foo|bar)" };
            var file = new SourceFile( "foo.cs" );
            file.Content = "using System.foo;";
            Assert.That( filter.GetMatches( file ), Is.Not.Empty );
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