//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2011 Jason Cole and Envisage Technologies Corp.
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
        public Change Change;
    }

    public class ChangeCatalogEventArgs : EventArgs
    {
        public ChangeCatalog Catalog;
    }

    public class SeeAlsoEventArgs : EventArgs
    {
        public SeeAlso SeeAlso;
    }

    public class ExceptionEventArgs : EventArgs
    {
        public Exception Exception;
    }

    public class TaskEventArgs : EventArgs
    {
        public Task Task;
        public SourceFile File;
        public bool Checked;
    }

    public class TasksEventArgs : EventArgs
    {
        public List<Task> Tasks;
    }
}
