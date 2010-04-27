//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Xml;
using System.Collections.Generic;

namespace swept.Tests
{
    class MockStorageAdapter : IStorageAdapter
    {
        private List<string> _loadedFiles;
        public string FileName { get; set; }
        public XmlDocument LibraryDoc { get; set; }
        public Dictionary<string, List<string>> FilesInFolder { get; set; }
        public Dictionary<string, List<string>> FoldersInFolder { get; set; }
        public string CWD = String.Empty;
        public bool ThrowBadXmlException;

        public MockStorageAdapter()
        {
            setDocFromText( StorageAdapter.emptyCatalogText );
            FilesInFolder = new Dictionary<string, List<string>>();
            FoldersInFolder = new Dictionary<string, List<string>>();
            _loadedFiles = new List<string>();
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

        public string GetCWD()
        {
            return CWD;
        }

        public IEnumerable<string> GetFilesInFolder( string folder )
        {
            if (FilesInFolder.ContainsKey( folder ))
                return FilesInFolder[folder];

            return new List<string>();
        }

       public IEnumerable<string> GetFoldersInFolder( string folder )
        {
            if (FoldersInFolder.ContainsKey( folder ))
                return FoldersInFolder[folder];

            return new List<string>();
        }


       public SourceFile LoadFile( string fileName )
       {
           if (!_loadedFiles.Contains(fileName))
           {
               _loadedFiles.Add( fileName );
           }

           return new SourceFile( fileName );
       }


       internal bool DidLoad( string fileName )
       {
           return _loadedFiles.Contains( fileName );
       }
    }
}
