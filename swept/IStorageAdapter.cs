//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Xml;

namespace swept
{
    public interface IStorageAdapter
    {
        void Save( string fileName, string xmlText );
        XmlDocument LoadLibrary(string libraryPath);
    }
}
