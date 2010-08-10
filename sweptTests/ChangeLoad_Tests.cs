using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace swept.Tests
{
    [TestFixture]
    public class ChangeLoad_Tests
    {
        ChangeLoad _left;
        ChangeLoad _right;
        Change freshIndents;
        SourceFile fooFile;

        private List<int> _leftLines;
        private List<int> _rightLines;
        private IssueSet _leftIssues;
        private IssueSet _rightIssues;

        [SetUp]
        public void SetUp()
        {
            _left = new ChangeLoad();
            _right = new ChangeLoad();

            freshIndents = new Change { Description = "Refresh indentation" };
            fooFile = new SourceFile( "foo.cs" );
        }

        private void prep_left()
        {
            _leftLines = new List<int> { 1, 2 };
            _leftIssues = new IssueSet( freshIndents, fooFile, _leftLines );
            _left.IssueSets.Add( fooFile, _leftIssues );
        }

        private void prep_right()
        {
            _rightLines = new List<int> { 1, 3 };
            _rightIssues = new IssueSet( freshIndents, fooFile, _rightLines );
            _right.IssueSets.Add( fooFile, _rightIssues );
        }

        #region Union
        [Test]
        public void Union_of_empty_loads_is_empty()
        {
            ChangeLoad outcome = _left.Union( _right );

            Assert.That( outcome, Is.Not.Null );
            Assert.That( outcome.IssueSets, Is.Empty );
        }

        [Test]
        public void Union_with_left_content_has_content()
        {
            prep_left();
            ChangeLoad outcome = _left.Union( _right );

            Assert.That( outcome.IssueSets, Is.Not.Empty );
            Assert.That( outcome.IssueSets, Has.Count.EqualTo( 1 ) );
            Assert.That( outcome.IssueSets[fooFile], Is.SameAs( _leftIssues ) );
        }

        [Test]
        public void Union_with_right_content_has_content()
        {
            prep_right();
            ChangeLoad outcome = _left.Union( _right );

            Assert.That( outcome.IssueSets, Is.Not.Empty );
            Assert.That( outcome.IssueSets, Has.Count.EqualTo( 1 ) );
            Assert.That( outcome.IssueSets[fooFile], Is.SameAs( _rightIssues ) );
        }

        [Test]
        public void Union_unifies_files_and_line_numbers_correctly()
        {
            prep_left();
            prep_right();

            ChangeLoad outcome = _left.Union( _right );

            Assert.That( outcome.IssueSets, Is.Not.Empty );
            Assert.That( outcome.IssueSets, Has.Count.EqualTo( 1 ) );

            IssueSet fooIssues = outcome.IssueSets[fooFile];
            var fooLines = fooIssues.MatchLineNumbers;
            Assert.That( fooLines, Has.Count.EqualTo( 3 ) );
        }
        #endregion

        #region Intersection
        [Test]
        public void Intersection_of_empty_loads_is_empty()
        {
            ChangeLoad outcome = _left.Intersection( _right );

            Assert.That( outcome, Is.Not.Null );
            Assert.That( outcome.IssueSets, Is.Empty );
        }

        [Test]
        public void Intersection_with_left_content_is_empty()
        {
            prep_left();
            ChangeLoad outcome = _left.Intersection( _right );

            Assert.That( outcome.IssueSets, Is.Empty );
        }

        [Test]
        public void Intersection_with_right_content_is_empty()
        {
            prep_right();
            ChangeLoad outcome = _left.Intersection( _right );

            Assert.That( outcome.IssueSets, Is.Empty );
        }

        [Test]
        public void Intersection_unifies_files_and_line_numbers_correctly()
        {
            prep_left();
            prep_right();

            ChangeLoad outcome = _left.Intersection( _right );

            Assert.That( outcome.IssueSets, Is.Not.Empty );
            Assert.That( outcome.IssueSets, Has.Count.EqualTo( 1 ) );

            IssueSet fooIssues = outcome.IssueSets[fooFile];
            var fooLines = fooIssues.MatchLineNumbers;
            Assert.That( fooLines, Has.Count.EqualTo( 1 ) );
            Assert.That( fooLines[0], Is.EqualTo( 1 ) );
        }
        #endregion

        #region Subtraction
        [Test]
        public void Subtraction_of_empty_loads_is_empty()
        {
            ChangeLoad outcome = _left.Subtraction( _right );

            Assert.That( outcome, Is.Not.Null );
            Assert.That( outcome.IssueSets, Is.Empty );
        }

        [Test]
        public void Subtraction_with_left_content_has_content()
        {
            prep_left();
            ChangeLoad outcome = _left.Subtraction( _right );

            Assert.That( outcome.IssueSets, Is.Not.Empty );
            Assert.That( outcome.IssueSets, Has.Count.EqualTo( 1 ) );
            var outcomeIssues = outcome.IssueSets[fooFile];

            Assert_IssueSets_match( outcomeIssues, _leftIssues );
        }

        private void Assert_IssueSets_match( IssueSet resultIssues, IssueSet checkIssues )
        {
            Assert.That( resultIssues, Is.Not.SameAs( checkIssues ) );

            Assert.That( resultIssues.Clause, Is.SameAs( checkIssues.Clause ) );
            Assert.That( resultIssues.SourceFile, Is.SameAs( checkIssues.SourceFile ) );

            foreach (int line in resultIssues.MatchLineNumbers)
            {
                Assert.That( checkIssues.MatchLineNumbers.Contains( line ) );
            }

            foreach (int line in checkIssues.MatchLineNumbers)
            {
                Assert.That( resultIssues.MatchLineNumbers.Contains( line ) );
            }
        }

        [Test]
        public void Subtraction_with_right_content_is_empty()
        {
            prep_right();
            ChangeLoad outcome = _left.Subtraction( _right );

            Assert.That( outcome.IssueSets, Is.Empty );
        }

        [Test]
        public void Subtraction_unifies_files_and_line_numbers_correctly()
        {
            prep_left();
            prep_right();

            ChangeLoad outcome = _left.Subtraction( _right );

            Assert.That( outcome.IssueSets, Is.Not.Empty );
            Assert.That( outcome.IssueSets, Has.Count.EqualTo( 1 ) );

            IssueSet fooIssues = outcome.IssueSets[fooFile];
            var fooLines = fooIssues.MatchLineNumbers;
            Assert.That( fooLines, Has.Count.EqualTo( 1 ) );
            Assert.That( fooLines[0], Is.EqualTo( 2 ) );
        }
        #endregion
    }
}
