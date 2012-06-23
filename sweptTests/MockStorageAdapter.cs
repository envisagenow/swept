//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Xml;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace swept.Tests
{
    class MockStorageAdapter : IStorageAdapter
    {
        private List<string> _loadedFiles;
        public string FileName { get; set; }
        public XmlDocument LibraryDoc { get; set; }
        public Dictionary<string, List<string>> FilesInFolder { get; set; }
        public Dictionary<string, List<string>> FoldersInFolder { get; set; }
        public XDocument RunHistory { get; set; }
        public string CWD = String.Empty;
        public bool ThrowBadXmlException;

        public MockStorageAdapter()
        {
            setDocFromText( StorageAdapter.emptyCatalogText );
            FilesInFolder = new Dictionary<string, List<string>>();
            FoldersInFolder = new Dictionary<string, List<string>>();
            _loadedFiles = new List<string>();
        }

        private void setDocFromText( string xmlText )
        {
            LibraryDoc = new XmlDocument();
            LibraryDoc.LoadXml( xmlText );
        }

        private IOException _ll_ioex;
        public void LoadLibrary_Throw( IOException ex )
        {
            _ll_ioex = ex;
        }

        public XmlDocument LoadLibrary( string libraryPath )
        {
            if (_ll_ioex != null)
                throw _ll_ioex;

            FileName = libraryPath;
            if (ThrowBadXmlException)
            {
                ThrowBadXmlException = false;
                throw new XmlException();
            }

            return LibraryDoc;
        }

        public string GetCWD()
        {
            return CWD;
        }

        private IOException _gfif_ioex;
        public void GetFilesInFolder_Throw( IOException ex )
        {
            _gfif_ioex = ex;
        }

        public IEnumerable<string> GetFilesInFolder( string folder )
        {
            if (_gfif_ioex != null)
                throw _gfif_ioex;

            if (FilesInFolder.ContainsKey( folder ))
                return FilesInFolder[folder];

            return new List<string>();
        }

        public IEnumerable<string> GetFilesInFolder( string folder, string searchPattern )
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
            if (!_loadedFiles.Contains( fileName ))
            {
                _loadedFiles.Add( fileName );
            }

            return new SourceFile( fileName );
        }


        internal bool DidLoad( string fileName )
        {
            return _loadedFiles.Contains( fileName );
        }

        public void SaveRunHistory( XDocument runHistory )
        {
            RunHistory = runHistory;
        }

        public Exception RunHistoryNotFoundException { get; set; }
        public XDocument LoadRunHistory()
        {
            if (RunHistoryNotFoundException != null)
                throw RunHistoryNotFoundException;
            return RunHistory;
        }
    }
}
