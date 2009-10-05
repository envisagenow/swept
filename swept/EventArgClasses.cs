//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using System.Collections.Generic;
using System;

namespace swept
{
    public class FileEventArgs : EventArgs
    {
        public string Name;
    }

    public class FileListEventArgs : EventArgs
    {
        public List<string> Names;
    }

    public class ChangeEventArgs : EventArgs
    {
        public Change change;
    }

    public class ChangeCatalogEventArgs : EventArgs
    {
        public ChangeCatalog Catalog;
    }

    public class SourceCatalogEventArgs : EventArgs
    {
        public SourceFileCatalog Catalog;
    }

    public class TaskEventArgs : EventArgs
    {
        public Task Task;
        public SourceFile File;
        public bool Checked;
    }
}
