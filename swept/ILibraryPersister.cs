//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Xml;

namespace swept
{
    public interface ILibraryPersister
    {
        void Save( string fileName, string xmlText );
        XmlDocument LoadLibrary(string libraryPath);
    }
}
