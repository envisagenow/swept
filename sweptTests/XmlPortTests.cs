//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Linq;
using NUnit.Framework;
using System.Xml;
using System.Collections.Generic;
using System.Xml.Linq;
using swept.DSL;

namespace swept.Tests
{
    [TestFixture]
    public class XmlPortTests
    {
        XmlPort _port;

        [SetUp]
        public void SetUp()
        {
            _port = new XmlPort();
        }

        private RuleCatalog getRuleCatalog(string ruleText)
        {
            return _port.RuleCatalog_FromXDocument(XDocument.Parse(ruleText));
        }

        private List<string> getExclusions(string catalogText)
        {
            XDocument xml = XDocument.Parse(catalogText);
            return _port.ExcludedFolders_FromXDocument(xml);
        }

        [Test]
        public void ExcludedFolders_are_optional()
        {
            var exclusions = getExclusions("<SweptProjectData><RuleCatalog></RuleCatalog></SweptProjectData>");

            Assert.That(exclusions.Count(), Is.EqualTo(0));
        }

        [Test]
        public void ExcludedFolders_are_read_from_library()
        {
            var exclusions = getExclusions("<SweptProjectData><RuleCatalog></RuleCatalog><ExcludedFolders>  bin, .svn  </ExcludedFolders></SweptProjectData>");

            Assert.That(exclusions.Count(), Is.EqualTo(2));
            Assert.That(exclusions.First(), Is.EqualTo("bin"));
            Assert.That(exclusions.Last(), Is.EqualTo(".svn"));
        }


        [TestCase("Any", RuleFailOn.Any)]
        [TestCase("None", RuleFailOn.None)]
        [TestCase("Increase", RuleFailOn.Increase)]
        public void Populates_FailMode(string failModeText, RuleFailOn failOn)
        {
            var ruleElement = XElement.Parse(String.Format("<Rule ID='this' FailMode='{0}'> ^CSharp </Rule>", failModeText));
            var rule = _port.Rule_FromElement(ruleElement);
            Assert.That(rule.FailOn, Is.EqualTo(failOn));
        }

        [Test]
        public void Populates_Note_from_child_element()
        {
            var ruleElement = XElement.Parse("<Rule ID='this'> ^CSharp <Note>This is peculiar.</Note> </Rule>");
            var rule = _port.Rule_FromElement(ruleElement);
            Assert.That(rule.Notes, Is.EqualTo("This is peculiar."));
        }

        [Test]
        public void RuleCatalog_from_IncorrectXML_Throws()
        {
            var ex = Assert.Throws<Exception>(() => getRuleCatalog("<x>eek!</x>"));
            Assert.That(ex.Message, Is.EqualTo("Document must have a <RuleCatalog> node inside a <SweptProjectData> node.  Please supply one."));
        }

        [Test]
        public void FailMode_parse_failure_throws_clear_message_through_port()
        {
            string parseFailMessage = "Rule with ID [this] has an unknown FailMode value [Fake].";

            var ruleElement = XElement.Parse("<Rule ID='this' FailMode='Fake'> ^CSharp </Rule>");
            var ruleParseException = Assert.Throws<Exception>(() => _port.Rule_FromElement(ruleElement));
            Assert.That(ruleParseException.Message, Is.EqualTo(parseFailMessage));

            string badMode = "<SweptProjectData><RuleCatalog><Rule ID='this' FailMode='Fake'> ^CSharp </Rule></RuleCatalog></SweptProjectData>";
            var catalogParseException = Assert.Throws<Exception>(() => getRuleCatalog(badMode));
            Assert.That(catalogParseException.Message, Is.EqualTo(parseFailMessage));
        }

        [Test]
        public void GetSortedRules_with_empty_rule_filters_and_adhoc_rule_returns_adhoc_rule()
        {
            string queryText = "^CSharp and @'Test' and ~'ExpectedException'";
            var rule = _port.GetAdHocRule(queryText);

            Assert.That(rule.ID, Is.EqualTo("adHoc_01"));
            Assert.That(rule.Description, Is.EqualTo(queryText));
            Assert.IsInstanceOf<OpIntersectionNode>(rule.Subquery);
            Assert.IsInstanceOf<OpIntersectionNode>(((OpBinaryNode)(rule.Subquery)).LHS);
            Assert.IsInstanceOf<QueryContentNode>(((OpBinaryNode)(rule.Subquery)).RHS);
        }



        [Test]
        public void Tags_Empty_By_Default()
        {
            var ruleElement = XElement.Parse("<Rule ID='this'> ^CSharp </Rule>");
            var rule = _port.Rule_FromElement(ruleElement);
            Assert.That(!rule.Tags.Any());
        }

        [Test]
        public void Blank_Tags_Are_Empty()
        {
            var ruleElement = XElement.Parse("<Rule ID='this' Tags=''> ^CSharp </Rule>");
            var rule = _port.Rule_FromElement(ruleElement);
            Assert.That(!rule.Tags.Any());
        }

        [Test]
        public void Single_Tag_Parsed_Correctly()
        {
            var ruleElement = XElement.Parse("<Rule ID='this' Tags='foo'> ^CSharp </Rule>");
            var rule = _port.Rule_FromElement(ruleElement);
            Assert.That(rule.Tags.Count, Is.EqualTo(1));
            Assert.That(rule.Tags[0], Is.EqualTo("foo"));
        }

        [Test]
        public void Multiple_Tags_Parsed_Correctly()
        {
            var ruleElement = XElement.Parse("<Rule ID='this' Tags='foo bar baz'> ^CSharp </Rule>");
            var rule = _port.Rule_FromElement(ruleElement);
            Assert.That(rule.Tags.Count, Is.EqualTo(3));
            Assert.That(rule.Tags[0], Is.EqualTo("foo"));
            Assert.That(rule.Tags[1], Is.EqualTo("bar"));
            Assert.That(rule.Tags[2], Is.EqualTo("baz"));
        }

    }
}
