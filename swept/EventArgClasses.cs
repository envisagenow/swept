//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System.Collections.Generic;
using System;

namespace swept
{
    public class FileEventArgs : EventArgs
    {
        public string Name;
        public string Content;
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
