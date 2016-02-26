//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using NUnit.Framework;
using swept;
using System.Collections.Generic;
using System;
using System.Linq;
using swept.DSL;

namespace swept.Tests
{
    [TestFixture]
    public class RuleCatalogTests
    {
        private RuleCatalog cat;
        [SetUp]
        public void given_a_catalog_with_several_rules()
        {
            cat = new RuleCatalog();

            Rule avoidAliasUsing = new Rule { ID = "e1", Description = "Don't use 'using' to alias.", Subquery = new QueryLanguageNode { Language = FileLanguage.CSharp } };
            cat.Add(avoidAliasUsing);

            cat.Add(new Rule { ID = "e2", Description = "Upgrade to XHTML", Subquery = new QueryLanguageNode { Language = FileLanguage.HTML } });
            cat.Add(new Rule { ID = "e3", Description = "Put <title> on all pages", Subquery = new QueryLanguageNode { Language = FileLanguage.HTML } });
        }

        [Test]
        public void GetRules_returns_only_pertinent_Rules()
        {
            List<Rule> rules = cat.GetRulesForFile(new SourceFile("hello_world.cs"));
            Assert.AreEqual(1, rules.Count);
            Assert.AreEqual("e1", rules[0].ID);
        }

        [Test]
        public void GetRules_returns_no_Rules_when_language_filters_enough()
        {
            List<Rule> rules = cat.GetRulesForFile(new SourceFile("hello_style.css"));
            Assert.AreEqual(0, rules.Count);
        }

        [Test]
        public void Empty_Catalog_returns_empty_list()
        {
            List<Rule> rules = cat.GetRulesForFile(new SourceFile("hello_style.css"));
            Assert.AreEqual(0, rules.Count);
        }

        [Test]
        public void Duplicate_IDs_not_allowed()
        {
            RuleCatalog cat = new RuleCatalog();

            Rule a_17a = new Rule { ID = "a_17", Description = "I was here first!" };
            Rule a_17b = new Rule { ID = "a_17", Description = "You impostor!" };

            cat.Add(a_17a);
            var ex = Assert.Throws<Exception>(() => cat.Add(a_17b));
            Assert.That(ex.Message, Is.EqualTo("Swept cannot add the rule \"You impostor!\" with the ID [a_17], the rule \"I was here first!\" already has that ID."));
        }

        [Test]
        public void GetSortedRules_with_empty_rule_filters_and_adhoc_rule_returns_adhoc_rule_only()
        {
            var rules = cat.GetSortedRules(new List<string>(), "^CSharp and @'Test' and ~'ExpectedException'");
            Assert.That(rules.Count, Is.EqualTo(1));
            Assert.That(rules[0].ID, Is.EqualTo("adHoc_01"));
            Assert.That(rules[0].Description, Is.EqualTo("^CSharp and @'Test' and ~'ExpectedException'"));
        }

        [Test]
        public void Rules_sort_alphabetically_by_ID()
        {
            Rule a_17 = new Rule { ID = "a_17", };
            Rule a_18 = new Rule { ID = "a_18", };
            Rule a_117 = new Rule { ID = "a_117", };
            Rule a_177 = new Rule { ID = "a_177", };
            Rule b_52 = new Rule { ID = "b_52", };

            RuleCatalog cat = new RuleCatalog();

            cat.Add(b_52);
            cat.Add(a_17);
            cat.Add(a_177);
            cat.Add(a_117);
            cat.Add(a_18);

            var rules = cat.GetSortedRules(new List<string>());
            Assert.That(rules[0].ID, Is.EqualTo(a_117.ID));
            Assert.That(rules[1].ID, Is.EqualTo(a_17.ID));
            Assert.That(rules[2].ID, Is.EqualTo(a_177.ID));
            Assert.That(rules[3].ID, Is.EqualTo(a_18.ID));
            Assert.That(rules[4].ID, Is.EqualTo(b_52.ID));
        }

        [Test]
        public void GetSortedRules_filters_when_list_specifies_rules()
        {
            var ruleList = new List<string> { "B_52", "a_117", "not_in_rule_list" };

            Rule a_17 = new Rule { ID = "a_17", };
            Rule a_18 = new Rule { ID = "a_18", };
            Rule a_117 = new Rule { ID = "a_117", };
            Rule a_177 = new Rule { ID = "a_177", };
            Rule b_52 = new Rule { ID = "b_52", };

            RuleCatalog cat = new RuleCatalog();

            cat.Add(b_52);
            cat.Add(a_17);
            cat.Add(a_177);
            cat.Add(a_117);
            cat.Add(a_18);

            var rules = cat.GetSortedRules(ruleList);
            Assert.That(rules.Count, Is.EqualTo(2));
            Assert.That(rules[0].ID, Is.EqualTo(a_117.ID));
            Assert.That(rules[1].ID, Is.EqualTo(b_52.ID), "Case insensitive match on rule argument");
        }

        [Test]
        public void Filter_out_rules_on_single_tag()
        {
            var tags = new List<Pick> { new Pick { Domain = PickDomain.Tag, Value = "dev" } };

            Rule a_17 = new Rule { ID = "biz_cares", Tags = new List<string> { "weasels", "devotees", "biz" } };
            Rule b_52 = new Rule { ID = "dev_cares", Tags = new List<string> { "weasels", "dev" } };

            RuleCatalog cat = new RuleCatalog();

            cat.Add(b_52);
            cat.Add(a_17);

            List<Rule> actualRules;
            List<Rule> expectedRules = new List<Rule> { b_52 };

            actualRules = cat.FilterRulesOnPicks(cat._rules, tags);


            Assert.That(actualRules.All(r => expectedRules.Contains(r)));
            Assert.That(expectedRules.All(r => actualRules.Contains(r)));
        }

        [Test]
        public void Filter_out_rules_on_several_tags()
        {
            var tags = new List<Pick> { new Pick { Domain = PickDomain.Tag, Value = "biz" }, new Pick { Domain = PickDomain.Tag, Value = "cust" }};

            Rule a_17 = new Rule { ID = "biz_cares", Tags = new List<string> { "weasels", "devotees", "biz" } };
            Rule a_99 = new Rule { ID = "cust_cares", Tags = new List<string> { "weasels", "devotees", "cust" } };
            Rule b_52 = new Rule { ID = "dev_cares", Tags = new List<string> { "weasels", "dev" } };

            RuleCatalog cat = new RuleCatalog();

            cat.Add(b_52);
            cat.Add(a_17);
            cat.Add(a_99);

            List<Rule> actualRules;
            List<Rule> expectedRules = new List<Rule> { a_17, a_99 };

            actualRules = cat.FilterRulesOnPicks(cat._rules, tags);

            Assert.That(actualRules.All(r => expectedRules.Contains(r)));
            Assert.That(expectedRules.All(r => actualRules.Contains(r)));
        }


        [Test]
        public void Filter_out_rules_on_single_excluded_tag()
        {
            var tags = new List<Pick> { new Pick { Domain = PickDomain.Tag, Value = "-dev" } };

            Rule a_17 = new Rule { ID = "biz_cares", Tags = new List<string> { "weasels", "devotees", "biz" } };
            Rule b_52 = new Rule { ID = "dev_cares", Tags = new List<string> { "weasels", "dev" } };

            RuleCatalog cat = new RuleCatalog();

            cat.Add(b_52);
            cat.Add(a_17);

            List<Rule> actualRules;
            List<Rule> expectedRules = new List<Rule> { a_17 };

            actualRules = cat.FilterRulesOnPicks(cat._rules, tags);


            Assert.That(actualRules.All(r => expectedRules.Contains(r)));
            Assert.That(expectedRules.All(r => actualRules.Contains(r)));
        }


        [Test]
        public void Filter_out_rules_on_includes_and_excludes()
        {
            var tags = new List<Pick> { new Pick { Domain = PickDomain.Tag, Value = "dev" }, new Pick { Domain = PickDomain.Tag, Value = "-devotees" } };

            Rule a_17 = new Rule { ID = "biz_cares", Tags = new List<string> { "weasels", "devotees", "biz" } };
            Rule a_99 = new Rule { ID = "dev_cares", Tags = new List<string> { "weasels", "devotees", "dev" } };
            Rule b_52 = new Rule { ID = "dev_cares2", Tags = new List<string> { "weasels", "dev" } };
            Rule c_64 = new Rule { ID = "dev_cares3", Tags = new List<string> { "weasels", "dev", "devoted" } };

            RuleCatalog cat = new RuleCatalog();

            cat.Add(b_52);
            cat.Add(a_17);
            cat.Add(a_99);
            cat.Add(c_64);

            List<Rule> actualRules;
            List<Rule> expectedRules = new List<Rule> { b_52, c_64 };

            actualRules = cat.FilterRulesOnPicks(cat._rules, tags);


            Assert.That(actualRules.All(r => expectedRules.Contains(r)));
            Assert.That(expectedRules.All(r => actualRules.Contains(r)));
        }
    }
}
