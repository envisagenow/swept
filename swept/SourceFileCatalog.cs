//  Swept:  Software Enhancement Progress Tracking.  Copyright 2008 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
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

        public SourceFileCatalog( SourceFileCatalog fileCat )
        {
            ChangeCatalog = fileCat.ChangeCatalog;

            Files = new List<SourceFile>();
            foreach( SourceFile file in fileCat.Files )
            {
                SourceFile newSourceFile = new SourceFile( file.Name );
                newSourceFile.CopyCompletionsFrom( file );
                Files.Add( newSourceFile );
            }
        }

        public void Add( SourceFile file )
        {
            if( !Files.Contains( file ) )
                Files.Add( file );
        }

        public void Delete( string fileName )
        {
            SourceFile file = Find( fileName );
            if( file == null ) return;
            Files.Remove( file );
        }

        public void Rename(string oldName, string newName)
        {
            SourceFile file = FetchFile(oldName);
            file.Name = newName;
        }

        private SourceFile Find( string name )
        {
            return Files.Find( f => f.Name == name );
        }

        internal SourceFile FetchFile( string name )
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

        public string ToXmlText()
        {
            string catalogLabel = "SourceFileCatalog";
            string xmlText = String.Format( "<{0}>\r\n", catalogLabel );
            foreach( SourceFile source in Files )
            {
                xmlText += source.ToXmlText();
            }
            xmlText += String.Format( "</{0}>\r\n", catalogLabel );
            return xmlText;
        }

        #endregion
    }
}
