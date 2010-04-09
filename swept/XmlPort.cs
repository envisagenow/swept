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
        XmlPort_CompoundFilter _filterPort;

        public XmlPort()
        {
            _filterPort = new XmlPort_CompoundFilter();
        }

        public string ToText( ChangeCatalog changeCatalog, SourceFileCatalog sourceCatalog )
        {
            return string.Format(
@"<SweptProjectData>
{0}
{1}
</SweptProjectData>",
                ToText( changeCatalog ),
                ToText( sourceCatalog )
            );
        }

        public string ToText( ChangeCatalog changeCatalog )
        {
            string catalogLabel = "ChangeCatalog";
            string xmlText = String.Format( "<{0}>\r\n", catalogLabel );

            changeCatalog._changes.Sort( ( a, b ) => a.ID.CompareTo(b.ID) );

            foreach (Change change in changeCatalog._changes)
            {
                xmlText += _filterPort.ToText( change );
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
            xmlText += String.Format( "    </{0}>\r\n", elementLabel );

            return xmlText;
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
                Change change = (Change)_filterPort.CompoundFilter_FromNode( changeNode );

                cat.Add( change );
            }

            return cat;
        }

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
            return file;
        }
    }
}
