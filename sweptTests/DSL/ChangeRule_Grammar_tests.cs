//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2011 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using NUnit.Framework;

namespace swept.DSL.Tests
{
    [TestFixture]
    public class ChangeRule_Grammar_tests : ChangeRule_tests
    {
        [Test]
        public void DirectQuery_with_ContentPattern()
        {
            var parser = GetChangeRuleParser( "lines.match \"foom!\"" );

            var dq = parser.direct_query() as QueryContentNode;

            Assert.That( dq, Is.Not.Null );
            Assert.That( dq.ContentPattern, Is.EqualTo( "foom!" ) );
        }

        [Test]
        public void DirectQuery_with_NamePattern()
        {
            var parser = GetChangeRuleParser( "file.name \"manager\"" );

            var dq = parser.direct_query() as QueryFileNameNode;

            Assert.That( dq, Is.Not.Null );
            Assert.That( dq.NamePattern, Is.EqualTo( "manager" ) );
        }

        [Test]
        public void DirectQuery_with_language()
        {
            var parser = GetChangeRuleParser( "file.language CSS" );

            var dq = parser.direct_query() as QueryLanguageNode;

            Assert.That( dq, Is.Not.Null );
            Assert.That( dq.Language, Is.EqualTo( FileLanguage.CSS ) );
        }

        [Test]
        public void Atom_from_DirectQuery()
        {
            var parser = GetChangeRuleParser( "file.language CSS" );

            ISubquery atom = parser.atom();

            Assert.That( atom, Is.Not.Null );
            var dq = atom as QueryLanguageNode;
            Assert.That( dq, Is.Not.Null );
            Assert.That( dq.Language, Is.EqualTo( FileLanguage.CSS ) );
        }

        [Test]
        public void AndExpression_with_two_DirectQueries()
        {
            var parser = GetChangeRuleParser( "file.language CSS and lines.match \"foom!\"" );

            ISubquery sq = parser.and_exp();
            var andExp = sq as OpIntersectionNode;

            Assert.That( andExp, Is.Not.Null );

            var lhs = andExp.LHS as QueryLanguageNode;
            var rhs = andExp.RHS as QueryContentNode;

            Assert.That( lhs.Language, Is.EqualTo( FileLanguage.CSS ) );
            Assert.That( rhs.ContentPattern, Is.EqualTo( "foom!" ) );
        }

        [Test]
        public void OrExpression_with_two_DirectQueries()
        {
            var parser = GetChangeRuleParser( "file.language CSharp || lines.match 'foom!'" );

            ISubquery sq = parser.expression();
            var orExp = sq as OpUnionNode;

            Assert.That( orExp, Is.Not.Null );

            var lhs = orExp.LHS as QueryLanguageNode;
            var rhs = orExp.RHS as QueryContentNode;

            Assert.That( lhs.Language, Is.EqualTo( FileLanguage.CSharp ) );
            Assert.That( rhs.ContentPattern, Is.EqualTo( "foom!" ) );
        }

        [Test]
        public void Rather_thick_rule_properly_parsed()
        {
            var text = "^CSharp && (~'foo' || ~'bar') || @'.*Controller.cs' ";
            var parser = GetChangeRuleParser( text );

            ISubquery sq = parser.expression();

            Assert.That( sq is OpUnionNode );     //  The 'or' is the top division
            var csfb_or_controller = sq as OpUnionNode;
            Assert.That( csfb_or_controller.LHS is OpIntersectionNode );
            var cs_and_FooOrBar = csfb_or_controller.LHS as OpIntersectionNode;
            Assert.That( (cs_and_FooOrBar.LHS as QueryLanguageNode).Language, Is.EqualTo( FileLanguage.CSharp ) );
            var foo_or_bar = cs_and_FooOrBar.RHS as OpUnionNode;
            Assert.That( (foo_or_bar.LHS as QueryContentNode).ContentPattern, Is.EqualTo( "foo" ) );
        }

    }
}
