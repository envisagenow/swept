//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Linq;
using NUnit.Framework;
using System.Xml;
using System.Collections.Generic;
using System.Xml.Linq;

namespace swept.Tests
{
    [TestFixture]
    public class XmlPortTests
    {
        private RuleCatalog getRuleCatalog( string ruleText )
        {
            XDocument xml = XDocument.Parse( ruleText );
            var port = new XmlPort();
            return port.RuleCatalog_FromXDocument( xml );
        }

        private List<string> getExclusions( string catalogText )
        {
            XDocument xml = XDocument.Parse( catalogText );
            var port = new XmlPort();
            return port.ExcludedFolders_FromXDocument( xml );
        }

        [Test]
        public void ExcludedFolders_are_optional()
        {
            var exclusions = getExclusions( "<SweptProjectData><RuleCatalog></RuleCatalog></SweptProjectData>" );

            Assert.That( exclusions.Count(), Is.EqualTo( 0 ) );
        }

        [Test]
        public void ExcludedFolders_are_read_from_library()
        {
            var exclusions = getExclusions( "<SweptProjectData><RuleCatalog></RuleCatalog><ExcludedFolders>  bin, .svn  </ExcludedFolders></SweptProjectData>" );

            Assert.That( exclusions.Count(), Is.EqualTo( 2 ) );
            Assert.That( exclusions.First(), Is.EqualTo( "bin" ) );
            Assert.That( exclusions.Last(), Is.EqualTo( ".svn" ) );
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
