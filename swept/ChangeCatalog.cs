//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace swept
{
    public class ChangeCatalog
    {
        public bool IsDirty { get; private set; }
        internal List<Change> changes;

        public ChangeCatalog()
        {
            changes = new List<Change>();
        }


        public List<Change> GetListForLanguage( FileLanguage fileLanguage )
        {
            return changes.FindAll( c => c.Language == fileLanguage );
        }

        public List<Change> GetChangesForFile(SourceFile file)
        {
            return GetListForLanguage(file.Language);
        }

        public void Add(Change change)
        {
            IsDirty = true;
            changes.Add(change);
        }

        public void Remove(string changeID)
        {
            IsDirty = true;
            changes.RemoveAll(c => c.ID == changeID);
        }

        public List<Change> FindAll(Predicate<Change> match)
        {
            return changes.FindAll(match);
        }

        public void MarkClean()
        {
            IsDirty = false;
        }

        //FUTURE: Sort changes when writing them out
        public string ToXmlText()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<ChangeCatalog>");

            foreach(Change change in this.changes)
            {
                sb.AppendLine(change.ToXmlText());
            }

            sb.Append("</ChangeCatalog>");
            return sb.ToString();
        }

        /// <summary>Will return a new instance from the proper XML text</summary>
        public static ChangeCatalog FromXmlText(string xmlText)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlText);
                return FromXmlDocument(doc);
            }
            catch (XmlException xe)
            {
                throw new Exception(String.Format("Text [{0}] was not valid XML.  Please check its contents.  Details: {1}", xmlText, xe.Message));
            }
        }

        public static ChangeCatalog FromXmlDocument(XmlDocument doc)
        {
            XmlNode node = doc.SelectSingleNode("SweptProjectData/ChangeCatalog");
            if (node == null)
                throw new Exception("Document must have a <ChangeCatalog> node.  Please supply one.");

            ChangeCatalog cat = new ChangeCatalog();

            XmlNodeList changes = node.SelectNodes("Change");
            foreach (XmlNode changeNode in changes)
            {
                Change change = Change.FromNode(changeNode);

                cat.changes.Add(change);
            }

            return cat;
        }

    }
}
