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

        // TODO--0.N: public void Removed_Changes_are_saved()
        // TODO--0.N: public void Removed_Changes_are_loaded()

        #region ToText
        [Test]
        public void Filter_ToText_empty()
        {
            CompoundFilter filter = new Change();

            string serializedFilter = port.ToText( filter );
            string expectedFilter =
@"    <Change />
";
            Assert.AreEqual( expectedFilter, serializedFilter );
        }

        [Test]
        public void Filter_ToText_attributes()
        {
            CompoundFilter filter = new Change
            {
                ID = "thing",
                Description = "for my test",
                ContentPattern = "bad code",
                NamePattern = "(affected|relevant)_file",
                Language = FileLanguage.CSharp,
                Subpath = @"down\\(here|there)",
            };

            string serializedFilter = port.ToText( filter );
            string expectedFilter =
@"    <Change ID='thing' Description='for my test' Subpath='down\\(here|there)' NamePattern='(affected|relevant)_file' Language='CSharp' ContentPattern='bad code' />
";
            Assert.AreEqual( expectedFilter, serializedFilter );
        }

        [Test]
        public void Filter_ToText_children()
        {
            CompoundFilter filter = new Change{
                ID = "parent",
                Description = "outer"
            };
            filter.Children.Add( new CompoundFilter { ID = "child1" } );
            filter.Children.Add( new CompoundFilter { ID = "child2", Operator = FilterOperator.Or } );

            string serializedFilter = port.ToText( filter );
            string expectedFilter =
@"    <Change ID='parent' Description='outer'>
        <When ID='child1' />
        <Or ID='child2' />
    </Change>
";
            Assert.AreEqual( expectedFilter, serializedFilter );
        }

        [Test]
        public void shows_manual_completion_when_allowed()
        {
            Change change = new Change { ID = "permitted", ManualCompletion = true };
            string serializedFilter = port.ToText( change );

            string expectedText = @"    <Change ID='permitted' ManualCompletion='Allowed' />
";
            Assert.That( serializedFilter, Is.EqualTo( expectedText ) );

            change.ManualCompletion = false;
            serializedFilter = port.ToText( change );

            expectedText = @"    <Change ID='permitted' />
";
            Assert.That( serializedFilter, Is.EqualTo( expectedText ) );
        }

        [Test]
        public void shows_SeeAlsos()
        {
            Change change = new Change { ID = "references" };
            var newSeeAlso = new SeeAlso { 
                Description = "Google is your friend.", 
                Target = "http://www.google.com",
                TargetType = TargetType.URL
            };
            change.SeeAlsos.Add( newSeeAlso );
            string serializedFilter = port.ToText( change );

            string expectedText = 
@"    <Change ID='references'>
        <SeeAlso Description='Google is your friend.' Target='http://www.google.com' TargetType='URL' />
    </Change>
";
            Assert.That( serializedFilter, Is.EqualTo( expectedText ) );
        }

        #endregion

        #region SeeAlso
        [Test]
        public void SeeAlso_empty_ToXml()
        {
            var seeAlso = new SeeAlso();
            string expectedText = "        <SeeAlso TargetType='URL' />\r\n";
            string actualText = port.ToText( seeAlso );
            Assert.That( actualText, Is.EqualTo( expectedText ) );
        }

        [Test]
        public void SeeAlso_URL_ToXml()
        {
            var seeAlso = new SeeAlso
            {
                Description = "simple",
                Target = "helloworld.com",
                TargetType = TargetType.URL
            };
            string expectedText = "        <SeeAlso Description='simple' Target='helloworld.com' TargetType='URL' />\r\n";
            string actualText = port.ToText( seeAlso );
            Assert.That( actualText, Is.EqualTo( expectedText ) );
        }

        [Test]
        public void full_SeeAlso_SVN_ToXml()
        {
            var seeAlso = new SeeAlso
            {
                Target = "TextOutputAdapter.cs",
                Commit = "3427",
                TargetType = TargetType.SVN
            };
            string expectedText = "        <SeeAlso Target='TextOutputAdapter.cs' Commit='3427' TargetType='SVN' />\r\n";
            string actualText = port.ToText( seeAlso );
            Assert.That( actualText, Is.EqualTo( expectedText ) );
        }

        [Test]
        public void full_SeeAlso_ProjectFile_ToXml()
        {
            var seeAlso = new SeeAlso { 
                Target = "utility\\MegaToString.cs", 
                TargetType = TargetType.File 
            };
            string expectedText = "        <SeeAlso Target='utility\\MegaToString.cs' TargetType='File' />\r\n";
            string actualText = port.ToText( seeAlso );
            Assert.That( actualText, Is.EqualTo( expectedText ) );
        }
        #endregion

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

            CompoundFilter filter = port.CompoundFilter_FromNode( node );
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
            var namesToOps = new Dictionary<string, FilterOperator>() {
                { "Either", FilterOperator.Or },
                { "Or", FilterOperator.Or },
                { "When", FilterOperator.And },
                { "And", FilterOperator.And },
                { "Change", FilterOperator.And },
                { "AndNot", FilterOperator.Not },
                { "Not", FilterOperator.Not },
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

            CompoundFilter filter = port.CompoundFilter_FromNode( node );
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

            CompoundFilter filter = port.CompoundFilter_FromNode( node );
            Assert.That( filter.ID, Is.EqualTo( "single" ) );
            Assert.That( filter.Children.Count, Is.EqualTo( 1 ) );
        }

        // TODO--0.3: finish feature: manual completion (UI checkbox)
        //  Manual completion:  Some filters may show false-positive, and we may want to give
        //  the developer a chance to manually mark a file as okay.  (Currently this is always on.)
        [Test]
        public void FromNode_sets_manual_completion_permission()
        {
            string filter_text =
@"<Change ID='single' ManualCompletion='Allowed' />
";
            var node = Node_FromText( filter_text );

            CompoundFilter filter = port.CompoundFilter_FromNode( node );
            Assert.IsTrue( filter.ManualCompletion );
        }

        [Test, ExpectedException( ExpectedMessage = "Don't understand the manual completion permission 'Allwoed'." )]
        public void FromNode_throws_on_manual_completion_typo()
        {
            string filter_text =
@"<Change ID='typo' ManualCompletion='Allwoed' />";
            var node = Node_FromText( filter_text );

            port.CompoundFilter_FromNode( node );  // bang
        }


        [Test]
        public void FromNode_retrieves_attributes()
        {
            string filter_text =
@"<Change ID='1212' Description='Through and through'>
    <When Subpath='swords' NamePattern='vorpal.cs' Language='CSharp' ContentPattern='using System.Snicker.Snack;' />
</Change>
";
            var node = Node_FromText( filter_text );

            CompoundFilter filter = port.CompoundFilter_FromNode( node );
            Assert.That( filter.Children.Count, Is.EqualTo( 1 ) );
            var child = filter.Children[0];

            Assert.That( filter.ID, Is.EqualTo( "1212" ) );
            Assert.That( filter.Description, Is.EqualTo( "Through and through" ) );

            Assert.That( child.Operator, Is.EqualTo( FilterOperator.And ) );
            Assert.That( child.Subpath, Is.EqualTo( "swords" ) );
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

        private CompoundFilter UsePersisters_Filter
        {
            get
            {
                CompoundFilter use_persisters = new Change
                {
                    ID = "Pers_1",
                    Description = "Use persisters instead of direct DB access",
                };

                use_persisters.Children.Add( new CompoundFilter { ContentPattern = "(XADR|Oracle|NHibernate)" } );
                use_persisters.Children.Add( new CompoundFilter { NamePattern = "(Persister|Service)", Operator = FilterOperator.Not } );
                use_persisters.Children.Add( new CompoundFilter { Subpath = "(Xadr|TestXadr)", Operator = FilterOperator.Not } );

                CompoundFilter special_cases = new CompoundFilter { Operator = FilterOperator.Or, ID = "Special cases" };
                special_cases.Children.Add( new CompoundFilter { NamePattern = "this.cs" } );
                special_cases.Children.Add( new CompoundFilter { NamePattern = "that.cs", Operator = FilterOperator.Or } );
                special_cases.Children.Add( new CompoundFilter { NamePattern = "another.cs", Operator = FilterOperator.Or } );

                use_persisters.Children.Add( special_cases );

                return use_persisters;
            }
        }

        private const string UsePersisters_Text = 
@"    <Change ID='Pers_1' Description='Use persisters instead of direct DB access'>
        <When ContentPattern='(XADR|Oracle|NHibernate)' />
        <AndNot NamePattern='(Persister|Service)' />
        <AndNot Subpath='(Xadr|TestXadr)' />
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
            CompoundFilter filterFromNode = port.CompoundFilter_FromNode( usePersistersNode );

            CompoundFilter expectedFilter = UsePersisters_Filter;

            //multiple asserts because I'm not getting very good visibility into the problem.
            CompoundFilter thisFromNode = filterFromNode.Children[3].Children[0];
            CompoundFilter thisExpected = expectedFilter.Children[3].Children[0];
            Assert.That( thisFromNode.Equals( thisExpected ) );
            Assert.That( filterFromNode.Children[3].Equals( expectedFilter.Children[3] ) );
            Assert.That( filterFromNode.Equals( expectedFilter ) );
        }

        [Test]
        public void use_persisters_instead_of_direct_db_access_RoundTrip()
        {
            var usePersistersNode = Node_FromText( UsePersisters_Text );
            CompoundFilter filterFromNode = port.CompoundFilter_FromNode( usePersistersNode );

            string textFromFilter = port.ToText( filterFromNode );

            Assert.That( textFromFilter, Is.EqualTo( UsePersisters_Text ) );
        }

        [Test]
        public void use_persisters_instead_of_direct_db_access_ToText()
        {
            CompoundFilter use_persisters = UsePersisters_Filter;
            string textFromFilter = port.ToText( use_persisters );

            Assert.That( textFromFilter, Is.EqualTo( UsePersisters_Text ) );
        }

        [Test]
        public void Can_serialize_full_Change_ToXml()
        {
            Change change = new Change
            {
                ID = "Uno",
                Description = "Eliminate profanity from error messages.",
                ContentPattern = "contains this",
                Subpath = @"utilities\error_handling",
                NamePattern = "messages",
                Language = FileLanguage.CSharp,
                Operator = FilterOperator.Or, // note that the operator isn't expressed at the top level.
                ManualCompletion = true,
            };

            string xmlText = port.ToText( change );

            string expectedXml = string.Format(
                "    <Change ID='{0}' Description='{1}' ManualCompletion='{2}' Subpath='{3}' NamePattern='{4}' Language='{5}' ContentPattern='{6}' />{7}",
                change.ID, change.Description, change.ManualCompletionString,
                change.Subpath, change.NamePattern, change.Language,
                change.ContentPattern,
                Environment.NewLine
            );

            Assert.AreEqual( expectedXml, xmlText );
        }
        #endregion
    }
}
