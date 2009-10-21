//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System.Collections.Generic;
using System;

namespace swept
{
    public class TaskWindow
    {
        internal SourceFileCatalog _fileCatalog;
        internal ChangeCatalog _changeCatalog;
        internal IUserAdapter _GUIAdapter;

        private SourceFile _currentFile;
        public SourceFile CurrentFile
        {
            get
            {
                //SELF:  Keep an eye on this, it may be too cute.
                // A lazy-init "Null Object" pattern.  Cleans null checks and branches out of the code.
                if( _currentFile == null )
                    _currentFile = new SourceFile( "No source file" ) { Language = FileLanguage.None };
                return _currentFile;
            }
            private set
            {
                _currentFile = value;
            }
        }

        public String Title
        {
            get { return CurrentFile.Name; }
        }



        public TaskWindow()
        {
            Tasks = new List<Task>();
            _GUIAdapter = new UserGUIAdapter();
        }

        public List<Task> Tasks { get; private set; }
        public bool Visible { get; set; }

        private void ToggleWindowVisibility()
        {
            Visible = !Visible;
        }

        public void ToggleTaskCompletion( int index )
        {
            Task task = Tasks[index];
            SetTaskCompletion( task, !task.Completed );
        }

        // Smell:  With a name and args like this...
        private void SetTaskCompletion( Task task, bool completion )
        {
            task.Completed = completion;
            CurrentFile.AdjustCompletionFrom( task );
        }

        private void ShowFile( string fileName )
        {
            ShowFile( _fileCatalog.Fetch( fileName ) );
        }
        public void ShowFile( SourceFile file )
        {
            List<Change> changes = _changeCatalog.GetChangesForFile( file );
            ShowFile( file, changes );
        }
        internal void ShowFile( SourceFile file, List<Change> changes )
        {
            CurrentFile = file;
            SetTasksFromChanges( changes );
        }

        public void RefreshChangeCatalog( ChangeCatalog catalog )
        {
            _changeCatalog = catalog;
            List<Change> changes = _changeCatalog.GetChangesForFile( CurrentFile );
            SetTasksFromChanges( changes );
        }

        public void RefreshSourceCatalog( SourceFileCatalog catalog )
        {
            _fileCatalog = catalog;
        }

        private void SetTasksFromChanges( List<Change> changes )
        {
            Tasks.Clear();

            foreach( Change change in changes )
            {
                Task entry = Task.FromChange( change );
                entry.Completed = CurrentFile.Completions.Exists( c => c.ChangeID == entry.ID );
                Tasks.Add( entry );
            }

            Raise_TaskListReset();
        }

        #region Raise events

        public event EventHandler<EventArgs> Event_TaskWindowToggled;
        public void Raise_TaskWindowToggled()
        {
            if( Event_TaskWindowToggled != null )
                Event_TaskWindowToggled( this, new EventArgs { } );
        }

        public event EventHandler<EventArgs> Event_TaskListReset;
        public void Raise_TaskListReset()
        {
            if( Event_TaskListReset != null )
                Event_TaskListReset( Tasks, null );
        }

        #endregion

        #region Event listeners

        public void Hear_FileGotFocus( object sender, FileEventArgs args )
        {
            _GUIAdapter.DebugMessage( string.Format( "{0}( {1} )", "TaskWindow.Hear_FileGotFocus", args.Name ) );

            ShowFile( args.Name );
        }

        public void Hear_NonSourceGotFocus( object sender, EventArgs args )
        {
            ShowFile( null, new List<Change>() );
        }

        public void Hear_ChangeCatalogUpdated( object sender, ChangeCatalogEventArgs args )
        {
            RefreshChangeCatalog( args.Catalog );
        }

        public void Hear_SourceCatalogUpdated( object sender, SourceCatalogEventArgs args )
        {
            RefreshSourceCatalog( args.Catalog );
        }

        public void Hear_TaskWindowToggled( object sender, EventArgs args )
        {
            ToggleWindowVisibility();
        }

        public void Hear_TaskCheck( object sender, TaskEventArgs args )
        {
            SetTaskCompletion( args.Task, args.Checked );
        }
        #endregion


    }
}
