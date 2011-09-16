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
        //  These tests don't exhaust all combinations of behavior, they just show 
        //  two simple answers per distinct behavior, one matching and one not.
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

        [Test]
        public void Negation_Answers()
        {
            var parser = GetChangeRuleParser( "!f.l CSharp" );
            var query = parser.expression();
            Assert.That( query as OpNegationNode, Is.Not.Null );

            ClauseMatch answer = query.Answer( new SourceFile( "foo.cs" ) );
            Assert.That( answer.DoesMatch, Is.False );

            answer = query.Answer( new SourceFile( "foo.html" ) );
            Assert.That( answer.DoesMatch );
        }

        [Test]
        public void Chain_Unary_Answers()
        {
            var parser = GetChangeRuleParser( "!!^CSharp" );
            var query = parser.expression();
            Assert.That( query, Is.InstanceOf<OpNegationNode>() );
            Assert.That( (query as OpNegationNode).RHS, Is.InstanceOf<OpNegationNode>() );

            ClauseMatch answer = query.Answer( new SourceFile( "foo.cs" ) );
            Assert.That( answer.DoesMatch );

            answer = query.Answer( new SourceFile( "foo.html" ) );
            Assert.That( answer.DoesMatch, Is.False );
        }

        [Test]
        public void FileScope_Answers()
        {
            var parser = GetChangeRuleParser( "* ~/things/" );
            var query = parser.expression();
            Assert.That( query, Is.InstanceOf<OpFileScopeNode>() );

            SourceFile fooFile = new SourceFile( "foo.cs" );
            fooFile.Content = "// stuff\n\n";
            ClauseMatch answer = query.Answer( fooFile );
            Assert.That( answer.DoesMatch, Is.False );

            fooFile.Content = "// things\n\n";
            answer = query.Answer( fooFile );
            Assert.That( answer.DoesMatch );
            Assert.That( answer, Is.InstanceOf<FileMatch>() );
        }

        [Test]
        public void Compound_Answers()
        {
            string rule = "^CSharp and (~\"using Acadis.IRepoFramework;\" or ~\"XadrAction\")";

            var parser = GetChangeRuleParser(rule);
            var query = parser.expression();
            Assert.That(query as OpIntersectionNode, Is.Not.Null);

            var bar = new SourceFile("bar.cs");
            bar.Content = "using example;\n//hello, world!\n";
            ClauseMatch answer = query.Answer(bar);

            Assert.That(answer.DoesMatch, Is.False);
        }
    }
}
