using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace swept.Tests
{
    [CoverageExclude]
    [TestFixture]
    public class ScopedMatches_Tests
    {
        [Test]
        public void Construction_sets_line_numbers_correctly()
        {
            ScopedMatches matches = new ScopedMatches( MatchScope.Line, new List<int> { 17, 23 } );

            Assert.That( matches.Count, Is.EqualTo( 2 ) );
            Assert.That( matches[0], Is.EqualTo( 17 ) );
            Assert.That( matches[1], Is.EqualTo( 23 ) );
        }



        #region The scope of matches resulting from a set operation depends on the scopes of the source sets.

        [TestCase( MatchScope.Line, MatchScope.Line, MatchScope.Line )]
        [TestCase( MatchScope.Line, MatchScope.File, MatchScope.Line )]
        [TestCase( MatchScope.File, MatchScope.Line, MatchScope.Line )]
        [TestCase( MatchScope.File, MatchScope.File, MatchScope.File )]
        public void Test_scope_produced_by_scope_union( MatchScope leftScope, MatchScope rightScope, MatchScope resultScope )
        {
            var left = new ScopedMatches( leftScope, new List<int>() );
            var right = new ScopedMatches( rightScope, new List<int>() );

            ScopedMatches result = left.Union( right );
            Assert.That( result.Scope == resultScope );
        }
        
        [TestCase( MatchScope.Line, MatchScope.Line, MatchScope.Line )]
        [TestCase( MatchScope.Line, MatchScope.File, MatchScope.Line )]
        [TestCase( MatchScope.File, MatchScope.Line, MatchScope.Line )]
        [TestCase( MatchScope.File, MatchScope.File, MatchScope.File )]
        public void Check_Intersection_Scope( MatchScope leftScope, MatchScope rightScope, MatchScope resultScope )
        {
            var left = new ScopedMatches( leftScope, new List<int>() );
            var right = new ScopedMatches( rightScope, new List<int>() );

            ScopedMatches result = left.Intersection( right );
            Assert.That( result.Scope == resultScope );
        }

        [TestCase( MatchScope.Line, MatchScope.Line, MatchScope.Line )]
        [TestCase( MatchScope.Line, MatchScope.File, MatchScope.Line )]
        [TestCase( MatchScope.File, MatchScope.Line, MatchScope.File )]
        [TestCase( MatchScope.File, MatchScope.File, MatchScope.File )]
        public void Check_Subtraction_Scope( MatchScope leftScope, MatchScope rightScope, MatchScope resultScope )
        {
            var left = new ScopedMatches( leftScope, new List<int>() );
            var right = new ScopedMatches( rightScope, new List<int>() );

            ScopedMatches result = left.Subtraction( right );
            Assert.That( result.Scope == resultScope );
        }

        #endregion

        #region File scoped matches have a (somewhat cheesy) match entry for line 1.  We don't want to mix that entry with line-scoped operations.

        [Test]
        public void Mixed_Scope_Union_has_correct_line_matches()
        {
            var left = new ScopedMatches( MatchScope.Line, new List<int> { 2, 3 } );
            var right = new ScopedMatches( MatchScope.File, new List<int> { 1 } );

            ScopedMatches result = left.Union( right );

            Assert.That( result.Count, Is.EqualTo( 2 ) );
            Assert.That( result[0], Is.EqualTo( 2 ) );
            Assert.That( result[1], Is.EqualTo( 3 ) );

            result = right.Union( left );

            Assert.That( result.Count, Is.EqualTo( 2 ) );
            Assert.That( result[0], Is.EqualTo( 2 ) );
            Assert.That( result[1], Is.EqualTo( 3 ) );
        }

        [Test]
        public void Mixed_Scope_Intersection_has_correct_line_matches()
        {
            var left = new ScopedMatches( MatchScope.Line, new List<int> { 1, 3 } );
            var right = new ScopedMatches( MatchScope.File, new List<int> { 1 } );

            ScopedMatches result = left.Intersection( right );

            Assert.That( result.Count, Is.EqualTo( 2 ) );
            Assert.That( result[0], Is.EqualTo( 1 ) );
            Assert.That( result[1], Is.EqualTo( 3 ) );

            result = right.Intersection( left );

            Assert.That( result.Count, Is.EqualTo( 2 ) );
            Assert.That( result[0], Is.EqualTo( 1 ) );
            Assert.That( result[1], Is.EqualTo( 3 ) );
        }

        [Test]
        public void Mixed_Scope_Subtraction_has_correct_line_matches()
        {
            var left = new ScopedMatches( MatchScope.Line, new List<int> { 1, 3 } );
            var right = new ScopedMatches( MatchScope.File, new List<int> { 1 } );

            ScopedMatches result = left.Subtraction( right );

            Assert.That( result.Count, Is.EqualTo( 0 ) );

            result = right.Subtraction( left );

            Assert.That( result.Count, Is.EqualTo( 0 ) );
        }

        #endregion

        #region Subtraction
        [Test]
        public void Subtraction_of_empty_sets_changes_nothing()
        {
            var lines = new ScopedMatches( MatchScope.Line, new List<int> { 1, 3 } );
            var empty_lines = new ScopedMatches( MatchScope.Line, new List<int>() );
            var file = new ScopedMatches( MatchScope.File, new List<int> { 1 } );
            var empty_file = new ScopedMatches( MatchScope.File, new List<int>() );

            ScopedMatches result = lines.Subtraction( empty_lines );
            Assert.That( result.Count, Is.EqualTo( 2 ) );

            result = lines.Subtraction( empty_file );
            Assert.That( result.Count, Is.EqualTo( 2 ) );

            result = file.Subtraction( empty_lines );
            Assert.That( result.Count, Is.EqualTo( 1 ) );

            result = file.Subtraction( empty_file );
            Assert.That( result.Count, Is.EqualTo( 1 ) );
        }

        [Test]
        public void Subtraction_from_empty_sets_are_empty()
        {
            var lines = new ScopedMatches( MatchScope.Line, new List<int> { 1, 3 } );
            var empty_lines = new ScopedMatches( MatchScope.Line, new List<int>() );
            var file = new ScopedMatches( MatchScope.File, new List<int> { 1 } );
            var empty_file = new ScopedMatches( MatchScope.File, new List<int>() );

            ScopedMatches result = empty_lines.Subtraction( lines );
            Assert.That( result.Count, Is.EqualTo( 0 ) );

            result = empty_lines.Subtraction( file );
            Assert.That( result.Count, Is.EqualTo( 0 ) );

            result = empty_file.Subtraction( lines );
            Assert.That( result.Count, Is.EqualTo( 0 ) );

            result = empty_file.Subtraction( file );
            Assert.That( result.Count, Is.EqualTo( 0 ) );
        }

        [Test]
        public void Subtraction_of_file_from_file_is_empty()
        {
            var left = new ScopedMatches( MatchScope.File, new List<int> { 1 } );
            var right = new ScopedMatches( MatchScope.File, new List<int> { 1 } );

            var result = left.Subtraction( right );
            Assert.That( result.Count, Is.EqualTo( 0 ) );
        }

        [Test]
        public void Subtraction_of_lines_from_lines_removes_intersection()
        {
            var left = new ScopedMatches( MatchScope.Line, new List<int> { 2, 3, 6 } );
            var right = new ScopedMatches( MatchScope.Line, new List<int> { 2, 4, 5, 6, 7 } );

            var result = left.Subtraction( right );
            Assert.That( result.Count, Is.EqualTo( 1 ) );
            Assert.That( result[0], Is.EqualTo( 3 ) );
        }

        #endregion
    }
}
