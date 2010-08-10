using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace swept.Tests
{
    [TestFixture]
    public class IssueSetTests
    {
        [Test]
        public void can_store_issues_found_by_filter()
        {
            Change change = new Change { ContentPattern = "b" };
            SourceFile file = new SourceFile( "foo.cs" ) { Content = CompoundFilterTests._multiLineFile };

            IssueSet issue = change.GetIssueSet( file );

            IList<int> matchLineNumbers = issue.MatchLineNumbers;
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

            Assert.That( barIssue.MatchLineNumbers.Count, Is.EqualTo( 5 ) );
            Assert.That( fooIssue.MatchLineNumbers.Count, Is.EqualTo( 2 ) );
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
        public void Can_unite_MatchLineNumbers()
        {
            var issueSet = new IssueSet( null, null, ClauseMatchScope.Line, new List<int> { 1, 2 } );
            issueSet.UniteLines( new List<int> { 1, 3 } );

            Assert.That( issueSet.MatchLineNumbers, Has.Count.EqualTo( 3 ) );
        }

    }
}
