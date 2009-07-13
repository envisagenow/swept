//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using System.Collections.Generic;
using System.Text;
using System;

namespace swept
{
    public class StudioAdapter
    {
        internal TaskWindow taskWindow;
        internal ChangeWindow changeWindow;
        public ProjectLibrarian Librarian { get; set; }

        #region Initialization

        // TODO: Convert to event form
        public void WhenAddinStarted()
        {
            // TODO: Scratch up a new everything when the Swept addin is started
            //Starter starter = new Starter(this);

            // TODO: ? Clear this self-load out?
            Librarian = new ProjectLibrarian();
        }

        #endregion


        #region Publish events

        public event EventHandler<FileEventArgs> EventSolutionOpened;
        public void RaiseSolutionOpened(string solutionPath)
        {
            if (EventSolutionOpened != null)
                EventSolutionOpened(this, new FileEventArgs { Name = solutionPath });
        }

        public event EventHandler<FileEventArgs> EventFileGotFocus;
        public void RaiseFileGotFocus(string fileName)
        {
            if (EventFileGotFocus != null)
                EventFileGotFocus(this, new FileEventArgs { Name = fileName });
        }

        public event EventHandler EventNonSourceGotFocus;
        public void RaiseNonSourceGetsFocus()
        {
            if (EventNonSourceGotFocus != null)
                EventNonSourceGotFocus(this, new EventArgs());
        }

        public event EventHandler<FileEventArgs> EventFileSaved;
        public void RaiseFileSaved(string fileName)
        {
            if (EventFileSaved != null)
                EventFileSaved(this, new FileEventArgs { Name = fileName });
        }

        public event EventHandler<FileEventArgs> EventFilePasted;
        public void RaiseFilePasted(string fileName)
        {
            if (EventFilePasted != null)
                EventFilePasted(this, new FileEventArgs { Name = fileName });
        }

        public event EventHandler<FileListEventArgs> EventFileSavedAs;
        public void RaiseFileSavedAs(string oldName, string newName)
        {
            if (EventFileSavedAs != null)
            {
                var names = new List<string>();
                names.Add(oldName);
                names.Add(newName);
                EventFileSavedAs(this, new FileListEventArgs { Names = names });
            }
        }

        public event EventHandler<FileEventArgs> EventFileChangesAbandoned;
        public void RaiseFileChangesAbandoned(string fileName)
        {
            if (EventFileChangesAbandoned != null)
                EventFileChangesAbandoned(this, new FileEventArgs { Name = fileName });
        }

        public event EventHandler<FileEventArgs> EventFileDeleted;
        public void RaiseFileDeleted(string fileName)
        {
            if (EventFileDeleted != null)
                EventFileDeleted(this, new FileEventArgs { Name = fileName });
        }


        public event EventHandler<FileListEventArgs> EventFileRenamed;
        public void RaiseFileRenamed(string oldName, string newName)
        {
            if (EventFileRenamed != null)
            {
                var names = new List<string>();
                names.Add(oldName);
                names.Add(newName);
                EventFileRenamed(this, new FileListEventArgs { Names = names });
            }
        }

        public event EventHandler EventSolutionSaved;
        public void RaiseSolutionSaved()
        {
            if (EventSolutionSaved != null)
                EventSolutionSaved(this, new EventArgs());
        }

        #endregion
        //SELF: Bring in other events as they're found when wrestling with Visual Studio Addin API.
        //SELF: Think through other events that the ChangeWindow may publish
    }
}
