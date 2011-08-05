//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2011 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace swept
{
    public class ProjectLibrarian
    {
        //  The Change Catalog holds things the team wants to improve in this solution.
        internal ChangeCatalog _changeCatalog;

        internal IStorageAdapter _storageAdapter;
        internal EventSwitchboard _switchboard;

        private List<Task> _allTasks;

        private string _solutionPath;
        public string SolutionPath
        {
            get
            {
                return _solutionPath;
            }
            internal set
            {
                _solutionPath = value;
            }
        }

        public string LibraryPath {get; set;}

        public ProjectLibrarian( IStorageAdapter storageAdapter, EventSwitchboard switchboard )
        {
            _storageAdapter = storageAdapter;
            _switchboard = switchboard;

            _changeCatalog = new ChangeCatalog();
            _allTasks = new List<Task>();
        }


        #region Events
        public void Hear_SolutionOpened( object sender, FileEventArgs arg )
        {
            OpenSolution( arg.Name );
        }

        public void Hear_SolutionClosed( object sender, swept.FileEventArgs args )
        {
            CloseSolution();
        }
        public void Hear_FileOpened( object sender, FileEventArgs e )
        {
            OpenSourceFile( e.Name, e.Content );
        }

        public void Hear_FileClosing( object sender, FileEventArgs e )
        {
            throw new NotImplementedException();
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
            //clear task lisk
            //blank the ChangeCatalog
            throw new NotImplementedException();
        }

        internal void OpenSourceFile( string name, string content )
        {
            var openedFile = new SourceFile( name ) { Content = content };

            _allTasks.AddRange( GetTasksForFile( openedFile ) );
            _switchboard.Raise_TaskListChanged( _allTasks );
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
    }
}