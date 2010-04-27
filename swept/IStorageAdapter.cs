//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Xml;
using System.Collections.Generic;

namespace swept
{
    public interface IStorageAdapter
    {
        void Save( string fileName, string xmlText );
        XmlDocument LoadLibrary(string libraryPath);
        void RenameLibrary( string oldPath, string newPath );
        string GetCWD();
        IEnumerable<string> GetFilesInFolder( string folder );

        IEnumerable<string> GetFoldersInFolder( string p );
        SourceFile LoadFile( string fileName );
    }
}
