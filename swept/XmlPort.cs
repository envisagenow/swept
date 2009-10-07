//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using System;
using System.Collections.Generic;
using System.Xml;

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
            foreach (KeyValuePair<string, Change> pair in changeCatalog._changes)
            {
                xmlText += ToText( pair.Value );
            }
            xmlText += String.Format("</{0}>", catalogLabel);
            return xmlText;
        }

        public string ToText(Change change)
        {
            return String.Format(
                "    <Change ID='{0}' Description='{1}' Language='{2}' />" + Environment.NewLine,
                change.ID, change.Description, change.Language
            );
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

        private static Change Change_FromNode(XmlNode xmlNode)
        {
            FileLanguage lang = (FileLanguage)Enum.Parse(typeof(FileLanguage), xmlNode.Attributes["Language"].Value);

            Change change = new Change(xmlNode.Attributes["ID"].Value, xmlNode.Attributes["Description"].Value, lang);

            return change;
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
