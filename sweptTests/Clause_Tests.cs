//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace swept.Tests
{
    [TestFixture]
    public class Clause_Tests
    {
        [Test]
        public void Clause_MatchScope_for_each_criterion()
        {
            Clause clause = new Clause { Language = FileLanguage.CSharp };
            Assert.That( clause.MatchScope, Is.EqualTo( ClauseMatchScope.File ) );

            clause = new Clause { NamePattern = @".*\.as(p|c)x(\.cs)?" };
            Assert.That( clause.MatchScope, Is.EqualTo( ClauseMatchScope.File ) );

            clause = new Clause { ContentPattern = @"\.DeprecatedMethod\(" };
            Assert.That( clause.MatchScope, Is.EqualTo( ClauseMatchScope.Line ) );

            // TODO: XML syntax for promoting line scope to file scope
            clause = new Clause { ContentPattern = "using Legacy.Technology.library;", ForceFileScope = true };
            Assert.That( clause.MatchScope, Is.EqualTo( ClauseMatchScope.File ) );
        }

        [Test]
        public void IssueSet_for_file_scoped_match_has_one_match_line()
        {
            Clause isCSharp = new Clause { Language = FileLanguage.CSharp };
            SourceFile file = new SourceFile( "foo.cs" );

            IssueSet set = isCSharp.GetIssueSet( file );

            Assert.That( set, Is.Not.Null );
            Assert.That( set.MatchScope, Is.EqualTo( ClauseMatchScope.File ) );
            Assert.That( set.DoesMatch );

            Assert.That( set.MatchLineNumbers, Has.Count.EqualTo( 1 ) );
            Assert.That( set.MatchLineNumbers[0], Is.EqualTo( 1 ) );
        }

        [Test]
        public void Non_matching_IssueSet_exists_with_DoesMatch_false()
        {
            IssueSet set = null;

            Clause isCSharp = new Clause { Language = FileLanguage.CSharp };

            SourceFile file = new SourceFile( "foo.html" );
            set = isCSharp.GetIssueSet( file );

            Assert.That( set.DoesMatch, Is.False );
            Assert.That( set.MatchLineNumbers, Has.Count.EqualTo( 0 ) );
        }


        public static string _multiLineFile =
@"
axxxxxx
abxx
bcxxxx
cxxxxxxxxx
";


        [Test]
        public void sibling_with_And_operator_will_intersect_matches()
        {
            SourceFile file = new SourceFile( "bs.cs" );
            file.Content = _multiLineFile;

            Clause child = new Clause();
            Clause and_sibling = new Clause { Operator = ClauseOperator.And };
            Clause parent = new Clause { ContentPattern = "xx" };
            parent.Children.Add( child );
            parent.Children.Add( and_sibling );

            child.ContentPattern = "b";
            and_sibling.ContentPattern = "a";
            parent.DoesMatch( file );

            Assert.That( parent._matchList.Count, Is.EqualTo( 1 ) );
            Assert.That( parent._matchList[0], Is.EqualTo( 3 ) );
        }

        [Test]
        public void LineScoped_child_will_create_line_scoped_result()
        {
            SourceFile file = new SourceFile( "bs_foo.cs" ) { Content = _multiLineFile };

            Clause child = new Clause { ContentPattern = "b" };
            Clause parent = new Clause();
            parent.Children.Add( child );

            Assert.That( parent.MatchesChildren( file ) );

            Assert.That( child._matchList.Count, Is.EqualTo( 2 ) );
            Assert.That( parent._matchList.Count, Is.EqualTo( 2 ) );
            Assert.That( parent._matchList[0], Is.EqualTo( 3 ) );
        }

        [Test]
        public void FileScoped_child_will_create_file_scoped_result_in_parent()
        {
            SourceFile file = new SourceFile( "bs_foo.cs" );
            file.Content = _multiLineFile;

            Clause child = new Clause { ContentPattern = "b", ForceFileScope=true };
            Clause parent = new Clause();
            parent.Children.Add( child );
    
            Assert.That( parent.MatchesChildren( file ) );

            Assert.That( parent._matchList.Count, Is.EqualTo( 1 ) );
            Assert.That( parent._matchList[0], Is.EqualTo( 1 ) );
            Assert.That( parent.MatchScope, Is.EqualTo( ClauseMatchScope.File ) );
        }

        [Test]
        public void FileScoped_sibling_will_create_file_scoped_result_in_parent()
        {
            SourceFile file = new SourceFile( "bs_foo.cs" );
            file.Content = _multiLineFile;

            Clause child = new Clause { ContentPattern = "b", };
            Clause sibling = new Clause { NamePattern = ".*foo.*", Operator = ClauseOperator.And };
            Clause parent = new Clause( );
            parent.Children.Add( child );
            parent.Children.Add( sibling );

            Assert.That( parent.MatchesChildren( file ) );

            Assert.That( sibling._matchList.Count, Is.EqualTo( 1 ) );
            Assert.That( parent._matchList.Count, Is.EqualTo( 1 ) );
            Assert.That( parent._matchList[0], Is.EqualTo( 1 ) );
            Assert.That( parent.MatchScope, Is.EqualTo( ClauseMatchScope.File ) );
        }

        private class ClauseHarness : Clause
        {
            private ClauseMatchScope _matchScope = ClauseMatchScope.File;

            public override ClauseMatchScope MatchScope { get { return _matchScope; } }

            public ClauseHarness( ClauseMatchScope matchScope )
            {
                _matchScope = matchScope;
            }
        }

        [Test]
        public void FileScoped_match_returned_when_intersected_with_nonmatching_LineScope_matches()
        {
            ClauseHarness fileScopedClause = new ClauseHarness(ClauseMatchScope.File){_matchList = new List<int>() { 1 }};
            ClauseHarness lineScopedClause = new ClauseHarness(ClauseMatchScope.Line){_matchList = new List<int>() { 3,4 }};

//            fileScopedClause.Intersect( lineScopedClause );

//            Assert.That( fileScopedClause.MatchScope, Is.EqualTo() );
//            Assert.That( fileScopedClause._matchList, Is.EqualTo() );

        }
    }


}
