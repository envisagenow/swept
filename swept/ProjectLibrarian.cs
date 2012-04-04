//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2011 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace swept
{
    public interface IHearIDEEvents
    {
        void Hear_SolutionOpened( object sender, FileEventArgs args );
        void Hear_SolutionClosed( object sender, EventArgs args );
        void Hear_FileOpened( object sender, FileEventArgs args );
        void Hear_FileClosing( object sender, FileEventArgs args );
    }

    public class ProjectLibrarian : IHearIDEEvents
    {
        //  The Change Catalog holds things the team wants to improve in this solution.
        internal ChangeCatalog _changeCatalog;

        internal IStorageAdapter _storageAdapter;
        internal EventSwitchboard _switchboard;

        private List<Task> _allTasks;
        public string SolutionPath { get; internal set; }
        public string LibraryPath {get; set;}

        public ProjectLibrarian( IStorageAdapter storageAdapter, EventSwitchboard switchboard )
        {
            _storageAdapter = storageAdapter;
            _switchboard = switchboard;

            _changeCatalog = new ChangeCatalog();
            _allTasks = new List<Task>();
        }


        #region IHearIDEEvents
        public void Hear_SolutionOpened( object sender, FileEventArgs args )
        {
            OpenSolution( args.Name );
        }

        public void Hear_SolutionClosed( object sender, EventArgs args )
        {
            CloseSolution();
        }

        public void Hear_FileOpened( object sender, FileEventArgs args )
        {
            OpenSourceFile( args.Name, args.Content );
        }

        public void Hear_FileClosing( object sender, FileEventArgs args )
        {
            CloseSourceFile( args.Name );
        }

        #endregion


        private void OpenSolution(string solutionPath)
        {
            SolutionPath = solutionPath;
            LibraryPath = Path.ChangeExtension( SolutionPath, "swept.library" );

            OpenLibrary(LibraryPath);
        }

        public void OpenLibrary( string libraryPath )
        {
            XmlDocument libraryDoc = GetLibraryDocument( libraryPath );

            XmlPort port = new XmlPort();
            _changeCatalog = port.ChangeCatalog_FromXmlDocument( libraryDoc );

            // TODO:  Watch for FileSystem-level change events on the library file, and reload?
            
            _switchboard.Raise_ChangeCatalogUpdated(_changeCatalog);
        }

        private XmlDocument GetLibraryDocument( string libraryPath )
        {
            XmlDocument doc;
            try
            {
                doc = _storageAdapter.LoadLibrary( libraryPath );
            }
            catch (XmlException xex)
            {
                _switchboard.Raise_SweptException( xex );
                doc = StorageAdapter.emptyCatalogDoc;
            }
            catch (IOException ioex)
            {
                throw new IOException( String.Format( "Swept expects a library named [{0}].{1}Misnamed or didn't create Swept library?  Renamed solution but did not rename Swept library?", libraryPath, Environment.NewLine ), ioex );
            }

            return doc;
        }

        private void CloseSolution()
        {
            _allTasks.Clear();
            _changeCatalog = null;
        }

        internal void OpenSourceFile( string name, string content )
        {
            var openedFile = new SourceFile( name ) { Content = content };

            var newTasks = GetTasksForFile( openedFile );
            _allTasks.AddRange( newTasks );
            _switchboard.Raise_TaskListChanged( newTasks );
        }

        private List<Task> GetTasksForFile( SourceFile file )
        {
            var tasks = new List<Task>();
            foreach (var change in _changeCatalog.GetSortedChanges())
            {
                tasks.AddRange( Task.FromChangesForFile( change, file ) );
            }
            return tasks;
        }

        public List<Change> GetSortedChanges()
        {
            return _changeCatalog.GetSortedChanges();
        }

        private void CloseSourceFile( string name )
        {
            _allTasks.RemoveAll( task => name == task.File.Name );
        }
    }
}