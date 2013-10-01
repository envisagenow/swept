//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2013 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using swept.DSL;
using Antlr.Runtime;
using System.Xml.Linq;
using System.Linq;

namespace swept
{
    internal class XmlPort
    {

        public RuleCatalog RuleCatalog_FromXDocument( XDocument doc )
        {
            XElement node = doc.Descendants("SweptProjectData").Descendants("RuleCatalog").SingleOrDefault();

            if (node == null)
                throw new Exception( "Document must have a <RuleCatalog> node inside a <SweptProjectData> node.  Please supply one." );

            return RuleCatalog_FromElement( node );
        }

        public List<string> ExcludedFolders_FromXDocument( XDocument doc )
        {
            var projectData = doc.Descendants( "SweptProjectData" ).Single();
            XElement node = projectData.Descendants( "ExcludedFolders" ).SingleOrDefault();

            if (node == null)
                return new List<string>();

            return ExcludedFolders_FromElement( node );
        }

        public List<string> ExcludedFolders_FromElement( XElement element )
        {
            var exclusions = new List<string>();

            string rawText = element.Value;

            var folders = rawText.Split( new string[] { "," }, StringSplitOptions.RemoveEmptyEntries );
            foreach (var folder in folders)
            {
                exclusions.Add( folder.Trim() );
            }

            return exclusions;
        }

        public RuleCatalog RuleCatalog_FromElement( XElement element )
        {
            RuleCatalog cat = new RuleCatalog();

            var rules = element.Elements( "Rule" );
            foreach (XElement ruleElement in rules)
            {
                cat.Add( Rule_FromElement( ruleElement ) );
            }
            return cat;
        }

        public const string cfa_ID = "ID";
        public const string cfa_Description = "Description";
        public const string cfa_FailMode = "FailMode";

        protected internal Rule Rule_FromElement( XElement ruleElement )
        {
            Rule rule = new Rule();

            if (ruleElement.Attribute(cfa_ID) != null)
                rule.ID = ruleElement.Attribute(cfa_ID).Value;
            else
                throw new Exception( "Rules must have IDs at their top level." );

            if (ruleElement.Attribute(cfa_Description) != null)
                rule.Description = ruleElement.Attribute(cfa_Description).Value;

            if (ruleElement.Attribute(cfa_FailMode) != null)
            {
                string failText = ruleElement.Attribute(cfa_FailMode).Value;
                try
                {
                    //  When?:  rule.FailOn = RuleFailOn.Parse( failText );
                    //  Or: rule.FailOn =  failText.To<RuleFailOn>();
                    //  Or even:  rule.FailOn = Enum.Parse<RuleFailOn>( failText );
                    rule.FailOn = (RuleFailOn)Enum.Parse( typeof( RuleFailOn ), failText );
                }
                catch (ArgumentException argEx)
                {
                    throw new Exception( String.Format( "Rule with ID [{0}] has an unknown {1} value [{2}].", rule.ID, cfa_FailMode, failText ), argEx );
                }
            }

            XElement possibleNote = ruleElement.Elements( "Note" ).FirstOrDefault();
            if (possibleNote != null)
                rule.Notes = possibleNote.Value;

            foreach (var child in ruleElement.Descendants( "SeeAlso" ))
            {
                rule.SeeAlsos.Add( SeeAlso_FromElement( child ) );
            }

            rule.Subquery = BuildRuleQuery( ruleElement.Value );

            return rule;
        }

        internal ISubquery BuildRuleQuery( string queryText )
        {
            var lexer = new ChangeRuleLexer( new ANTLRStringStream( queryText ) );
            var parser = new ChangeRuleParser( new CommonTokenStream( lexer ) );
            
            return parser.expression();
        }

        public SeeAlso SeeAlso_FromElement( XElement element )
        {
            SeeAlso seeAlso = new SeeAlso();
            if (element.Attribute("Description") != null)
            {
                seeAlso.Description = element.Attribute("Description").Value;
            }

            if (element.Attribute("Target") != null)
            {
                seeAlso.Target = element.Attribute("Target").Value;
            }

            if (element.Attribute("TargetType") != null)
            {
                string typeString = element.Attribute("TargetType").Value;
                seeAlso.TargetType = (TargetType)Enum.Parse( typeof( TargetType ), typeString );
            }

            if (element.Attribute("Commit") != null)
            {
                seeAlso.Commit = element.Attribute("Commit").Value;
            }

            return seeAlso;
        }
    }
}
