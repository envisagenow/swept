//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System.Collections.Generic;
using System;

namespace swept
{
    public class TaskWindow
    {
        #region properties
        internal SourceFileCatalog _fileCatalog;
        internal ChangeCatalog _changeCatalog;
        internal IUserAdapter _UserAdapter;

        public List<Task> Tasks { get; private set; }
        public bool Visible { get; set; }

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

        #endregion

        public TaskWindow()
        {
            Tasks = new List<Task>();
            _UserAdapter = new UserGUIAdapter();
        }

        public void Follow( SeeAlso seeAlso )
        {
            _UserAdapter.ShowSeeAlso( seeAlso );
        }

        private void ToggleWindowVisibility()
        {
            Visible = !Visible;
        }

        private void ShowFile( string fileName, string content )
        {
            ShowFile( _fileCatalog.Fetch( fileName ), content );
        }
        public void ShowFile( SourceFile file, string content )
        {
            file.Content = content;
            List<Change> changes = _changeCatalog.GetChangesForFile( file );
            ShowFile( file, changes );
        }
        internal void ShowFile( SourceFile file, List<Change> changes )
        {
            CurrentFile = file;
            SetTasksFromChanges( changes, file );
        }

        public void RefreshChangeCatalog( ChangeCatalog catalog )
        {
            _changeCatalog = catalog;
            List<Change> changes = _changeCatalog.GetChangesForFile( CurrentFile );
            SetTasksFromChanges( changes, CurrentFile );
        }

        public void RefreshSourceCatalog( SourceFileCatalog catalog )
        {
            _fileCatalog = catalog;
        }

        private void SetTasksFromChanges( List<Change> changes, SourceFile sourceFile )
        {
            Tasks.Clear();

            foreach( Change change in changes )
            {
                foreach (var task in Task.FromIssueSet( change.GetIssueSet( sourceFile ) ))
                {
                    Tasks.Add(task);
                }
            }

            Raise_TaskListReset();
        }

        private void SendViewToTaskLocation( Task task )
        {
            Raise_TaskLocationSought( task );
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
            if (Event_TaskListReset != null)
                Event_TaskListReset( Tasks, null );
        }

        public event EventHandler<TaskEventArgs> Event_TaskLocationSought;
        public void Raise_TaskLocationSought( Task task )
        {
            if (Event_TaskLocationSought != null)
                Event_TaskLocationSought( Tasks, new TaskEventArgs { Task = task } );
        }

        #endregion

        #region Event listeners

        public void Hear_FileGotFocus( object sender, FileEventArgs args )
        {
            _UserAdapter.DebugMessage( string.Format( "{0}( {1} )", "TaskWindow.Hear_FileGotFocus", args.Name ) );

            ShowFile( args.Name, args.Content );
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

        public void Hear_SeeAlsoFollowed( object sender, SeeAlsoEventArgs args )
        {
            Follow( args.SeeAlso );
        }

        public void Hear_TaskChosen( object sender, TaskEventArgs args )
        {
            SendViewToTaskLocation( args.Task );
        }
        #endregion

    }
}
