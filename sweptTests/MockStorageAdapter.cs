//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
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
    }
}
