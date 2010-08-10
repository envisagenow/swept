//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using NUnit.Framework;
using System.Xml;
using System.Collections.Generic;

namespace swept.Tests
{
    [TestFixture]
    public class XmlPort_CompoundFilter_Tests 
    {
        private XmlPort_CompoundFilter port;

        [SetUp]
        public void given_an_XmlPort()
        {
            port = new XmlPort_CompoundFilter();
        }

        #region From Node
        [Test, ExpectedException( ExpectedMessage = "Changes must have IDs at their top level.")]
        public void FromNode_requires_ID_at_top_level()
        {
            var node = Node_FromText( @"<Change />" );
            port.CompoundFilter_FromNode( node );
        }

        [Test]
        public void FromNode_works_when_empty()
        {
            string filter_text = @"<Change ID='Empty'/>";
            var node = Node_FromText( filter_text );

            Clause filter = port.CompoundFilter_FromNode( node );
            Assert.That( filter.Children.Count, Is.EqualTo( 0 ) );
        }

        [Test, ExpectedException( ExpectedMessage = "Filters do not have the following attributes: 'Author'." )]
        public void FromNode_warns_about_unknown_attribute_author()
        {
            string filter_text = @"<Change ID='misattributed' Author='Shakespeare'/>";
            var node = Node_FromText( filter_text );

            port.CompoundFilter_FromNode( node );
        }

        [Test, ExpectedException( ExpectedMessage = "Filters do not have the following attributes: 'Author', 'Version'." )]
        public void FromNode_warns_about_multiple_unknown_attributes()
        {
            string filter_text = @"<Change ID='misattributed' Author='Bob' Version='0.1'/>";
            var node = Node_FromText( filter_text );

            port.CompoundFilter_FromNode( node );
        }

        [Test]
        public void FromNode_sets_Operator()
        {
            // Puzzle:  How to keep this current as operators are _added_.
            var namesToOps = new Dictionary<string, ClauseOperator>() {
                { "Either", ClauseOperator.Or },
                { "Or", ClauseOperator.Or },
                { "When", ClauseOperator.And },
                { "And", ClauseOperator.And },
                { "Change", ClauseOperator.And },
                { "AndNot", ClauseOperator.Not },
                { "Not", ClauseOperator.Not },
            };

            foreach (string filterName in namesToOps.Keys)
            {
                string nodeText = string.Format( "<{0} ID='another operator'/>", filterName );
                var node = Node_FromText( nodeText );
                var filter = port.CompoundFilter_FromNode( node );
                Assert.That( filter.Operator, Is.EqualTo( namesToOps[filterName] ) );
            }
        }

        [Test, ExpectedException( ExpectedMessage = "Swept does not know how to create a 'What' filter." )]
        public void FromNode_throws_on_unknown_operator()
        {
            var whatNode = Node_FromText( @"<What Language='CSharp' />" );
            port.CompoundFilter_FromNode( whatNode );
        }

        [Test]
        public void FromNode_builds_deeply_nested_filters()
        {
            string filter_text =
@"<Change ID='parent'>
    <When ID='child'>
        <When ID='grandchild' />
        <And ID='second grandchild'>
            <When ID='first great-grandchild' />
        </And>
    </When>
    <Or ID='second child' />
</Change>
";
            var node = Node_FromText( filter_text );

            Clause filter = port.CompoundFilter_FromNode( node );
            Assert.That( filter.ID, Is.EqualTo( "parent" ) );
            Assert.That( filter.Children.Count, Is.EqualTo( 2 ) );
            Assert.That( filter.Children[0].Children[1].Children[0].ID, Is.EqualTo( "first great-grandchild" ) );
        }

        [Test]
        public void FromNode_subfilter_IDs_are_optional()
        {
            string filter_text =
@"<Change ID='single'>
    <When ContentPattern='bad code to find' />
</Change>
";
            var node = Node_FromText( filter_text );

            Clause filter = port.CompoundFilter_FromNode( node );
            Assert.That( filter.ID, Is.EqualTo( "single" ) );
            Assert.That( filter.Children.Count, Is.EqualTo( 1 ) );
        }

        [Test]
        public void FromNode_retrieves_attributes()
        {
            string filter_text =
@"<Change ID='1212' Description='Through and through'>
    <When NamePattern='vorpal.cs' Language='CSharp' ContentPattern='using System.Snicker.Snack;' />
</Change>
";
            var node = Node_FromText( filter_text );

            Clause filter = port.CompoundFilter_FromNode( node );
            Assert.That( filter.Children.Count, Is.EqualTo( 1 ) );
            var child = filter.Children[0];

            Assert.That( filter.ID, Is.EqualTo( "1212" ) );
            Assert.That( filter.Description, Is.EqualTo( "Through and through" ) );

            Assert.That( child.Operator, Is.EqualTo( ClauseOperator.And ) );
            Assert.That( child.NamePattern, Is.EqualTo( "vorpal.cs" ) );
            Assert.That( child.Language, Is.EqualTo( FileLanguage.CSharp ) );

            Assert.That( child.ContentPattern, Is.EqualTo( "using System.Snicker.Snack;" ) );
        }

        [Test]
        public void FromNode_gets_SeeAlsos()
        {
            string filter_text =
@"<Change ID='1' Description='URL reference'>
    <SeeAlso Description='Always remember, Google says do not do evil.' Target='http://www.google.com' />
</Change>
";
            var node = Node_FromText( filter_text );

            Change change = (Change)port.CompoundFilter_FromNode( node );
            Assert.That( change.SeeAlsos.Count, Is.EqualTo( 1 ) );
            SeeAlso seeAlso = change.SeeAlsos[0];

            Assert.That( seeAlso.Description, Is.EqualTo( "Always remember, Google says do not do evil." ) );
            Assert.That( seeAlso.Target, Is.EqualTo( "http://www.google.com" ) );
        }

        [Test, ExpectedException( ExpectedMessage = "Don't understand a file of language [Jabberwocky]." )]
        public void FromNode_unknown_Language_throws()
        {
            var node = Node_FromText( @"<Change ID='1212' Language='Jabberwocky' />" );
            port.CompoundFilter_FromNode( node );
        }

        public XmlNode Node_FromText( string text )
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml( text );
            return doc.ChildNodes[0];
        }

        [Test]
        public void SeeAlso_FromNode_empty()
        {
            var node = Node_FromText( @"<SeeAlso />" );
            var seeAlso = port.SeeAlso_FromNode( node );
            Assert.That( string.IsNullOrEmpty( seeAlso.Description ) );
        }

        [Test]
        public void SeeAlso_FromNode()
        {
            var node = Node_FromText( @"<SeeAlso Description='basic FromNode.' />" );
            var seeAlso = port.SeeAlso_FromNode( node );
            Assert.That( seeAlso.Description, Is.EqualTo( "basic FromNode." ) );
        }

        [Test]
        public void SeeAlso_FromNode_SVN()
        {
            var node = Node_FromText( @"<SeeAlso Target='myrepo/myfile.cs' Commit='7890' Description='This is how we do it.' TargetType='SVN' />" );
            var seeAlso = port.SeeAlso_FromNode( node );
            Assert.That( seeAlso.Target, Is.EqualTo( "myrepo/myfile.cs" ) );
            Assert.That( seeAlso.Commit, Is.EqualTo( "7890" ) );
            Assert.That( seeAlso.Description, Is.EqualTo( "This is how we do it." ) );
        }

        [Test]
        public void SeeAlso_FromNode_URL()
        {
            var node = Node_FromText( @"<SeeAlso Target='site/page.htm' Description='URL' TargetType='URL' />" );
            var seeAlso = port.SeeAlso_FromNode( node );
            Assert.That( seeAlso.Target, Is.EqualTo( "site/page.htm" ) );
            Assert.That( seeAlso.Description, Is.EqualTo( "URL" ) );
        }
        #endregion

        #region Integration round-trips

        private Clause UsePersisters_Filter
        {
            get
            {
                Clause use_persisters = new Change
                {
                    ID = "Pers_1",
                    Description = "Use persisters instead of direct DB access",
                };

                use_persisters.Children.Add( new Clause { ContentPattern = "(XADR|Oracle|NHibernate)" } );
                use_persisters.Children.Add( new Clause { NamePattern = "(Persister|Service)", Operator = ClauseOperator.Not } );
                use_persisters.Children.Add( new Clause { NamePattern = "(Xadr|TestXadr)", Operator = ClauseOperator.Not } );

                Clause special_cases = new Clause { Operator = ClauseOperator.Or, ID = "Special cases" };
                special_cases.Children.Add( new Clause { NamePattern = "this.cs" } );
                special_cases.Children.Add( new Clause { NamePattern = "that.cs", Operator = ClauseOperator.Or } );
                special_cases.Children.Add( new Clause { NamePattern = "another.cs", Operator = ClauseOperator.Or } );

                use_persisters.Children.Add( special_cases );

                return use_persisters;
            }
        }

        private const string UsePersisters_Text = 
@"    <Change ID='Pers_1' Description='Use persisters instead of direct DB access'>
        <When ContentPattern='(XADR|Oracle|NHibernate)' />
        <AndNot NamePattern='(Persister|Service)' />
        <AndNot NamePattern='(Xadr|TestXadr)' />
        <Or ID='Special cases'>
            <When NamePattern='this.cs' />
            <Or NamePattern='that.cs' />
            <Or NamePattern='another.cs' />
        </Or>
    </Change>
";

        [Test]
        public void use_persisters_instead_of_direct_db_access_FromNodes()
        {
            var usePersistersNode = Node_FromText( UsePersisters_Text );
            Clause filterFromNode = port.CompoundFilter_FromNode( usePersistersNode );

            Clause expectedFilter = UsePersisters_Filter;

            //multiple asserts because I'm not getting very good visibility into the problem.
            Clause thisFromNode = filterFromNode.Children[3].Children[0];
            Clause thisExpected = expectedFilter.Children[3].Children[0];
            Assert.That( thisFromNode.Equals( thisExpected ) );
            Assert.That( filterFromNode.Children[3].Equals( expectedFilter.Children[3] ) );
            Assert.That( filterFromNode.Equals( expectedFilter ) );
        }
        #endregion
    }
}
