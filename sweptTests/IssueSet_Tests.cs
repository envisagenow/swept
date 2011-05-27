using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace swept.Tests
{
    [CoverageExclude]
    [TestFixture]
    public class IssueSet_Tests
    {
        [Test]
        public void Can_create_line_IssueSet()
        {
            Clause clause = new Clause();
            SourceFile fooFile = new SourceFile( "foo.cs" );
            IssueSet set = new IssueSet( clause, fooFile, MatchScope.Line, new List<int>() { 1 });
            Assert.That( set, Is.Not.Null );
            Assert.That( set.Clause, Is.EqualTo( clause ) );
            Assert.That( set.SourceFile, Is.EqualTo( fooFile ) );
            Assert.That( set.Scope, Is.EqualTo( MatchScope.Line ) );
            Assert.That( set.LinesWhichMatch[0], Is.EqualTo( 1 ) );

            Assert.That( set.DoesMatch );
        }

        [Test]
        public void Can_clone_IssueSet()
        {
            Clause clause = new Clause();
            SourceFile fooFile = new SourceFile( "foo.cs" );
            
            IssueSet set = new IssueSet( clause, fooFile, MatchScope.Line, new List<int>() { 1 });

            IssueSet clone = new IssueSet(set);

            Assert.That( clone, Is.Not.Null );
            Assert.That( clone.Clause, Is.EqualTo( clause ) );
            Assert.That( clone.SourceFile, Is.EqualTo( fooFile ) );
            Assert.That( clone.Scope, Is.EqualTo( MatchScope.Line ) );
            Assert.That( clone.LinesWhichMatch[0], Is.EqualTo( 1 ) );

            Assert.That( set.DoesMatch );
        }

        [Test]
        public void can_store_issues_found_by_filter()
        {
            Change change = new Change { ContentPattern = "b" };
            SourceFile file = new SourceFile( "foo.cs" ) { Content = CompoundFilterTests._multiLineFile };

            IssueSet set = change.GetIssueSet( file );

            IList<int> matchLineNumbers = set.LinesWhichMatch;
            Assert.That( matchLineNumbers.Count, Is.EqualTo( 2 ) );
        }

        [Test]
        public void IssueSets_have_distinct_match_lists()
        {
            Change change = new Change { ContentPattern = "b" };
            SourceFile fooFile = new SourceFile( "foo.cs" ) { Content = CompoundFilterTests._multiLineFile };
            IssueSet fooIssue = change.GetIssueSet( fooFile );

            string barContent = @"bar
bar
bar

barbar
";
            SourceFile barFile = new SourceFile( "bar.cs" ) { Content = barContent };
            IssueSet barIssue = change.GetIssueSet( barFile );

            Assert.That( barIssue.LinesWhichMatch.Count, Is.EqualTo( 5 ) );
            Assert.That( fooIssue.LinesWhichMatch.Count, Is.EqualTo( 2 ) );
        }

        [Test]
        public void IssueSet_remembers_Change_and_SourceFile()
        {
            Change change = new Change { ContentPattern = "b" };
            SourceFile file = new SourceFile( "foo.cs" ) { Content = CompoundFilterTests._multiLineFile };

            IssueSet issue = change.GetIssueSet( file );

            Assert.That( issue.SourceFile, Is.EqualTo( file ) );
            Assert.That( issue.Clause, Is.EqualTo( change ) );
        }

        [Test]
        public void IssueSet_Intersection_creates_new_IssueSet()
        {
            IssueSet left = new IssueSet( null, null, MatchScope.Line, new List<int> { 2, 4, 8 } );
            IssueSet rght = new IssueSet( null, null, MatchScope.Line, new List<int> { 2, 4, 6 } );
            IssueSet result = left.Intersection( rght );
            Assert.That( result, Is.Not.SameAs( left ) );
            Assert.That( left.LinesWhichMatch.Count, Is.EqualTo( 3 ) );
            Assert.That( result.LinesWhichMatch.Count, Is.EqualTo( 2 ) );
        }

        [Test]
        public void IssueSet_Subtraction_creates_new_IssueSet()
        {
            IssueSet left = new IssueSet( null, null, MatchScope.Line, new List<int> { 2, 4, 8 } );
            IssueSet rght = new IssueSet( null, null, MatchScope.Line, new List<int> { 2, 4, 6 } );
            IssueSet result = left.Subtraction( rght );
            Assert.That( result, Is.Not.SameAs( left ) );
            Assert.That( left.LinesWhichMatch.Count, Is.EqualTo( 3 ) );
            Assert.That( result.LinesWhichMatch.Count, Is.EqualTo( 1 ) );
        }

        [Test]
        public void IssueSet_Union_creates_new_IssueSet()
        {
            IssueSet left = new IssueSet( null, null, MatchScope.Line, new List<int> { 2, 4, 8 } );
            IssueSet rght = new IssueSet( null, null, MatchScope.Line, new List<int> { 2, 4, 6 } );
            IssueSet result = left.Union( rght );
            Assert.That( result, Is.Not.SameAs( left ) );
            Assert.That( left.LinesWhichMatch.Count, Is.EqualTo( 3 ) );
            Assert.That( result.LinesWhichMatch.Count, Is.EqualTo( 4 ) );
        }
    }            
}
