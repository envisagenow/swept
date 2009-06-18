//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using System;
using System.Collections.Generic;
using System.Text;

namespace swept.Tests
{
    class MockLibraryWriter : ILibraryWriter
    {
        public string FileName { get; set; }
        public string XmlText { get; private set; }

        public void Save(string fileName, string xmlText)
        {
            FileName = fileName;
            XmlText = xmlText;
        }
    }
}
