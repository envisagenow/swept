//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009, 2012 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Xml;
using System.Collections.Generic;
using System.Xml.Linq;

namespace swept
{
    public interface IStorageAdapter
    {
        XmlDocument LoadLibrary(string libraryPath);
        string GetCWD();

        IEnumerable<string> GetFilesInFolder( string folder );
        IEnumerable<string> GetFilesInFolder( string folder, string searchPattern );
        IEnumerable<string> GetFoldersInFolder( string folder );

        SourceFile LoadFile( string fileName );

        //XDocument LoadHistory( string historyPath );
        //void SaveHistory( XDocument runHistory );
    }
}
