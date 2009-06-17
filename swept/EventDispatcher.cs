//  Swept:  Software Enhancement Progress Tracking.  Copyright 2008 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
using System.Collections.Generic;
using System.Text;
using System;

namespace swept
{
    public class EventDispatcher
    {
        internal TaskWindow taskWindow;
        internal ChangeWindow changeWindow;
        public ProjectLibrarian Librarian { get; set; }

        #region Initialization

        public void WhenPluginStarted()
        {
            //  Somebody else is firing up the dispatcher, if it catches the event here...
            Librarian = new ProjectLibrarian();
        }

        public void WhenSolutionOpened( string solutionPath )
        {
            Librarian.OpenSolution(solutionPath);
        }
        #endregion



        #region Publish events

        public event EventHandler<FileEventArgs> RaiseFileGotFocus;
        public void WhenFileGotFocus( string fileName )
        {
            if (RaiseFileGotFocus != null)
                RaiseFileGotFocus(this, new FileEventArgs { Name = fileName });
        }

        public event EventHandler RaiseNonSourceGotFocus;
        public void WhenNonSourceGetsFocus()
        {
            if (RaiseNonSourceGotFocus != null)
                RaiseNonSourceGotFocus(this, new EventArgs());
        }

        public event EventHandler<FileEventArgs> RaiseFileSaved;
        public void WhenFileSaved( string fileName )
        {
            if (RaiseFileSaved != null)
                RaiseFileSaved(this, new FileEventArgs { Name = fileName });
        }

        public event EventHandler<FileEventArgs> RaiseFilePasted;
        public void WhenFilePasted(string fileName)
        {
            if (RaiseFilePasted != null)
                RaiseFilePasted(this, new FileEventArgs { Name = fileName });
        }

        public event EventHandler<FileListEventArgs> RaiseFileSavedAs;
        public void WhenFileSavedAs(string oldName, string newName)
        {
            if (RaiseFileSavedAs != null)
            {
                var names = new List<string>();
                names.Add(oldName);
                names.Add(newName);
                RaiseFileSavedAs(this, new FileListEventArgs { Names = names });
            }
        }

        public event EventHandler<FileEventArgs> RaiseFileChangesAbandoned;
        public void WhenFileChangesAbandoned( string fileName )
        {
            if( RaiseFileChangesAbandoned != null )
                RaiseFileChangesAbandoned( this, new FileEventArgs { Name = fileName } );
        }

        public event EventHandler<FileEventArgs> RaiseFileDeleted;
        public void WhenFileDeleted( string fileName )
        {
            if (RaiseFileDeleted != null)
                RaiseFileDeleted(this, new FileEventArgs { Name = fileName });
        }

        public event EventHandler<EventArgs> RaiseTaskWindowToggled;
        public void WhenTaskWindowToggled()
        {
            if (RaiseTaskWindowToggled != null)
                RaiseTaskWindowToggled(this, new EventArgs { });
        }


        public event EventHandler<FileListEventArgs> RaiseFileRenamed;
        public void WhenFileRenamed(string oldName, string newName)
        {
            if (RaiseFileRenamed != null)
            {
                var names = new List<string>();
                names.Add(oldName);
                names.Add(newName);
                RaiseFileRenamed(this, new FileListEventArgs { Names = names });
            }
        }

        public event EventHandler<ChangeEventArgs> RaiseChangeAdded;
        public void WhenChangeAdded( Change change )
        {
            if (RaiseChangeAdded != null)
            {
                RaiseChangeAdded(this, new ChangeEventArgs{ change = change });
            }

            WhenChangeListUpdated();
        }

        public event EventHandler RaiseTaskCompletionChanged;
        public void WhenTaskCompletionChanged()
        {
            if( RaiseTaskCompletionChanged != null )
                RaiseTaskCompletionChanged(this, new EventArgs());
        }

        public event EventHandler RaiseSolutionSaved;
        public void WhenSolutionSaved()
        {
            if ( RaiseSolutionSaved != null)
                RaiseSolutionSaved(this, new EventArgs());
        }

        #endregion


        //TODO:  Convert to event form
        public void WhenChangeListUpdated()
        {
            //TODO:  Figure out how to send this event without having to post the entire change catalog...
            taskWindow.RefreshChangeList(Librarian.changeCatalog);
            Librarian.Persist();    //  Save only the change list?
        }


        //SELF: Bring in other events as they're found when wrestling with Visual Studio Addin API.
        //SELF: Think through other events that the ChangeWindow may publish

    }

}
