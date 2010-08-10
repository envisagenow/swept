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
        public void IssueSet_for_file_scoped_match_has_no_match_lines()
        {
            Clause isCSharp = new Clause { Language = FileLanguage.CSharp };
            SourceFile file = new SourceFile( "foo.cs" );

            IssueSet set = isCSharp.GetFileIssueSet( file );

            Assert.That( set, Is.Not.Null );
            Assert.That( set.MatchScope, Is.EqualTo( ClauseMatchScope.File ) );
            Assert.That( set.DoesMatch );

            Assert.That( set.MatchLineNumbers, Has.Count.EqualTo( 0 ) );

            //// alt impl
            //Assert.That( issues.MatchLineNumbers, Has.Count.EqualTo( 1 ) );
            //Assert.That( issues.MatchLineNumbers[0], Is.EqualTo( 1 ) );
        }

        [Test]
        public void Non_matching_IssueSet_exists_with_DoesMatch_false()
        {
            IssueSet set = null;

            Clause isCSharp = new Clause { Language = FileLanguage.CSharp };

            SourceFile file = new SourceFile( "foo.html" );
            set = isCSharp.GetFileIssueSet( file );

            Assert.That( set.DoesMatch, Is.False );
            Assert.That( set.MatchLineNumbers, Has.Count.EqualTo( 0 ) );
        }
    }
}
