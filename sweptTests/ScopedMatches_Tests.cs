﻿using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace swept.Tests
{
    [CoverageExclude]
    [TestFixture]
    /// ScopedMatches - The result of checking a filter against a file
    public class ScopedMatches_Tests
    {
        [Test]
        public void Construction_sets_line_numbers_correctly()
        {
            ClauseMatch matches = new ClauseMatch( new List<int> { 17, 23 } );

            Assert.That( matches.LinesWhichMatch.Count, Is.EqualTo( 2 ) );
            Assert.That( matches.LinesWhichMatch[0], Is.EqualTo( 17 ) );
            Assert.That( matches.LinesWhichMatch[1], Is.EqualTo( 23 ) );
        }

        [Test]
        public void Bool_Constructor_sets_File_Scope()
        {
            var result = new ClauseMatch( true );
            Assert.That( result.Scope, Is.EqualTo( MatchScope.File ) );
        }

        [Test]
        public void List_Constructor_sets_Line_Scope()
        {
            var result = new ClauseMatch( new List<int> { } );
            Assert.That( result.Scope, Is.EqualTo( MatchScope.Line ) );
        }

        [Test]
        public void Bool_Constructor_keeps_match_bool()
        {
            var result = new ClauseMatch( true );
            Assert.That( result.FileDoesMatch );
            result = new ClauseMatch( false );
            Assert.That( result.FileDoesMatch, Is.False );
        }

        [Test]
        public void Line_Constructor_keeps_match_bool()
        {
            List<int> squares = new List<int> { 1, 4 };
            var result = new ClauseMatch( squares );
            Assert.That( result.LinesWhichMatch, Is.EqualTo( squares ) );
            List<int> cubes = new List<int> { 1, 8, 27 };
            result = new ClauseMatch( cubes );
            Assert.That( result.LinesWhichMatch, Is.EqualTo( cubes ) );
        }


#region Equality

        [Test]
        public void test_scopedMatches_equal()
        {
            ClauseMatch ScopedMatches1 = new ClauseMatch( MatchScope.Line, new List<int> { 1, 2 } );
            ClauseMatch ScopedMatches2 = new ClauseMatch( MatchScope.Line, new List<int> { 1, 2 } );

            Assert.True( ScopedMatches1.Equals(ScopedMatches2) );
        }

        [Test]
        public void test_scopedMatches_inequal()
        {
            ClauseMatch ScopedMatches1 = new ClauseMatch( MatchScope.Line, new List<int> { 1, 2 } );
            ClauseMatch ScopedMatches2 = new ClauseMatch( MatchScope.Line, new List<int> { 1, 3 } );

            Assert.False( ScopedMatches1.Equals( ScopedMatches2 ) );
        }

        [Test]
        public void test_scopedMatches_inequal_null()
        {
            Assert.False( new ClauseMatch( MatchScope.Line, new List<int>() ).Equals( null ) );
        }

        [Test]
        public void test_scopedMatches_inequal_length()
        {
            ClauseMatch ScopedMatches1 = new ClauseMatch( MatchScope.Line, new List<int> { 1, 2, 3 } );
            ClauseMatch ScopedMatches2 = new ClauseMatch( MatchScope.Line, new List<int> { 1, 2 } );

            Assert.False( ScopedMatches1.Equals( ScopedMatches2 ) );
        }

#endregion


        #region The new look of Union Testing

        [Test]
        public void union_line_12_to_line_23_makes_line_123()
        {
            var left     = new ClauseMatch( new List<int> { 1, 2 } );
            var right    = new ClauseMatch( new List<int> { 2, 3 } );
            var expected = new ClauseMatch( new List<int> { 1, 2, 3 } );

            ClauseMatch result = left.Union( right );
            Assert.That( result.Equals(expected) );
        }

        [Test]
        public void union_line_12_to_line_empty_makes_line_12()
        {
            var left     = new ClauseMatch( new List<int> { 1, 2 } );
            var right    = new ClauseMatch( new List<int> { } );
            var expected = new ClauseMatch( new List<int> { 1, 2 } );

            ClauseMatch result = left.Union( right );
            Assert.That( result.Equals( expected ) );
        }


        [Test]
        public void union_file_true_to_line_empty_makes_file_true()
        {
            var left = new ClauseMatch( true );
            var right = new ClauseMatch( new List<int> { } );
            var expected = new ClauseMatch( true );

            ClauseMatch result = left.Union( right );
            Assert.That( result.Equals( expected ) );
        }
        #endregion


        #region The scope of matches resulting from a set operation depends on the scopes of the source sets.
        [TestCase( MatchScope.Line, MatchScope.Line, MatchScope.Line )]
        [TestCase( MatchScope.Line, MatchScope.File, MatchScope.Line )]
        [TestCase( MatchScope.File, MatchScope.Line, MatchScope.Line )]
        [TestCase( MatchScope.File, MatchScope.File, MatchScope.File )]
        public void copied( MatchScope leftScope, MatchScope rightScope, MatchScope resultScope )
        {
            var left = new ClauseMatch( leftScope, new List<int>() );
            var right = new ClauseMatch( rightScope, new List<int>() );

            ClauseMatch result = left.Union( right );
            Assert.That( result.Scope == resultScope );
        }
        
        [TestCase( MatchScope.Line, MatchScope.Line, MatchScope.Line )]
        [TestCase( MatchScope.Line, MatchScope.File, MatchScope.Line )]
        [TestCase( MatchScope.File, MatchScope.Line, MatchScope.Line )]
        [TestCase( MatchScope.File, MatchScope.File, MatchScope.File )]
        public void Check_Intersection_Scope( MatchScope leftScope, MatchScope rightScope, MatchScope resultScope )
        {
            var left = new ClauseMatch( leftScope, new List<int>() );
            var right = new ClauseMatch( rightScope, new List<int>() );

            ClauseMatch result = left.Intersection( right );
            Assert.That( result.Scope == resultScope );
        }

        [TestCase( MatchScope.Line, MatchScope.Line, MatchScope.Line )]
        [TestCase( MatchScope.Line, MatchScope.File, MatchScope.Line )]
        [TestCase( MatchScope.File, MatchScope.Line, MatchScope.File )]
        [TestCase( MatchScope.File, MatchScope.File, MatchScope.File )]
        public void Check_Subtraction_Scope( MatchScope leftScope, MatchScope rightScope, MatchScope resultScope )
        {
            var left = new ClauseMatch( leftScope, new List<int>() );
            var right = new ClauseMatch( rightScope, new List<int>() );

            ClauseMatch result = left.Subtraction( right );
            Assert.That( result.Scope == resultScope );
        }

        #endregion

        #region File scoped matches have a (somewhat cheesy) match entry for line 1.  We don't want to mix that entry with line-scoped operations.

        [Test, Ignore("reimplementing...")]
        public void Mixed_Scope_Union_has_correct_line_matches()
        {
            var left = new ClauseMatch( MatchScope.Line, new List<int> { 2, 3 } );
            var right = new ClauseMatch( MatchScope.File, new List<int> { 1 } );

            ClauseMatch result = left.Union( right );

            Assert.That( result.LinesWhichMatch.Count, Is.EqualTo( 2 ) );
            Assert.That( result.LinesWhichMatch[0], Is.EqualTo( 2 ) );
            Assert.That( result.LinesWhichMatch[1], Is.EqualTo( 3 ) );

            result = right.Union( left );

            Assert.That( result.LinesWhichMatch.Count, Is.EqualTo( 2 ) );
            Assert.That( result.LinesWhichMatch[0], Is.EqualTo( 2 ) );
            Assert.That( result.LinesWhichMatch[1], Is.EqualTo( 3 ) );
        }

        [Test]
        public void Mixed_Scope_Intersection_has_correct_line_matches()
        {
            var left = new ClauseMatch( MatchScope.Line, new List<int> { 1, 3 } );
            var right = new ClauseMatch( MatchScope.File, new List<int> { 1 } );

            ClauseMatch result = left.Intersection( right );

            Assert.That( result.LinesWhichMatch.Count, Is.EqualTo( 2 ) );
            Assert.That( result.LinesWhichMatch[0], Is.EqualTo( 1 ) );
            Assert.That( result.LinesWhichMatch[1], Is.EqualTo( 3 ) );

            result = right.Intersection( left );

            Assert.That( result.LinesWhichMatch.Count, Is.EqualTo( 2 ) );
            Assert.That( result.LinesWhichMatch[0], Is.EqualTo( 1 ) );
            Assert.That( result.LinesWhichMatch[1], Is.EqualTo( 3 ) );
        }

        [Test]
        public void Mixed_Scope_Subtraction_has_correct_line_matches()
        {
            var left = new ClauseMatch( MatchScope.Line, new List<int> { 1, 3 } );
            var right = new ClauseMatch( MatchScope.File, new List<int> { 1 } );

            ClauseMatch result = left.Subtraction( right );

            Assert.That( result.LinesWhichMatch.Count, Is.EqualTo( 0 ) );

            result = right.Subtraction( left );

            Assert.That( result.LinesWhichMatch.Count, Is.EqualTo( 0 ) );
        }

        #endregion

        #region Subtraction
        [Test]
        public void Subtraction_of_empty_sets_changes_nothing()
        {
            var lines = new ClauseMatch( MatchScope.Line, new List<int> { 1, 3 } );
            var empty_lines = new ClauseMatch( MatchScope.Line, new List<int>() );
            var file = new ClauseMatch( MatchScope.File, new List<int> { 1 } );
            var empty_file = new ClauseMatch( MatchScope.File, new List<int>() );

            ClauseMatch result = lines.Subtraction( empty_lines );
            Assert.That( result.LinesWhichMatch.Count, Is.EqualTo( 2 ) );

            result = lines.Subtraction( empty_file );
            Assert.That( result.LinesWhichMatch.Count, Is.EqualTo( 2 ) );

            result = file.Subtraction( empty_lines );
            Assert.That( result.LinesWhichMatch.Count, Is.EqualTo( 1 ) );

            result = file.Subtraction( empty_file );
            Assert.That( result.LinesWhichMatch.Count, Is.EqualTo( 1 ) );
        }

        [Test]
        public void Subtraction_from_empty_sets_are_empty()
        {
            var lines = new ClauseMatch( MatchScope.Line, new List<int> { 1, 3 } );
            var empty_lines = new ClauseMatch( MatchScope.Line, new List<int>() );
            var file = new ClauseMatch( MatchScope.File, new List<int> { 1 } );
            var empty_file = new ClauseMatch( MatchScope.File, new List<int>() );

            ClauseMatch result = empty_lines.Subtraction( lines );
            Assert.That( result.LinesWhichMatch.Count, Is.EqualTo( 0 ) );

            result = empty_lines.Subtraction( file );
            Assert.That( result.LinesWhichMatch.Count, Is.EqualTo( 0 ) );

            result = empty_file.Subtraction( lines );
            Assert.That( result.LinesWhichMatch.Count, Is.EqualTo( 0 ) );

            result = empty_file.Subtraction( file );
            Assert.That( result.LinesWhichMatch.Count, Is.EqualTo( 0 ) );
        }

        [Test]
        public void Subtraction_of_file_from_file_is_empty()
        {
            var left = new ClauseMatch( MatchScope.File, new List<int> { 1 } );
            var right = new ClauseMatch( MatchScope.File, new List<int> { 1 } );

            var result = left.Subtraction( right );
            Assert.That( result.LinesWhichMatch.Count, Is.EqualTo( 0 ) );
        }

        [Test]
        public void Subtraction_of_lines_from_lines_removes_intersection()
        {
            var left = new ClauseMatch( MatchScope.Line, new List<int> { 2, 3, 6 } );
            var right = new ClauseMatch( MatchScope.Line, new List<int> { 2, 4, 5, 6, 7 } );

            var result = left.Subtraction( right );
            Assert.That( result.LinesWhichMatch.Count, Is.EqualTo( 1 ) );
            Assert.That( result.LinesWhichMatch[0], Is.EqualTo( 3 ) );
        }

        #endregion
    }
}
