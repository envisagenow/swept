//  Swept:  Software Enhancement Progress Tracking.  Copyright 2008 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
using System.Collections.Generic;
using System.Text;
using System;

namespace swept
{
    class EventDispatcher
    {
        internal TaskWindow taskWindow;
        public Librarian Librarian { get; set; }


        public void WhenPluginStarted()
        {
            //  Somebody else is firing up the dispatcher, if it catches the event here...
            Librarian = new Librarian();
        }

        public void WhenSolutionOpened( string solutionPath )
        {
            Librarian.OpenSolution(solutionPath);
        }

        public void WhenFileGetsFocus( string fileName )
        {
            taskWindow.ChangeFile(fileName, Librarian);
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

        public void WhenNonSourceGetsFocus()
        {
            taskWindow.NoSourceFile();
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

        //  We've implemented to here
    }
}
