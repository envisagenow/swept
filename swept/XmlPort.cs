//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;

namespace swept
{
    internal class XmlPort
    {
        public string ToText(ProjectLibrarian librarian)
        {
            return string.Format(
@"<SweptProjectData>
{0}
{1}
</SweptProjectData>",
                ToText( librarian._changeCatalog ),
                ToText(librarian._savedSourceCatalog)
            );
        }

        public string ToText(ChangeCatalog changeCatalog)
        {
            string catalogLabel = "ChangeCatalog";
            string xmlText = String.Format("<{0}>\r\n", catalogLabel);
            foreach ( Change change in changeCatalog._changes)
            {
                xmlText += ToText( change );
            }
            xmlText += String.Format("</{0}>", catalogLabel);
            return xmlText;
        }

        public string ToText( Change change )
        {
            var sb = new StringBuilder( "    <Change " );
            sb.AppendFormat( "ID='{0}' Description='{1}' Language='{2}' ",
                change.ID, change.Description, change.Language );

            if( !string.IsNullOrEmpty( change.Subpath ) )
                sb.AppendFormat( "Subpath='{0}' ", change.Subpath );
            
            if( !string.IsNullOrEmpty( change.NamePattern ) )
                sb.AppendFormat( "NamePattern='{0}' ", change.NamePattern );
            
            sb.AppendLine( "/>" );

            return sb.ToString();
        }

        public string ToText(SourceFileCatalog fileCatalog)
        {
            string catalogLabel = "SourceFileCatalog";
            string xmlText = String.Format("<{0}>\r\n", catalogLabel);
            fileCatalog.Files.Sort( (left, right) => left.Name.CompareTo( right.Name ) );
            fileCatalog.Files.ForEach( file => xmlText += ToText( file ) );
            xmlText += String.Format( "</{0}>", catalogLabel );
            return xmlText;
        }

        public string ToText(SourceFile file)
        {
            string elementLabel = "SourceFile";
            string xmlText = String.Format("    <{0} Name='{1}'>\r\n", elementLabel, file.Name);
            file.Completions.Sort( (left, right) => left.ChangeID.CompareTo(right.ChangeID) );
            file.Completions.ForEach( c => xmlText += ToText( c ) );
            xmlText += String.Format( "    </{0}>\r\n", elementLabel );

            return xmlText;
        }

        public string ToText(Completion comp)
        {
            return string.Format("        <Completion ID='{0}' />\r\n", comp.ChangeID);
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

            XmlNodeList changes = node.SelectNodes("Change");
            foreach (XmlNode changeNode in changes)
            {
                Change change = Change_FromNode(changeNode);

                cat.Add(change);
            }

            return cat;
        }

        private static Change Change_FromNode( XmlNode xmlNode )
        {
            FileLanguage language = (FileLanguage)Enum.Parse( typeof( FileLanguage ), xmlNode.Attributes["Language"].Value );

            string subpath = (xmlNode.Attributes["Subpath"] != null) ? xmlNode.Attributes["Subpath"].Value : string.Empty;
            string pattern = (xmlNode.Attributes["NamePattern"] != null) ? xmlNode.Attributes["NamePattern"].Value : string.Empty;
            return new Change
            {
                ID = xmlNode.Attributes["ID"].Value,
                Description = xmlNode.Attributes["Description"].Value,
                Language = language,
                Subpath = subpath,
                NamePattern = pattern,
            };
        }


        public SourceFileCatalog SourceFileCatalog_FromText(string xmlText)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlText);
                return SourceFileCatalog_FromXmlDocument(doc);
            }
            catch (XmlException xe)
            {
                throw new Exception(String.Format("Text [{0}] was not valid XML.  Please check its contents.  Details: {1}", xmlText, xe.Message));
            }
        }

        public SourceFileCatalog SourceFileCatalog_FromXmlDocument(XmlDocument doc)
        {
            XmlNode node = doc.SelectSingleNode("SweptProjectData/SourceFileCatalog");
            if (node == null)
                throw new Exception("Document must have a <SourceFileCatalog> node.  Please supply one.");

            return SourceFileCatalog_FromNode(node);
        }


        public SourceFileCatalog SourceFileCatalog_FromNode(XmlNode node)
        {
            SourceFileCatalog cat = new SourceFileCatalog();

            XmlNodeList files = node.SelectNodes("SourceFile");
            foreach (XmlNode fileNode in files)
            {
                cat.Files.Add(SourceFile_FromNode(fileNode));
            }

            return cat;
        } 

        private SourceFile SourceFile_FromNode(XmlNode xmlNode)
        {
            if (xmlNode.Attributes["Name"] == null)
                throw new Exception("A SourceFile node must have a Name attribute.  Please add one.");

            SourceFile file = new SourceFile(xmlNode.Attributes["Name"].Value);
            foreach (XmlNode completionNode in xmlNode.SelectNodes("Completion"))
            {
                Completion comp = Completion_FromNode(completionNode);
                file.Completions.Add(comp);
            }
            return file;
        }

        public Completion Completion_FromNode(XmlNode completion)
        {
            string changeID = completion.Attributes["ID"].Value;
            Completion fileChange = new Completion(changeID);
            return fileChange;
        }

    }
}
