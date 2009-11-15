//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Xml;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace swept
{
    internal class XmlPort
    {
        public string ToText( ProjectLibrarian librarian )
        {
            return string.Format(
@"<SweptProjectData>
{0}
{1}
</SweptProjectData>",
                ToText( librarian._changeCatalog ),
                ToText( librarian._savedSourceCatalog )
            );
        }

        public string ToText( ChangeCatalog changeCatalog )
        {
            string catalogLabel = "ChangeCatalog";
            string xmlText = String.Format( "<{0}>\r\n", catalogLabel );
            foreach( Change change in changeCatalog._changes )
            {
                xmlText += ToText( change );
            }
            xmlText += String.Format( "</{0}>", catalogLabel );
            return xmlText;
        }

        public string ToText( SourceFileCatalog fileCatalog )
        {
            string catalogLabel = "SourceFileCatalog";
            string xmlText = String.Format( "<{0}>\r\n", catalogLabel );
            fileCatalog.Files.Sort( ( left, right ) => left.Name.CompareTo( right.Name ) );
            fileCatalog.Files.ForEach( file => xmlText += ToText( file ) );
            xmlText += String.Format( "</{0}>", catalogLabel );
            return xmlText;
        }

        public string ToText( SourceFile file )
        {
            string elementLabel = "SourceFile";
            string xmlText = String.Format( "    <{0} Name='{1}'>\r\n", elementLabel, file.Name );
            file.Completions.Sort( ( left, right ) => left.ChangeID.CompareTo( right.ChangeID ) );
            file.Completions.ForEach( c => xmlText += ToText( c ) );
            xmlText += String.Format( "    </{0}>\r\n", elementLabel );

            return xmlText;
        }

        public string ToText( Completion comp )
        {
            return string.Format( "        <Completion ID='{0}' />\r\n", comp.ChangeID );
        }

        #region Compound Filters
        private const string cfa_ID = "ID";
        private const string cfa_Description = "Description";
        private const string cfa_ManualCompletion = "ManualCompletion";
        private const string cfa_Subpath = "Subpath";
        private const string cfa_NamePattern = "NamePattern";
        private const string cfa_Language = "Language";
        private const string cfa_ContentPattern = "ContentPattern";
        
        public string ToText( CompoundFilter filter )
        {
            filter.markFirstChildren();
            StringBuilder sb = new StringBuilder();
            IntoStringBuilder( filter, sb, 1 );
            return sb.ToString();
        }

        private static void AppendAttributeIfExists( StringBuilder sb, string name, string value )
        {
            if (string.IsNullOrEmpty( value )) return;
            sb.AppendFormat( " {0}='{1}'", name, value );
        }
        private void IntoStringBuilder( CompoundFilter filter, StringBuilder sb, int depth )
        {
            string indentFormat = "{0," + 4 * depth + "}";
            string indent = string.Format( indentFormat, string.Empty );

            sb.AppendFormat( "{0}<{1}", indent, filter.Name );

            AppendAttributeIfExists( sb, cfa_ID, filter.ID );
            AppendAttributeIfExists( sb, cfa_Description,        filter.Description );
            AppendAttributeIfExists( sb, cfa_ManualCompletion, filter.ManualCompletionString );
            AppendAttributeIfExists( sb, cfa_Subpath,            filter.Subpath );
            AppendAttributeIfExists( sb, cfa_NamePattern,        filter.NamePattern );
            AppendAttributeIfExists( sb, cfa_Language,           filter.LanguageString );
            AppendAttributeIfExists( sb, cfa_ContentPattern,     filter.ContentPattern );

            bool hasChildren = filter.Children.Count > 0;

            if (!hasChildren)
                sb.Append( " /" );

            sb.AppendFormat( ">{0}", Environment.NewLine );

            if (hasChildren)
            {
                filter.Children.ForEach( child => IntoStringBuilder( child, sb, depth + 1 ) );
                sb.AppendFormat( "{0}</{1}>{2}", indent, filter.Name, Environment.NewLine );
            }
        }
        #endregion

        public ChangeCatalog ChangeCatalog_FromXmlDocument( XmlDocument doc )
        {
            XmlNode node = doc.SelectSingleNode( "SweptProjectData/ChangeCatalog" );

            if( node == null )
                throw new Exception( "Document must have a <ChangeCatalog> node.  Please supply one." );

            return ChangeCatalog_FromNode( node );
        }


        public ChangeCatalog ChangeCatalog_FromNode( XmlNode node )
        {
            ChangeCatalog cat = new ChangeCatalog();

            XmlNodeList changes = node.SelectNodes( "Change" );
            foreach( XmlNode changeNode in changes )
            {
                Change change = (Change)CompoundFilter_FromNode( changeNode );

                cat.Add( change );
            }

            return cat;
        }

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

            if( filterNode.Attributes[cfa_ID] != null )
                filter.ID = filterNode.Attributes[cfa_ID].Value;
            else if( isTopLevel )
                throw new Exception( "Changes must have IDs at their top level." );

            if( filterNode.Attributes[cfa_Description] != null )
                filter.Description = filterNode.Attributes[cfa_Description].Value;

            if (filterNode.Attributes[cfa_ManualCompletion] != null)
            {
                if (filterNode.Attributes[cfa_ManualCompletion].Value != "Allowed")
                    throw new Exception( String.Format( "Don't understand the manual completion permission '{0}'.", filterNode.Attributes[cfa_ManualCompletion].Value ) );
                filter.ManualCompletion = true;
            }

            if (filterNode.Attributes[cfa_Subpath] != null)
                filter.Subpath = filterNode.Attributes[cfa_Subpath].Value;

            if (filterNode.Attributes[cfa_NamePattern] != null)
                filter.NamePattern = filterNode.Attributes[cfa_NamePattern].Value;

            if( filterNode.Attributes[cfa_Language] != null )
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

            foreach( XmlNode childNode in filterNode.ChildNodes )
            {
                filter.Children.Add( CompoundFilter_FromNode( childNode ) );
            }

            return filter;
        }
        // TODO: cleanup: Align the domain values with the xml attributes.


        public SourceFileCatalog SourceFileCatalog_FromText(string xmlText)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml( xmlText );
                return SourceFileCatalog_FromXmlDocument( doc );
            }
            catch (XmlException xe)
            {
                throw new Exception( String.Format( "Text [{0}] was not valid XML.  Please check its contents.  Details: {1}", xmlText, xe.Message ) );
            }
        }

        public SourceFileCatalog SourceFileCatalog_FromXmlDocument( XmlDocument doc )
        {
            XmlNode node = doc.SelectSingleNode( "SweptProjectData/SourceFileCatalog" );
            if (node == null)
                throw new Exception( "Document must have a <SourceFileCatalog> node.  Please supply one." );

            return SourceFileCatalog_FromNode( node );
        }


        public SourceFileCatalog SourceFileCatalog_FromNode( XmlNode node )
        {
            SourceFileCatalog cat = new SourceFileCatalog();

            XmlNodeList files = node.SelectNodes( "SourceFile" );
            foreach (XmlNode fileNode in files)
            {
                cat.Files.Add( SourceFile_FromNode( fileNode ) );
            }

            return cat;
        }

        private SourceFile SourceFile_FromNode( XmlNode xmlNode )
        {
            if (xmlNode.Attributes["Name"] == null)
                throw new Exception( "A SourceFile node must have a Name attribute.  Please add one." );

            SourceFile file = new SourceFile( xmlNode.Attributes["Name"].Value );
            foreach (XmlNode completionNode in xmlNode.SelectNodes( "Completion" ))
            {
                Completion comp = Completion_FromNode( completionNode );
                file.Completions.Add( comp );
            }
            return file;
        }

        public Completion Completion_FromNode( XmlNode completion )
        {
            string changeID = completion.Attributes[cfa_ID].Value;
            Completion fileChange = new Completion( changeID );
            return fileChange;
        }
    }
}
