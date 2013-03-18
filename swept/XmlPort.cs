//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Xml;
using swept.DSL;
using Antlr.Runtime;
using System.Text;

namespace swept
{
    internal class XmlPort
    {
        public RuleCatalog RuleCatalog_FromXmlDocument( XmlDocument doc )
        {
            XmlNode node = doc.SelectSingleNode( "SweptProjectData/RuleCatalog" );

            if (node == null)
                throw new Exception( "Document must have a <RuleCatalog> node inside a <SweptProjectData> node.  Please supply one." );

            return RuleCatalog_FromNode( node );
        }


        public RuleCatalog RuleCatalog_FromNode( XmlNode node )
        {
            RuleCatalog cat = new RuleCatalog();

            XmlNodeList rules = node.SelectNodes( "Rule" );
            foreach (XmlNode ruleNode in rules)
            {
                cat.Add( Rule_FromNode( ruleNode ) );
            }
            return cat;
        }

        public const string cfa_ID = "ID";
        public const string cfa_Description = "Description";
        public const string cfa_FailMode = "FailMode";
        public const string cfa_FailLimit = "Limit";

        private Rule Rule_FromNode( XmlNode ruleNode )
        {
            Rule rule = new Rule();

            if (ruleNode.Attributes[cfa_ID] != null)
                rule.ID = ruleNode.Attributes[cfa_ID].Value;
            else
                throw new Exception( "Rules must have IDs at their top level." );

            if (ruleNode.Attributes[cfa_Description] != null)
                rule.Description = ruleNode.Attributes[cfa_Description].Value;

            if (ruleNode.Attributes[cfa_FailMode] != null)
            {
                string failText = ruleNode.Attributes[cfa_FailMode].Value;
                try
                {
                    //  When, MS?:  rule.RunFail = Enum.Parse<RunFailMode>( failText );
                    rule.FailOn = (RuleFailOn)Enum.Parse( typeof( RuleFailOn ), failText );
                    if (rule.FailOn == RuleFailOn.Over)
                    {
                        try
                        {
                            string failOver = ruleNode.Attributes[cfa_FailLimit].Value;
                            rule.RunFailOverLimit = int.Parse( failOver );
                        }
                        catch( Exception failEx )
                        {
                            string msg = String.Format( "Found no integer 'Limit' for Rule ID [{0}].", rule.ID );
                            throw new Exception( msg, failEx );
                        }
                    }
                }
                catch (ArgumentException argEx)
                {
                    throw new Exception( String.Format( "Rule ID [{0}] has an unknown {1} value [{2}].", rule.ID, cfa_FailMode, failText ), argEx );
                }
            }

            // can't do it because I'm not upg'd to XDoc and XElems yet...
            //string ruleText = ruleNode.ChildNodes.First( c => c.NodeType == XmlNodeType.Text );
            var sb = new StringBuilder();
            foreach (XmlNode child in ruleNode.ChildNodes)
            {
                switch (child.NodeType)
                {
                case XmlNodeType.Text:
                    sb.AppendLine( child.Value.Trim() );
                    break;

                case XmlNodeType.Element:
                    rule.SeeAlsos.Add( SeeAlso_FromNode( child ) );
                    break;

                default:
                    throw new Exception( String.Format( "Not ready for child node [{0}] typed [{1}].", child.Value, child.NodeType ) );
                }
            }
            string ruleText = sb.ToString();
            rule.Subquery = BuildRuleQuery( ruleText );

            return rule;
        }

        internal ISubquery BuildRuleQuery( string queryText )
        {
            var lexer = new ChangeRuleLexer( new ANTLRStringStream( queryText ) );
            var parser = new ChangeRuleParser( new CommonTokenStream( lexer ) );
            
            return parser.expression();
        }
    
        public SeeAlso SeeAlso_FromNode( XmlNode node )
        {
            SeeAlso seeAlso = new SeeAlso();
            if (node.Attributes["Description"] != null)
            {
                seeAlso.Description = node.Attributes["Description"].Value;
            }

            if (node.Attributes["Target"] != null)
            {
                seeAlso.Target = node.Attributes["Target"].Value;
            }

            if (node.Attributes["TargetType"] != null)
            {
                string typeString = node.Attributes["TargetType"].Value;
                seeAlso.TargetType = (TargetType)Enum.Parse( typeof( TargetType ), typeString );
            }

            if (node.Attributes["Commit"] != null)
            {
                seeAlso.Commit = node.Attributes["Commit"].Value;
            }

            return seeAlso;
        }

    }
}
