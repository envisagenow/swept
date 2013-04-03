//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using NUnit.Framework;
using System.Xml;
using System.Collections.Generic;

namespace swept.Tests
{
    [TestFixture]
    public class XmlPortTests
    {
        private RuleCatalog getRuleCatalog( string ruleText )
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml( ruleText );
            var port = new XmlPort();
            return port.RuleCatalog_FromXmlDocument( xml );
        }

        [Test]
        public void Populates_FailMode_from_attribute()
        {
            var cat = getRuleCatalog( "<SweptProjectData><RuleCatalog><Rule ID='this' FailMode='Any'> ^CSharp </Rule></RuleCatalog></SweptProjectData>" );
            List<Rule> rules = cat.GetSortedRules();
            Assert.That( rules.Count, Is.EqualTo( 1 ) );

            Assert.That( rules[0].FailOn, Is.EqualTo( RuleFailOn.Any ) );
        }

        [Test]
        public void Populates_FailMode_increase_from_attribute()
        {
            var cat = getRuleCatalog( "<SweptProjectData><RuleCatalog><Rule ID='this' FailMode='Increase'> ^CSharp </Rule></RuleCatalog></SweptProjectData>" );
            List<Rule> rules = cat.GetSortedRules();
            Assert.That( rules.Count, Is.EqualTo( 1 ) );

            Assert.That( rules[0].FailOn, Is.EqualTo( RuleFailOn.Increase ) );
        }

        [Test]
        public void RuleCatalog_from_IncorrectXML_Throws()
        {
            var ex = Assert.Throws<Exception>( () => getRuleCatalog( "<x>eek!</x>" ) );
            Assert.That( ex.Message, Is.EqualTo( "Document must have a <RuleCatalog> node inside a <SweptProjectData> node.  Please supply one." ) );
        }

        [Test]
        public void FailMode_parse_gives_clear_exception_on_invalid_value()
        {
            string badMode = "<SweptProjectData><RuleCatalog><Rule ID='this' FailMode='Fake'> ^CSharp </Rule></RuleCatalog></SweptProjectData>";
            var ex = Assert.Throws<Exception>( () => getRuleCatalog( badMode ) );
            Assert.That( ex.Message, Is.EqualTo( "Rule ID [this] has an unknown FailMode value [Fake]." ) );
        }
    }
}
