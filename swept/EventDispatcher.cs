//  Swept:  Software Enhancement Progress Tracking.  Copyright 2008 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
using System.Collections.Generic;
using System.Text;
using System;

namespace swept
{
    public class FileEventArgs : EventArgs
    {
        public string Name;
    }


    public class EventDispatcher
    {
        internal TaskWindow taskWindow;
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


        public event EventHandler<FileEventArgs> RaiseFileGotFocus;
        public void WhenFileGetsFocus( string fileName )
        {
            if (RaiseFileGotFocus != null)
            {
                RaiseFileGotFocus(this, new FileEventArgs { Name = fileName } );
            }
        }


        public event EventHandler RaiseNonSourceGotFocus;
        public void WhenNonSourceGetsFocus()
        {
            if (RaiseNonSourceGotFocus != null)
                RaiseNonSourceGotFocus(this, new EventArgs());
        }

        public void WhenChangeListUpdated()
        {
            taskWindow.RefreshChangeList( Librarian.changeCatalog );
            Librarian.Persist();
        }

        public void WhenFileSaved( string fileName )
        {
            Librarian.SaveFile( fileName );
        }

        public void WhenFilePasted(string fileName)
        {
            Librarian.PasteFile( fileName );
        }

        public void WhenFileSavedAs(string originalName, string newName)
        {
            Librarian.SaveFileAs(originalName, newName);
        }

        public void WhenFileChangesAbandoned( string fileName )
        {
            Librarian.RevertFile( fileName );
        }

        public void WhenFileDeleted( string fileName )
        {
            Librarian.DeleteFile( fileName );
        }

        public void WhenFileRenamed(string oldName, string newName)
        {
            Librarian.RenameFile(oldName, newName);
        }

        public void WhenTaskCompletionChanged()
        {
            Librarian.SourceCatalogChanged();
        }

        public void WhenSolutionSaved()
        {
            Librarian.Persist();
        }

    }

}
