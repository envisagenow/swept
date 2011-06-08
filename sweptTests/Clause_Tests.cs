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
    [CoverageExclude]
    [TestFixture]
    public class Clause_Tests
    {
        [Test]
        public void Clause_MatchScope_for_each_criterion()
        {
            Clause clause = new Clause { Language = FileLanguage.CSharp };
            Assert.That( clause.Scope, Is.EqualTo( MatchScope.File ) );

            clause = new Clause { NamePattern = @".*\.as(p|c)x(\.cs)?" };
            Assert.That( clause.Scope, Is.EqualTo( MatchScope.File ) );

            clause = new Clause { ContentPattern = @"\.DeprecatedMethod\(" };
            Assert.That( clause.Scope, Is.EqualTo( MatchScope.Line ) );

            // TODO: XML syntax for promoting line scope to file scope
            clause = new Clause { ContentPattern = "using Legacy.Technology.library;", ForceFileScope = true };
            Assert.That( clause.Scope, Is.EqualTo( MatchScope.File ) );
        }

        [Test]
        public void Non_matching_IssueSet_exists_with_DoesMatch_false()
        {
            IssueSet set = null;

            Clause isCSharp = new Clause { Language = FileLanguage.CSharp };

            SourceFile file = new SourceFile( "foo.html" );
            set = isCSharp.GetIssueSet( file );

            Assert.That( set.DoesMatch, Is.False );
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
            ClauseMatch matches = parent.GetMatches( file );

            var lines = ((LineMatch)matches).Lines;
            Assert.That( lines.Count, Is.EqualTo( 1 ) );
            Assert.That( lines[0], Is.EqualTo( 3 ) );
        }

        //[Test]
        //public void LineScoped_child_will_create_line_scoped_result()
        //{
        //    SourceFile file = new SourceFile( "bs_foo.cs" ) { Content = _multiLineFile };

        //    Clause child = new Clause { ContentPattern = "b" };
        //    Clause parent = new Clause();
        //    parent.Children.Add( child );

        //    // TODO: upgrade to ScopedMatches
        //    List<int> childMatches = parent.GetChildMatches( file ).LinesWhichMatch;
        //    Assert.That( childMatches.Any() );
        //    Assert.That( childMatches.Count, Is.EqualTo( 2 ) );

        //    var parentMatches = parent.GetMatches( file );
        //    Assert.That( parentMatches.LinesWhichMatch.Count, Is.EqualTo( 2 ) );
        //    Assert.That( parentMatches.LinesWhichMatch[0], Is.EqualTo( 3 ) );
        //}

        //[Test]
        //public void FileScoped_child_will_create_file_scoped_result_in_parent()
        //{
        //    SourceFile file = new SourceFile( "bs_foo.cs" );

        //    Clause child = new Clause { NamePattern = "foo" };
        //    Clause parent = new Clause();
        //    parent.Children.Add( child );

        //    var childMatches = parent.GetChildMatches( file );
        //    Assert.That( childMatches.DoesMatch );

        //    var parentMatches = parent.GetMatches( file );
        //    Assert.That( parentMatches.LinesWhichMatch.Count, Is.EqualTo( 1 ) );
        //    Assert.That( parentMatches.LinesWhichMatch[0], Is.EqualTo( 1 ) );
        //    Assert.That( parent.Scope, Is.EqualTo( MatchScope.File ) );
        //}

        //[Test]
        //public void Forcing_File_Scope_sets_match_list_to_1()
        //{
        //    SourceFile file = new SourceFile( "bs_foo.cs" );
        //    file.Content = _multiLineFile;

        //    Clause child = new Clause { ContentPattern = "b", ForceFileScope=true };

        //    var matches = child.GetMatches( file );
        //    Assert.That( matches.LinesWhichMatch.Count, Is.EqualTo( 1 ) );
        //    Assert.That( matches.LinesWhichMatch[0], Is.EqualTo( 1 ) );
        //}

        //[Test, Ignore("Another portion of the evaluation that needs rethinking")]
        //public void LineScoped_childMatches_communicate_line_scope_to_parent()
        //{
        //    SourceFile file = new SourceFile( "bs_foo.cs" );
        //    file.Content = _multiLineFile;

        //    Clause child = new Clause { ContentPattern = "b", };
        //    Clause sibling = new Clause { NamePattern = ".*foo.*", Operator = ClauseOperator.And };
        //    Clause parent = new Clause( );
        //    parent.Children.Add( child );
        //    parent.Children.Add( sibling );

        //    ClauseMatch childMatches = parent.GetChildMatches( file );
        //    Assert.That( childMatches.DoesMatch );

        //    ClauseMatch parentMatches = parent.GetMatches( file );
        //    Assert.That( parentMatches.LinesWhichMatch.Count, Is.EqualTo( 1 ) );
        //    Assert.That( parentMatches.LinesWhichMatch[0], Is.EqualTo( 1 ) );
        //    Assert.That( parentMatches.Scope, Is.EqualTo( MatchScope.Line ) );
        //}

    }

}
