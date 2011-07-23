//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System.Collections.Generic;
using System;

namespace swept
{
    public class StudioAdapter
    {
        internal TaskWindow taskWindow;
        public ProjectLibrarian Librarian { get; set; }


        public void Hear_TaskLocationSought( object sender, TaskEventArgs args )
        {
            // TODO: transmit this to the IDE.
        }


        #region Publish events

        public event EventHandler<FileEventArgs> Event_SolutionOpened;
        public void Raise_SolutionOpened(string solutionPath)
        {
            if (Event_SolutionOpened != null)
                Event_SolutionOpened(this, new FileEventArgs { Name = solutionPath });
        }

        public event EventHandler<FileEventArgs> Event_FileGotFocus;
        public void Raise_FileGotFocus(string fileName, string content)
        {
            if (Event_FileGotFocus != null)
                Event_FileGotFocus(this, new FileEventArgs { Name = fileName, Content = content });
        }

        public event EventHandler Event_NonSourceGotFocus;
        public void Raise_NonSourceGetsFocus()
        {
            if (Event_NonSourceGotFocus != null)
                Event_NonSourceGotFocus(this, new EventArgs());
        }

        public event EventHandler<FileEventArgs> Event_FileSaved;
        public void Raise_FileSaved(string fileName)
        {
            if (Event_FileSaved != null)
                Event_FileSaved(this, new FileEventArgs { Name = fileName });
        }

        public event EventHandler<FileEventArgs> Event_FilePasted;
        public void Raise_FilePasted(string fileName)
        {
            if (Event_FilePasted != null)
                Event_FilePasted(this, new FileEventArgs { Name = fileName });
        }

        public event EventHandler<FileListEventArgs> Event_FileSavedAs;
        public void Raise_FileSavedAs(string oldName, string newName)
        {
            if (Event_FileSavedAs != null)
            {
                var names = new List<string>();
                names.Add(oldName);
                names.Add(newName);
                Event_FileSavedAs(this, new FileListEventArgs { Names = names });
            }
        }
        
        public event EventHandler<FileListEventArgs> Event_FileRenamed;
        public void Raise_FileRenamed(string oldName, string newName)
        {
            if (Event_FileRenamed != null)
            {
                var names = new List<string>();
                names.Add(oldName);
                names.Add(newName);
                Event_FileRenamed(this, new FileListEventArgs { Names = names });
            }
        }

        #endregion
        //SELF: Bring in other events as they're found when wrestling with Visual Studio Addin API.
        //SELF: Think through other events that the ChangeWindow may publish
    }
}
