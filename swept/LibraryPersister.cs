//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.IO;
using System.Xml;

namespace swept
{
    public class LibraryPersister : ILibraryPersister
    {
        static internal string _emptyCatalogString = 
@"<SweptProjectData>
<ChangeCatalog>
</ChangeCatalog>
<SourceFileCatalog>
</SourceFileCatalog>
</SweptProjectData>";

        public void Save(string fileName, string xmlText)
        {
            //throw new NotImplementedException("I can't save to disk yet");

            //FileInfo fi = new FileInfo( fileName );
            //TextWriter writer = fi.CreateText();
        }

        public XmlDocument LoadLibrary( string libraryPath )
        {
            //throw new NotImplementedException( "I don't load from disk yet" );

            if (!File.Exists( libraryPath ))
            {
                XmlDocument emptyDoc = new XmlDocument();
                emptyDoc.LoadXml( _emptyCatalogString );
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
                    string errInvalidXml = "File [{0}] was not valid XML.  Please check its contents.  Details: {1}";
                    throw new Exception( string.Format( errInvalidXml, libraryPath, xe.Message ) );
                }
            }
        }
        // future:  Pick another location if not found, store that location...somewhere?

    }
}
