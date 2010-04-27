//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

namespace swept
{
    public class StorageAdapter : IStorageAdapter
    {
        static internal string emptyCatalogText = 
@"<SweptProjectData>
<ChangeCatalog>
</ChangeCatalog>
<SourceFileCatalog>
</SourceFileCatalog>
</SweptProjectData>";

        static internal XmlDocument emptyCatalogDoc
        {
            get
            {
                var doc = new XmlDocument();
                doc.LoadXml( emptyCatalogText );
                return doc;
            }
        }

        public void Save(string fileName, string xmlText)
        {
            var doc = new XmlDocument();
            doc.LoadXml( xmlText );
            doc.Save( fileName );
        }

        public XmlDocument LoadLibrary( string libraryPath )
        {
            if (!File.Exists( libraryPath ))
            {
                XmlDocument emptyDoc = new XmlDocument();
                emptyDoc.LoadXml( emptyCatalogText );
                return emptyDoc;
            }

            using (XmlTextReader reader = new XmlTextReader( libraryPath ))
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load( reader );
                    return doc;
                }
                catch (XmlException xe)
                {
                    string errInvalidXml = "File [{0}] was not valid XML.  Please check its contents.\n  Details: {1}";
                    throw new Exception( string.Format( errInvalidXml, libraryPath, xe.Message ) );
                }
            }
        }

        public string GetCWD()
        {
            return Environment.CurrentDirectory;
        }

        public IEnumerable<string> GetFilesInFolder( string folder )
        {
            return Directory.GetFiles( folder );
        }

        public IEnumerable<string> GetFoldersInFolder( string folder )
        {
            return Directory.GetDirectories( folder );
        }
        
        // future:  Pick another location if not found, store that location...somewhere?

        public void RenameLibrary( string oldPath, string newPath )
        {
            throw new NotImplementedException();
        }

        public SourceFile LoadFile( string fileName )
        {
            try
            {
                SourceFile file = new SourceFile( fileName );
                file.Content = File.ReadAllText( fileName );
                return file;
            }
            catch
            {
                Console.Out.WriteLine( String.Format( "LoadFile( {0} ) failed.", fileName ) );
                throw;
            }
        }
    }
}
