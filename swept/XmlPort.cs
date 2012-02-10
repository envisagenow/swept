//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2012 Jason Cole and Envisage Technologies Corp.
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
        public ChangeCatalog ChangeCatalog_FromXmlDocument( XmlDocument doc )
        {
            XmlNode node = doc.SelectSingleNode( "SweptProjectData/ChangeCatalog" );

            if (node == null)
                throw new Exception( "Document must have a <ChangeCatalog> node inside a <SweptProjectData> node.  Please supply one." );

            return ChangeCatalog_FromNode( node );
        }


        public ChangeCatalog ChangeCatalog_FromNode( XmlNode node )
        {
            ChangeCatalog cat = new ChangeCatalog();

            XmlNodeList changes = node.SelectNodes( "Change" );
            foreach (XmlNode changeNode in changes)
            {
                cat.Add( Change_FromNode( changeNode ) );
            }
            return cat;
        }

        public const string cfa_ID = "ID";
        public const string cfa_Description = "Description";
        public const string cfa_FailMode = "FailMode";
        public const string cfa_FailLimit = "Limit";

        private Change Change_FromNode( XmlNode changeNode )
        {
            Change change = new Change();

            if (changeNode.Attributes[cfa_ID] != null)
                change.ID = changeNode.Attributes[cfa_ID].Value;
            else
                throw new Exception( "Changes must have IDs at their top level." );

            if (changeNode.Attributes[cfa_Description] != null)
                change.Description = changeNode.Attributes[cfa_Description].Value;

            if (changeNode.Attributes[cfa_FailMode] != null)
            {
                string failText = changeNode.Attributes[cfa_FailMode].Value;
                try
                {
                    //  When, MS?:  change.BuildFail = Enum.Parse<BuildFailMode>( failText );
                    change.BuildFail = (BuildFailMode)Enum.Parse( typeof( BuildFailMode ), failText );
                    if (change.BuildFail == BuildFailMode.Over)
                    {
                        try
                        {
                            string failOver = changeNode.Attributes[cfa_FailLimit].Value;
                            change.BuildFailOverLimit = int.Parse( failOver );
                        }
                        catch( Exception failEx )
                        {
                            string msg = String.Format( "Found no fail over 'Limit' for Change ID [{0}].", change.ID );
                            throw new Exception( msg, failEx );
                        }
                    }
                }
                catch (ArgumentException argEx)
                {
                    throw new Exception( String.Format( "Change ID [{0}] has an unknown {1} value [{2}].", change.ID, cfa_FailMode, failText ), argEx );
                }
            }

            // can't do it because I'm not upg'd to XDoc and XElems yet...
            //string ruleText = changeNode.ChildNodes.First( c => c.NodeType == XmlNodeType.Text );
            var sb = new StringBuilder();
            foreach (XmlNode child in changeNode.ChildNodes)
            {
                switch (child.NodeType)
                {
                case XmlNodeType.Text:
                    sb.AppendLine( child.Value.Trim() );
                    break;

                case XmlNodeType.Element:
                    change.SeeAlsos.Add( SeeAlso_FromNode( child ) );
                    break;

                default:
                    throw new Exception( String.Format( "Not ready for child node [{0}] typed [{1}].", child.Value, child.NodeType ) );
                }
            }
            string ruleText = sb.ToString();
            change.Subquery = BuildChangeRuleQuery( ruleText );

            return change;
        }

        internal ISubquery BuildChangeRuleQuery( string queryText )
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
