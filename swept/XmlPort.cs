//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Xml;
using System.Text;

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

        public string ToText( CompoundFilter filter )
        {
            filter.markFirstChildren();
            StringBuilder sb = new StringBuilder();
            IntoStringBuilder( filter, sb, 1 );
            return sb.ToString();
        }

        private void IntoStringBuilder( CompoundFilter filter, StringBuilder sb, int depth )
        {

            string indentFormat = "{0," + 4 * depth + "}";
            string indent = string.Format( indentFormat, string.Empty );

            sb.AppendFormat( "{0}<{1}", indent, filter.Name );

            if( !string.IsNullOrEmpty( filter.ID ) )
                sb.AppendFormat( " ID='{0}'", filter.ID );

            if( !string.IsNullOrEmpty( filter.Description ) )
                sb.AppendFormat( " Description='{0}'", filter.Description );

            if( !string.IsNullOrEmpty( filter.ContentPattern ) )
                sb.AppendFormat( " ContentPattern='{0}'", filter.ContentPattern );

            if( !string.IsNullOrEmpty( filter.Subpath ) )
                sb.AppendFormat( " FilePath='{0}'", filter.Subpath );

            if( !string.IsNullOrEmpty( filter.NamePattern ) )
                sb.AppendFormat( " NamePattern='{0}'", filter.NamePattern );

            if( filter.Language != FileLanguage.None )
                sb.AppendFormat( " Language='{0}'", filter.Language );

            bool hasChildren = filter.Children.Count > 0;

            if( !hasChildren )
                sb.Append( " /" );

            sb.AppendFormat( ">{0}", Environment.NewLine );

            if( hasChildren )
            {
                filter.Children.ForEach( child => IntoStringBuilder( child, sb, depth + 1 ) );
                sb.AppendFormat( "{0}</{1}>{2}", indent, filter.Name, Environment.NewLine );
            }
        }

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

        public CompoundFilter CompoundFilter_FromNode( XmlNode filterNode )
        {
            bool isTopLevel = filterNode.LocalName == "Change";
            CompoundFilter filter = isTopLevel ? new Change() : new CompoundFilter();

            if( filterNode.Attributes["ID"] != null )
                filter.ID = filterNode.Attributes["ID"].Value;
            else if( isTopLevel )
                throw new Exception( "Changes must have IDs at their top level." );

            if( filterNode.Attributes["Description"] != null )
                filter.Description = filterNode.Attributes["Description"].Value;

            if( filterNode.Attributes["ContentPattern"] != null )
                filter.ContentPattern = filterNode.Attributes["ContentPattern"].Value;

            if( filterNode.Attributes["FilePattern"] != null )
                filter.NamePattern = filterNode.Attributes["FilePattern"].Value;

            if( filterNode.Attributes["Subpath"] != null )
                filter.Subpath = filterNode.Attributes["Subpath"].Value;

            if( filterNode.Attributes["Language"] != null )
            {
                string languageText = filterNode.Attributes["Language"].Value;
                try
                {
                    filter.Language = (FileLanguage)Enum.Parse( typeof( FileLanguage ), languageText );
                }
                catch
                {
                    throw new Exception( string.Format( "Don't understand a file of language [{0}].", languageText ) );
                }
            }

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
            string changeID = completion.Attributes["ID"].Value;
            Completion fileChange = new Completion( changeID );
            return fileChange;
        }
    }
}
