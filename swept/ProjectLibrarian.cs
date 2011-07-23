//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace swept
{
    public class ProjectLibrarian
    {
        //  There are two major collections per solution:  The Change and Source File catalogs.

        //  The Change Catalog holds things the team wants to improve in this solution.
        internal ChangeCatalog _changeCatalog;

        internal SourceFileCatalog _sourceCatalog;

        internal IUserAdapter _userAdapter;
        internal IStorageAdapter _storageAdapter;

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
                _sourceCatalog.SolutionPath = _solutionPath;
            }
        }

        public string LibraryPath {get; set;}

        public ProjectLibrarian( IStorageAdapter storageAdapter, ChangeCatalog changeCatalog )
        {
            _storageAdapter = storageAdapter;
            _changeCatalog = changeCatalog;
            _sourceCatalog = new SourceFileCatalog();
            _userAdapter = new UserGUIAdapter();
        }

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

            _sourceCatalog = new SourceFileCatalog();
            _sourceCatalog.ChangeCatalog = _changeCatalog;
            _sourceCatalog.SolutionPath = SolutionPath;

            // TODO: return here to see if it's needed
            Raise_ChangeCatalogUpdated();
            Raise_SourceCatalogUpdated();
        }

        public List<Change> GetSortedChanges()
        {
            return _changeCatalog.GetSortedChanges();
        }

        public event EventHandler<ChangeCatalogEventArgs> Event_ChangeCatalogUpdated;
        public void Raise_ChangeCatalogUpdated()
        {
            if( Event_ChangeCatalogUpdated != null )
                Event_ChangeCatalogUpdated( this, new ChangeCatalogEventArgs { Catalog = _changeCatalog } );
        }

        public event EventHandler<SourceCatalogEventArgs> Event_SourceCatalogUpdated;
        public void Raise_SourceCatalogUpdated()
        {
            if( Event_SourceCatalogUpdated != null )
                Event_SourceCatalogUpdated( this, new SourceCatalogEventArgs { Catalog = _sourceCatalog } );
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
                _userAdapter.BadXmlInExpectedLibrary( libraryPath, xex );
                doc = StorageAdapter.emptyCatalogDoc;
                // TODO--0.3: Shut down addin cleanly on bad library XML
            }
            catch (IOException ioex)
            {
                throw new IOException( String.Format( "No such library as [{0}].{1}Wrong folder?  Typoed library name?", libraryPath, Environment.NewLine ), ioex );
            }

            return doc;
        }

        #region Event Listeners
        public void Hear_SolutionOpened(object sender, FileEventArgs arg)
        {
            OpenSolution(arg.Name);
        }

        #endregion
    }
}