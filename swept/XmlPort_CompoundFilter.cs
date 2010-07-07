//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace swept
{
    public class XmlPort_CompoundFilter
    {
        public const string cfa_ID = "ID";
        public const string cfa_Description = "Description";
        public const string cfa_ManualCompletion = "ManualCompletion";
        public const string cfa_Subpath = "Subpath";
        public const string cfa_NamePattern = "NamePattern";
        public const string cfa_Language = "Language";
        public const string cfa_ContentPattern = "ContentPattern";

        private static string[] UnknownAttributesIn( XmlNode filterNode )
        {
            var known = new List<string>() { cfa_ID, cfa_Description, cfa_ManualCompletion, cfa_Subpath, cfa_NamePattern, cfa_Language, cfa_ContentPattern };
            var unknown = new List<string>();
            foreach (XmlAttribute attribute in filterNode.Attributes)
            {
                if (!known.Contains( attribute.Name ))
                    unknown.Add( attribute.Name );
            }

            return unknown.ToArray();
        }

        public CompoundFilter CompoundFilter_FromNode( XmlNode filterNode )
        {
            bool isTopLevel = filterNode.LocalName == "Change";
            CompoundFilter filter = isTopLevel ? new Change() : new CompoundFilter();

            var invalidAttributes = UnknownAttributesIn( filterNode );
            if (invalidAttributes.Length > 0)
                throw new Exception( string.Format( "Filters do not have the following attributes: '{0}'.", string.Join( "', '", invalidAttributes ) ) );

            switch (filterNode.Name)
            {
            case "Either":
            case "Or":
                filter.Operator = FilterOperator.Or;
                break;

            case "And":
            case "When":
            case "Change":
                filter.Operator = FilterOperator.And;
                break;

            case "Not":
            case "AndNot":
                filter.Operator = FilterOperator.Not;
                break;

            default:
                throw new Exception( string.Format( "Swept does not know how to create a '{0}' filter.", filterNode.Name ) );
            }

            if (filterNode.Attributes[cfa_ID] != null)
                filter.ID = filterNode.Attributes[cfa_ID].Value;
            else if (isTopLevel)
                throw new Exception( "Changes must have IDs at their top level." );

            if (filterNode.Attributes[cfa_Description] != null)
                filter.Description = filterNode.Attributes[cfa_Description].Value;

            if (filterNode.Attributes[cfa_Subpath] != null)
                filter.Subpath = filterNode.Attributes[cfa_Subpath].Value;

            if (filterNode.Attributes[cfa_NamePattern] != null)
                filter.NamePattern = filterNode.Attributes[cfa_NamePattern].Value;

            if (filterNode.Attributes[cfa_Language] != null)
            {
                string languageText = filterNode.Attributes[cfa_Language].Value;
                try
                {
                    filter.Language = (FileLanguage)Enum.Parse( typeof( FileLanguage ), languageText );
                }
                catch
                {
                    throw new Exception( string.Format( "Don't understand a file of language [{0}].", languageText ) );
                }
            }

            if (filterNode.Attributes[cfa_ContentPattern] != null)
                filter.ContentPattern = filterNode.Attributes[cfa_ContentPattern].Value;

            foreach (XmlNode childNode in filterNode.ChildNodes)
            {
                switch (childNode.Name)
                {
                case "SeeAlso":
                    ((Change)filter).SeeAlsos.Add( SeeAlso_FromNode( childNode ) );
                    break;

                // TODO: become comfortable with this, or change it.
                default:
                    filter.Children.Add( CompoundFilter_FromNode( childNode ) );
                    break;
                }
            }

            return filter;
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
