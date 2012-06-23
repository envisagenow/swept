//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2011 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace swept.DSL.Tests
{
    [TestFixture]
    public class Grammar_tests : DSL_tests_base
    {
        //  To test that query objects created by the parser have the right values.
        //  I mix up the query pronunciation and the regex delimiters to imply that
        //  the two can be mix-matched.  How can I make that point more clearly?

        [TestCase( "l.m \"foom!\"" )]
        [TestCase( "lines.match 'foom!'" )]
        [TestCase( "~/foom!/" )]
        public void Query_of_ContentPattern( string query )
        {
            var parser = GetChangeRuleParser( query );
            var dq = parser.expression() as QueryContentNode;

            Assert.That( dq, Is.Not.Null );
            Assert.That( dq.Pattern.ToString(), Is.EqualTo( "foom!" ) );
        }

        [TestCase( "file.name \"manager\"" )]
        [TestCase( "@'manager'" )]
        [TestCase( "f.n /manager/" )]
        public void Query_of_NamePattern( string query )
        {
            var parser = GetChangeRuleParser( query );
            var dq = parser.expression() as QueryFileNameNode;

            Assert.That( dq, Is.Not.Null );
            Assert.That( dq.Pattern.ToString(), Is.EqualTo( "manager" ) );
        }

        [TestCase( "file.language CSS" )]
        [TestCase( "f.l CSS" )]
        [TestCase( "^CSS" )]
        public void Query_of_language( string query )
        {
            var parser = GetChangeRuleParser( query );
            ISubquery expr = parser.expression();
            var dq = expr as QueryLanguageNode;

            Assert.That( dq, Is.Not.Null );
            Assert.That( dq.Language, Is.EqualTo( FileLanguage.CSS ) );
        }

        [Test]
        public void Modifiers_post_regex_set_options()
        {
            var parser = GetChangeRuleParser( "~/foom!/i" );

            ISubquery sq = parser.expression();
            var qc = sq as QueryContentNode;

            Assert.That( qc.Pattern.ToString(), Is.EqualTo( "foom!" ) );
            Assert.That( qc.Pattern.Options & RegexOptions.IgnoreCase, Is.EqualTo( RegexOptions.IgnoreCase ) );
            Assert.That( qc.Pattern.IsMatch( "FoOM!" ) );
        }

        [TestCase( "and" )]
        [TestCase( "&&" )]
        public void Simple_intersection( string conjunction )
        {
            string query = string.Format("file.language CSS {0} lines.match \"foom!\"", conjunction);
            var parser = GetChangeRuleParser( query );

            ISubquery sq = parser.expression();
            var andExp = sq as OpIntersectionNode;

            Assert.That( andExp, Is.Not.Null );

            var lhs = andExp.LHS as QueryLanguageNode;
            var rhs = andExp.RHS as QueryContentNode;

            Assert.That( lhs.Language, Is.EqualTo( FileLanguage.CSS ) );
            Assert.That( rhs.Pattern.ToString(), Is.EqualTo( "foom!" ) );
        }

        [TestCase( "or" )]
        [TestCase( "||" )]
        public void Simple_union( string conjunction )
        {
            string query = string.Format( "file.language CSharp {0} lines.match 'foom!'", conjunction );
            var parser = GetChangeRuleParser( query );

            ISubquery sq = parser.expression();
            var orExp = sq as OpUnionNode;

            Assert.That( orExp, Is.Not.Null );

            var lhs = orExp.LHS as QueryLanguageNode;
            var rhs = orExp.RHS as QueryContentNode;

            Assert.That( lhs.Language, Is.EqualTo( FileLanguage.CSharp ) );
            Assert.That( rhs.Pattern.ToString(), Is.EqualTo( "foom!" ) );
        }

        [TestCase( "not" )]
        [TestCase( "!" )]
        public void Simple_negation( string negation )
        {
            string query = string.Format( "{0} file.language CSharp", negation );
            var parser = GetChangeRuleParser( query );

            ISubquery sq = parser.expression();
            var notExp = sq as OpNegationNode;

            Assert.That( notExp, Is.Not.Null );

            var rhs = notExp.RHS as QueryLanguageNode;

            Assert.That( rhs.Language, Is.EqualTo( FileLanguage.CSharp ) );
        }

        [Test]
        public void Chained_intersection()
        {
            var text = "^CSharp and @'.as[cp]x.cs' and ~'\\.OpenSession' ";
            var parser = GetChangeRuleParser( text );

            ISubquery sq = parser.expression();

            Assert.That( sq is OpIntersectionNode );
            var left_and_OpenSession = sq as OpIntersectionNode;

            Assert.That( left_and_OpenSession.RHS is QueryContentNode );
            var openSession = left_and_OpenSession.RHS as QueryContentNode;
            Assert.That( openSession.Pattern.ToString(), Is.EqualTo( "\\.OpenSession" ) );

            Assert.That( left_and_OpenSession.LHS is OpIntersectionNode );
            var cs_and_aspx = left_and_OpenSession.LHS as OpIntersectionNode;
            Assert.That( cs_and_aspx.LHS is QueryLanguageNode );
            Assert.That( cs_and_aspx.RHS is QueryFileNameNode );
            var cs = cs_and_aspx.LHS as QueryLanguageNode;
            var aspx = cs_and_aspx.RHS as QueryFileNameNode;

            Assert.That( cs.Language, Is.EqualTo( FileLanguage.CSharp ) );
            Assert.That( aspx.Pattern.ToString(), Is.EqualTo( ".as[cp]x.cs" ) );
        }

        [Test]
        public void Chained_union()
        {
            var text = "^CSharp or ^HTML or ^JavaScript";
            var parser = GetChangeRuleParser( text );

            ISubquery sq = parser.expression();

            var things_or_js = sq as OpUnionNode;
            var cs_or_html = things_or_js.LHS as OpUnionNode;
            var cs = cs_or_html.LHS as QueryLanguageNode;
            var html = cs_or_html.RHS as QueryLanguageNode;
            var js = things_or_js.RHS as QueryLanguageNode;

            Assert.That( cs.Language, Is.EqualTo( FileLanguage.CSharp ) );
            Assert.That( html.Language, Is.EqualTo( FileLanguage.HTML ) );
            Assert.That( js.Language, Is.EqualTo( FileLanguage.JavaScript ) );
        }

        [Test]
        public void Rather_thick_rule_properly_parsed()
        {
            var text = "^CSharp && (~/foo/ || ~'bar') || @'.*Controller.cs' ";
            var parser = GetChangeRuleParser( text );

            ISubquery sq = parser.expression();

            Assert.That( sq is OpUnionNode );     //  The 'or' is the top division
            var csfb_or_controller = sq as OpUnionNode;
            Assert.That( csfb_or_controller.LHS is OpIntersectionNode );
            var cs_and_FooOrBar = csfb_or_controller.LHS as OpIntersectionNode;
            Assert.That( (cs_and_FooOrBar.LHS as QueryLanguageNode).Language, Is.EqualTo( FileLanguage.CSharp ) );
            var foo_or_bar = cs_and_FooOrBar.RHS as OpUnionNode;
            Assert.That( (foo_or_bar.LHS as QueryContentNode).Pattern.ToString(), Is.EqualTo( "foo" ) );
        }
    }
}
