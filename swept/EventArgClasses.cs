//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using System.Collections.Generic;
using System.Text;
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

    public class TaskEventArgs : EventArgs
    {
        public Task task;
        public SourceFile file;
    }
}
