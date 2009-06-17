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
