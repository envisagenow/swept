//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2011 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Xml.Linq;

namespace swept
{
    [CoverageExclude( "Wrapper around file system" )]
    public class StorageAdapter : IStorageAdapter
    {
        static internal string emptyCatalogText = 
@"<SweptProjectData>
<RuleCatalog>
</RuleCatalog>
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

        public IEnumerable<string> GetFilesInFolder( string folder, string searchPattern )
        {
            return Directory.GetFiles( folder, searchPattern );
        }

        public IEnumerable<string> GetFoldersInFolder( string folder )
        {
            return Directory.GetDirectories( folder );
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

        public void SaveRunHistory( XDocument runHistory )
        {
            throw new NotImplementedException();
        }

        public XDocument LoadRunHistory()
        {
            throw new NotImplementedException();
        }
    }
}
