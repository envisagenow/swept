//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
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
        internal ChangeCatalog _savedChangeCatalog;  //  For comparison to find unsaved changes.

        //  The Source File Catalog tracks which changes have been completed for which files.
        internal SourceFileCatalog _sourceCatalog;
        internal SourceFileCatalog _savedSourceCatalog;  //  For comparison to find unsaved changes.

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
                _savedSourceCatalog.SolutionPath = _solutionPath;
            }
        }

        public string LibraryPath
        {
            get { return Path.ChangeExtension(SolutionPath, "swept.library"); }
        }

        public ProjectLibrarian()
        {
            _sourceCatalog = new SourceFileCatalog();
            _savedSourceCatalog = new SourceFileCatalog();
            _savedChangeCatalog = new ChangeCatalog();
            _changeCatalog = new ChangeCatalog();
            _userAdapter = new UserGUIAdapter();
            _storageAdapter = new StorageAdapter();
        }

        internal bool SourceFileCatalogSaved
        {
            get { return _sourceCatalog.Equals( _savedSourceCatalog ); }
        }

        internal bool ChangeCatalogSaved
        {
            get { return _changeCatalog.Equals( _savedChangeCatalog ); }
        }

        internal bool IsSaved
        {
            get
            {
                return ChangeCatalogSaved && SourceFileCatalogSaved;
            }
        }


        private void OpenSolution(string solutionPath)
        {
            SolutionPath = solutionPath;

            XmlDocument libraryDoc = GetLibraryDocument();

            XmlPort port = new XmlPort();
            _changeCatalog = port.ChangeCatalog_FromXmlDocument( libraryDoc );

            _savedChangeCatalog = _changeCatalog.Clone();

            _sourceCatalog = port.SourceFileCatalog_FromXmlDocument( libraryDoc );
            _sourceCatalog.ChangeCatalog = _changeCatalog;
            _sourceCatalog.SolutionPath = SolutionPath;
            _savedSourceCatalog = _sourceCatalog.Clone();

            Raise_ChangeCatalogUpdated();
            Raise_SourceCatalogUpdated();
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

        private XmlDocument GetLibraryDocument()
        {
            XmlDocument doc;
            try
            {
                doc = _storageAdapter.LoadLibrary( LibraryPath );
            }
            catch( XmlException exception )
            {
                _userAdapter.BadXmlInExpectedLibrary( LibraryPath, exception );
                doc = StorageAdapter.emptyCatalogDoc;
                // TODO--0.3: Shut down addin cleanly on bad library XML
            }

            return doc;
        }

        private void SaveFile(string fileName)
        {
            _userAdapter.DebugMessage( string.Format( "Raise_FileSaved( {0} )", fileName ) );

            SourceFile workingFile = _sourceCatalog.Fetch(fileName);
            SourceFile diskFile = _savedSourceCatalog.Fetch(fileName);
            diskFile.CopyCompletionsFrom(workingFile);

            Persist();
        }

        private void PasteFile(string fileName)
        {
            SourceFile pastedWorkingFile = _sourceCatalog.Fetch(fileName);
            SourceFile pastedDiskFile = _savedSourceCatalog.Fetch(fileName);

            string copyPrefix = "Copy of ";
            if (fileName.StartsWith(copyPrefix))
            {
                string baseFileName = fileName.Substring(copyPrefix.Length);
                SourceFile baseDiskFile = _savedSourceCatalog.Fetch(baseFileName);

                pastedDiskFile.CopyCompletionsFrom(baseDiskFile);
            }

            pastedWorkingFile.CopyCompletionsFrom(pastedDiskFile);

            Persist();
        }

        private void SaveFileAs(string oldName, string newName)
        {
            SourceFile workingOriginalFile = _sourceCatalog.Fetch(oldName);
            SourceFile diskOriginalFile = _savedSourceCatalog.Fetch(oldName);
            SourceFile workingNewFile = _sourceCatalog.Fetch(newName);
            SourceFile diskNewFile = _savedSourceCatalog.Fetch(newName);

            diskNewFile.CopyCompletionsFrom(workingOriginalFile);
            workingOriginalFile.CopyCompletionsFrom(diskOriginalFile);
            workingNewFile.CopyCompletionsFrom(diskNewFile);

            Persist();
        }

        private void AbandonFileChanges(string fileName)
        {
            SourceFile workingFile = _sourceCatalog.Fetch(fileName);
            SourceFile diskFile = _savedSourceCatalog.Fetch(fileName);
            workingFile.CopyCompletionsFrom(diskFile);
        }

        private void AddChange(Change change)
        {
            _changeCatalog.Add(change);

            //if we have any completions pre-existing for this ID
            List<SourceFile> filesWithHistory = _sourceCatalog.Files.FindAll(file => file.Completions.Exists(c => c.ChangeID == change.ID));
            if (filesWithHistory.Count > 0)
            {
                bool keep = _userAdapter.KeepChangeHistory(change);

                //if discard, sweep them out of the file catalogs.
                if (!keep)
                {
                    filesWithHistory.ForEach(file => file.Completions.RemoveAll(c => c.ChangeID == change.ID));
                }
            }
        }

        private void RenameFile(string oldName, string newName)
        {
            _sourceCatalog.Rename(oldName, newName);
            _savedSourceCatalog.Rename(oldName, newName);

            Persist();
        }

        private void RenameSolution( string oldSolutionPath, string newSolutionPath )
        {
            string oldLibraryPath = LibraryPath;
            SolutionPath = newSolutionPath;
            _storageAdapter.RenameLibrary( oldLibraryPath, LibraryPath );
        }

        private void SaveSolution()
        {
            _savedSourceCatalog = _sourceCatalog.Clone();
            Persist();
        }

        #region Event Listeners
        public void Hear_SolutionRenamed( object sender, swept.FileListEventArgs e )
        {
            RenameSolution( e.Names[0], e.Names[1] );
        }

        public void Hear_SolutionOpened(object sender, FileEventArgs arg)
        {
            OpenSolution(arg.Name);
        }

        public void Hear_FileSaved(object sender, FileEventArgs args)
        {
            SaveFile(args.Name);
        }

        public void Hear_ChangeListUpdated(object sender, EventArgs e)
        {
            Persist();
        }

        public void Hear_FilePasted(object sender, FileEventArgs args)
        {
            PasteFile(args.Name);
        }

        public void Hear_FileSavedAs(object sender, FileListEventArgs args)
        {
            SaveFileAs(args.Names[0], args.Names[1]);
        }

        public void Hear_FileChangesAbandoned(object sender, FileEventArgs args)
        {
            AbandonFileChanges(args.Name);
        }

        public void Hear_FileRenamed(object sender, FileListEventArgs args)
        {
            RenameFile(args.Names[0], args.Names[1]);
        }

        public void Hear_ChangeAdded(object sender, ChangeEventArgs args)
        {
            AddChange(args.change);
        }

        public void Hear_SolutionSaved(object sender, EventArgs args)
        {
            SaveSolution();
        }

        #endregion

        internal void Persist()
        {
            _savedChangeCatalog = _changeCatalog.Clone();

            var port = new XmlPort();
            var library = _storageAdapter.LoadLibrary( LibraryPath );
            _savedChangeCatalog = port.ChangeCatalog_FromXmlDocument( library );
            _changeCatalog = _savedChangeCatalog.Clone();
            _storageAdapter.Save( LibraryPath, port.ToText( this ) );
        }
    }
}