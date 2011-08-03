//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2011 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.using System;
using NUnit.Framework;
using System.Text.RegularExpressions;
using Antlr.Runtime;
using System;

namespace swept.DSL.Tests
{
    [TestFixture]
    public class NodeFactory_tests
    {
        private NodeFactory _factory;

        [SetUp]
        public void SetUp()
        {
            _factory = new NodeFactory();
        }

        [Test]
        public void Get_LanguageQuery_sets_on_match()
        {
            var t = new CommonToken( ChangeRuleLexer.FILE_LANGUAGE );
            var q = _factory.GetQuery( t, "CSharp" ) as QueryLanguageNode;
            Assert.That( q.Language, Is.EqualTo( FileLanguage.CSharp ) );
        }

        [Test]
        public void Get_LanguageQuery_errs_descriptively_on_mismatch()
        {
            var t = new CommonToken( ChangeRuleLexer.FILE_LANGUAGE );
            var ex = Assert.Throws<ArgumentException>( () => _factory.GetQuery( t, "CShoop" ) );
            Assert.That( ex.Message, Is.EqualTo( "Swept does not know how to tell if a file is language [CShoop] at this time." ) );
        }

        [Test]
        public void GetRegex_default_is_multiline()
        {
            // TODO: Push this test up so we're testing the behavior, and name the test as
            // per the comment below:
            //  So the begin and end anchors match for each line in the source file.
            var rex = _factory.GetRegex( "foo", null );
            Assert.That( rex.Options & RegexOptions.Multiline, Is.EqualTo( RegexOptions.Multiline ) );
        }

        [Test]
        public void GetRegex_option_i_sets_case_insensitive()
        {
            var rex = _factory.GetRegex( "foo", null );

            Assert.That( rex.IsMatch( "foo" ) );
            Assert.That( rex.IsMatch( "Foo" ), Is.False );

            rex = _factory.GetRegex( "foo", "i" );

            Assert.That( rex.IsMatch( "foo" ) );
            Assert.That( rex.IsMatch( "Foo" ) );
        }

        [Test]
        public void GetRegex_option_s_turns_singleline_on()
        {
            var rex = _factory.GetRegex( "foo.bar", "" );

            Assert.That( rex.IsMatch( "foo bar" ) );
            Assert.That( rex.IsMatch( "foo\nbar" ), Is.False );

            rex = _factory.GetRegex( "foo.bar", "s" );
            
            Assert.That( rex.IsMatch( "foo bar" ) );
            Assert.That( rex.IsMatch( "foo\nbar" ) );
        }
    }
}
