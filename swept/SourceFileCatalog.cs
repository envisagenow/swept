//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System;

namespace swept
{
    public class SourceFileCatalog
    {
        internal List<SourceFile> Files;
        public ChangeCatalog ChangeCatalog;

        public SourceFileCatalog()
        {
            Files = new List<SourceFile>();
        }

        public static SourceFileCatalog Clone( SourceFileCatalog parent )
        {
            SourceFileCatalog newCatalog = new SourceFileCatalog();

            newCatalog.ChangeCatalog = parent.ChangeCatalog;
            foreach (SourceFile file in parent.Files)
            {
                SourceFile newSourceFile = new SourceFile(file.Name);
                newSourceFile.CopyCompletionsFrom(file);
                newCatalog.Files.Add(newSourceFile);
            }

            return newCatalog;
        }

        public void Add(SourceFile file)
        {
            if (Files.Exists(sf => sf.Name == file.Name))
                return;
            Files.Add(file);
        }

        public void Delete( string fileName )
        {
            SourceFile file = Find( fileName );
            if( file == null ) return;
            Files.Remove( file );
        }

        public void Rename(string oldName, string newName)
        {
            SourceFile file = Fetch(oldName);
            file.Name = newName;
        }

        private SourceFile Find( string name )
        {
            return Files.Find( f => f.Name == name );
        }

        internal SourceFile Fetch( string name )
        {
            SourceFile foundFile = Find( name );

            if( foundFile == null )
            {
                foundFile = new SourceFile( name );
                Files.Add( foundFile );
            }

            return foundFile;
        }


        #region Serialization
        /// <summary>Will return a new instance from the proper XML text</summary>
        public static SourceFileCatalog FromXmlText( string xmlText )
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml( xmlText );
                return FromXmlDocument( doc );
            }
            catch( XmlException xe )
            {
                throw new Exception( String.Format( "Text [{0}] was not valid XML.  Please check its contents.  Details: {1}", xmlText, xe.Message ) );
            }
        }

        public static SourceFileCatalog FromXmlDocument( XmlDocument doc )
        {
            XmlNode root = doc.SelectSingleNode( "SourceFileCatalog" );
            if( root == null )
                throw new Exception( "Document must have a <SourceFileCatalog> root node.  Please supply one." );

            SourceFileCatalog cat = new SourceFileCatalog();

            XmlNodeList files = root.SelectNodes( "SourceFile" );
            foreach( XmlNode fileNode in files )
            {
                SourceFile file = SourceFile.FromNode( fileNode );

                cat.Files.Add( file );
            }

            return cat;
        }

        //FUTURE: Sort SourceFiles when writing them out
        public string ToXmlText()
        {
            string catalogLabel = "SourceFileCatalog";
            string xmlText = String.Format( "<{0}>\r\n", catalogLabel );
            foreach( SourceFile source in Files )
            {
                xmlText += source.ToXmlText();
            }
            xmlText += String.Format( "</{0}>", catalogLabel );
            return xmlText;
        }

        #endregion
    }
}
