//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Xml;

namespace swept.Tests
{
    class MockStorageAdapter : IStorageAdapter
    {
        public string FileName { get; set; }
        public XmlDocument LibraryDoc { get; set; }
        public bool ThrowBadXmlException;

        public MockStorageAdapter()
        {
            setDocFromText( StorageAdapter.emptyCatalogText );
        }

        public void Save(string fileName, string xmlText)
        {
            FileName = fileName;
            setDocFromText( xmlText );
        }

        private void setDocFromText( string xmlText )
        {
            LibraryDoc = new XmlDocument();
            LibraryDoc.LoadXml( xmlText );
        }
        
        public XmlDocument LoadLibrary( string libraryPath )
        {
            FileName = libraryPath;
            if( ThrowBadXmlException )
            {
                ThrowBadXmlException = false;
                throw new XmlException();
            }

            return LibraryDoc;
        }

        public string renamedOldLibraryPath;
        public string renamedNewLibraryPath;
        public void RenameLibrary( string oldPath, string newPath )
        {
            renamedNewLibraryPath = newPath;
            renamedOldLibraryPath = oldPath;
        }
    }
}
