//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2015 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;

namespace swept
{
    public interface IStorageAdapter
    {
        XDocument LoadLibrary(string libraryPath);
        SourceFile LoadFile(string folder, string fileName);
        XDocument LoadChangeSet(string changeSetPath);
        XDocument LoadRunHistory(string historyPath);
        XDocument LoadRunChanges(string changesPath);
        void SaveRunChanges(XDocument runChanges, string fileName);
        void SaveRunHistory(XDocument runHistory, string fileName);

        string GetCWD();

        IEnumerable<string> GetFilesInFolder(string folder);
        IEnumerable<string> GetFilesInFolder(string folder, string searchPattern);
        IEnumerable<string> GetFoldersInFolder(string folder);

        TextWriter GetOutputWriter(string outputLocation);
    }
}
