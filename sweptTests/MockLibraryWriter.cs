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
