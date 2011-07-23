//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2011 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using NUnit.Framework;

namespace swept.DSL.Tests
{
    [TestFixture]
    public class Answer_tests : DSL_tests_base
    {
        //  These tests don't exhaust all combinations of behavior, they
        //  just show two simple answers, one matching and one not.
        //  ClauseMatch is tested elsewhere more systematically.

        [Test]
        public void Content_Answers()
        {
            var parser = GetChangeRuleParser( "~/foobar/" );
            var query = parser.expression();
            Assert.That( query as QueryContentNode, Is.Not.Null );

            SourceFile file = new SourceFile( "foo.cs" );

            file.Content = "foobie";
            ClauseMatch answer = query.Answer( file );
            Assert.That( answer.DoesMatch, Is.False );

            file.Content = "there is a foobar here";
            answer = query.Answer( file );
            Assert.That( answer.DoesMatch );
        }

        [Test]
        public void Language_Answers()
        {
            var parser = GetChangeRuleParser( "f.l CSharp" );
            var query = parser.expression();
            Assert.That( query as QueryLanguageNode, Is.Not.Null );

            ClauseMatch answer = query.Answer( new SourceFile( "foo.cs" ) );
            Assert.That( answer.DoesMatch );

            answer = query.Answer( new SourceFile( "foo.csproj" ) );
            Assert.That( answer.DoesMatch, Is.False );
        }

        [Test]
        public void Filename_Answers()
        {
            var parser = GetChangeRuleParser( "@/Controller/i" );
            var query = parser.expression();
            Assert.That( query as QueryFileNameNode, Is.Not.Null );

            ClauseMatch answer = query.Answer( new SourceFile( "foo.cs" ) );
            Assert.That( answer.DoesMatch, Is.False );

            answer = query.Answer( new SourceFile( "foo_controller.csproj" ) );
            Assert.That( answer.DoesMatch );
        }

        [Test]
        public void Intersection_Answers()
        {
            var parser = GetChangeRuleParser( "f.l CSharp && @/bar/" );
            var query = parser.expression();
            Assert.That( query as OpIntersectionNode, Is.Not.Null );

            ClauseMatch answer = query.Answer( new SourceFile( "foo.cs" ) );
            Assert.That( answer.DoesMatch, Is.False );

            answer = query.Answer( new SourceFile( "bar.cs" ) );
            Assert.That( answer.DoesMatch );
        }

        [Test]
        public void Union_Answers()
        {
            var parser = GetChangeRuleParser( "^HTML || ^CSS" );
            var query = parser.expression();
            Assert.That( query as OpUnionNode, Is.Not.Null );

            ClauseMatch answer = query.Answer( new SourceFile( "foo.cs" ) );
            Assert.That( answer.DoesMatch, Is.False );

            answer = query.Answer( new SourceFile( "bar.css" ) );
            Assert.That( answer.DoesMatch );
        }

        [Test]
        public void Difference_Answers()
        {
            var parser = GetChangeRuleParser( "f.l CSharp - @/bar/" );
            var query = parser.expression();
            Assert.That( query as OpDifferenceNode, Is.Not.Null );

            ClauseMatch answer = query.Answer( new SourceFile( "bar.cs" ) );
            Assert.That( answer.DoesMatch, Is.False );

            answer = query.Answer( new SourceFile( "foo.cs" ) );
            Assert.That( answer.DoesMatch );

        }
    }
}
