//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2015 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Xml;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using System.Text.RegularExpressions;

namespace swept.Tests
{
    class MockStorageAdapter : IStorageAdapter
    {
        public MockStorageAdapter()
        {
            setDocFromText(StorageAdapter.emptyCatalogText);
            FilesInFolder = new Dictionary<string, List<string>>();
            FoldersInFolder = new Dictionary<string, List<string>>();
            _loadedFiles = new List<string>();
            FilesToFailToLoad = new List<string>();
        }


        private List<string> _loadedFiles;
        public string FileName { get; set; }
        public XDocument ChangeDoc { get; set; }
        public XDocument LibraryDoc { get; set; }
        public Dictionary<string, List<string>> FilesInFolder { get; set; }
        public Dictionary<string, List<string>> FoldersInFolder { get; set; }
        public List<string> FilesToFailToLoad { get; set; }
        public Object LoadRunChangesOutcome { get; set; }
        public XDocument RunHistory { get; set; }
        public XDocument RunChanges { get; set; }
        public string CWD = String.Empty;
        public bool ThrowBadXmlException;

        public TextWriter GetOutputWriter(string output)
        {
            throw new NotImplementedException();
        }

        private void setDocFromText(string xmlText)
        {
            LibraryDoc = XDocument.Parse(xmlText);
        }

        private IOException _ll_ioex;
        public void LoadLibrary_Throw(IOException ex)
        {
            _ll_ioex = ex;
        }

        public XDocument LoadLibrary(string libraryPath)
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
        public void GetFilesInFolder_Throw(IOException ex)
        {
            _gfif_ioex = ex;
        }

        public IEnumerable<string> GetFilesInFolder(string folder)
        {
            if (_gfif_ioex != null)
                throw _gfif_ioex;

            foreach (var key in FilesInFolder.Keys)
            {
                if (key.Equals(folder, StringComparison.InvariantCultureIgnoreCase))
                    return FilesInFolder[key];
            }
            return new List<string>();
        }

        public IEnumerable<string> GetFilesInFolder(string folder, string searchPattern)
        {
            //  approximate and good enough for our tests.
            var regex = searchPattern.Replace("*", ".*");

            if (FilesInFolder.ContainsKey(folder))
                return FilesInFolder[folder].Where(f => Regex.IsMatch(f, regex, RegexOptions.IgnoreCase));

            return new List<string>();
        }

        public IEnumerable<string> GetFoldersInFolder(string folder)
        {
            foreach (var key in FoldersInFolder.Keys)
            {
                if (key.Equals(folder, StringComparison.InvariantCultureIgnoreCase))
                    return FoldersInFolder[key];
            }

            return new List<string>();
        }

        public SourceFile LoadFile(string fileName)
        {
            if (!_loadedFiles.Contains(fileName))
            {
                _loadedFiles.Add(fileName);
            }

            if (FilesToFailToLoad.Contains(fileName)) return null;
            return new SourceFile(fileName);
        }


        internal bool DidLoad(string fileName)
        {
            return _loadedFiles.Contains(fileName);
        }

        public string SavedHistoryFileName;
        public void SaveRunHistory(XDocument runHistory, string fileName)
        {
            RunHistory = runHistory;
            SavedHistoryFileName = fileName;
        }

        public Exception RunHistoryNotFoundException { get; set; }
        public XDocument LoadRunHistory(string historyPath)
        {
            if (RunHistoryNotFoundException != null)
                throw RunHistoryNotFoundException;
            return RunHistory;
        }

        public Exception ChangeSetNotFoundException { get; set; }
        public XDocument LoadChangeSet(string changeSetPath)
        {
            if (ChangeSetNotFoundException != null)
                throw ChangeSetNotFoundException;
            return ChangeDoc;
        }

        public string SavedChangesFileName;
        public void SaveRunChanges(XDocument runChanges, string fileName)
        {
            RunChanges = runChanges;
            SavedChangesFileName = fileName;
        }

        public XDocument LoadRunChanges(string changesFilename)
        {
            if (LoadRunChangesOutcome == null)
                throw new FileNotFoundException(changesFilename);

            if (LoadRunChangesOutcome is Exception)
                throw (Exception)LoadRunChangesOutcome;

            return (XDocument)LoadRunChangesOutcome;
        }

    }
}
