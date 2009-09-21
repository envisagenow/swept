//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace swept
{

    public class ProjectLibrarian
    {
        //  There are two major collections per solution:  The Change and Source File catalogs.

        //  The Change Catalog holds things the team wants to improve in this solution.
        internal ChangeCatalog changeCatalog;
        internal ChangeCatalog savedChangeCatalog;  //  For comparison to know when there are unsaved changes.

        //  The Source File Catalog tracks which changes have been completed for which files.
        internal SourceFileCatalog sourceCatalog;
        internal SourceFileCatalog savedSourceCatalog;  //  For comparison to know when there are unsaved changes.

        internal IDialogPresenter showGUI;
        internal ILibraryPersister persister;
        public string SolutionPath { get; internal set; }
        public string LibraryPath
        {
            get { return Path.ChangeExtension(SolutionPath, "swept.library"); }
        }

        public ProjectLibrarian()
        {
            sourceCatalog = new SourceFileCatalog();
            savedSourceCatalog = new SourceFileCatalog();
            savedChangeCatalog = new ChangeCatalog();
            changeCatalog = new ChangeCatalog();
            showGUI = new DialogPresenter();
            persister = new LibraryPersister();
        }

        internal bool SourceFileCatalogSaved
        {
            get { return sourceCatalog.Equals( savedSourceCatalog ); }
        }

        internal bool ChangeCatalogSaved
        {
            get { return changeCatalog.Equals( savedChangeCatalog ); }
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
            XmlPort port = new XmlPort();

            string libraryXmlText = GetLibraryXmlText();

            changeCatalog = port.ChangeCatalog_FromText( libraryXmlText );

            savedChangeCatalog = changeCatalog.Clone();

            savedSourceCatalog = port.SourceFileCatalog_FromText( libraryXmlText );
            savedSourceCatalog.ChangeCatalog = changeCatalog;
            sourceCatalog = savedSourceCatalog.Clone();
        }

        private string GetLibraryXmlText()
        {
            // TODO: all this goes into the persister, and the persister learns to read and write.
            string libraryXmlText = null;
            try
            {
                libraryXmlText = persister.LoadLibrary(LibraryPath);
            }
            catch
            {
                libraryXmlText = 
@"<SweptProjectData>
<ChangeCatalog>    
</ChangeCatalog>
<SourceFileCatalog>
</SourceFileCatalog>
</SweptProjectData>"; 
            }

            return libraryXmlText;
        }

        private void SaveFile(string fileName)
        {
            SourceFile workingFile = sourceCatalog.Fetch(fileName);
            SourceFile diskFile = savedSourceCatalog.Fetch(fileName);
            diskFile.CopyCompletionsFrom(workingFile);

            Persist();
        }

        private void PasteFile(string fileName)
        {
            SourceFile pastedWorkingFile = sourceCatalog.Fetch(fileName);
            SourceFile pastedDiskFile = savedSourceCatalog.Fetch(fileName);

            string copyPrefix = "Copy of ";
            if (fileName.StartsWith(copyPrefix))
            {
                string baseFileName = fileName.Substring(copyPrefix.Length);
                SourceFile baseDiskFile = savedSourceCatalog.Fetch(baseFileName);

                pastedDiskFile.CopyCompletionsFrom(baseDiskFile);
            }

            pastedWorkingFile.CopyCompletionsFrom(pastedDiskFile);

            Persist();
        }

        private void SaveFileAs(string oldName, string newName)
        {
            SourceFile workingOriginalFile = sourceCatalog.Fetch(oldName);
            SourceFile diskOriginalFile = savedSourceCatalog.Fetch(oldName);
            SourceFile workingNewFile = sourceCatalog.Fetch(newName);
            SourceFile diskNewFile = savedSourceCatalog.Fetch(newName);

            diskNewFile.CopyCompletionsFrom(workingOriginalFile);
            workingOriginalFile.CopyCompletionsFrom(diskOriginalFile);
            workingNewFile.CopyCompletionsFrom(diskNewFile);

            Persist();
        }

        private void AbandonFileChanges(string fileName)
        {
            SourceFile workingFile = sourceCatalog.Fetch(fileName);
            SourceFile diskFile = savedSourceCatalog.Fetch(fileName);
            workingFile.CopyCompletionsFrom(diskFile);
        }

        private void DeleteFile(string fileName)
        {
            sourceCatalog.Delete(fileName);
            savedSourceCatalog.Delete(fileName);
            Persist();
        }

        private void AddChange(Change change)
        {
            changeCatalog.Add(change);

            //if we have any completions pre-existing for this ID
            List<SourceFile> filesWithHistory = sourceCatalog.Files.FindAll(file => file.Completions.Exists(c => c.ChangeID == change.ID));
            if (filesWithHistory.Count > 0)
            {
                bool keep = showGUI.KeepChangeHistory(change);

                //if discard, sweep them out of the file catalogs.
                if (!keep)
                {
                    filesWithHistory.ForEach(file => file.Completions.RemoveAll(c => c.ChangeID == change.ID));
                }
            }
        }

        private void RenameFile(string oldName, string newName)
        {
            sourceCatalog.Rename(oldName, newName);
            savedSourceCatalog.Rename(oldName, newName);

            Persist();
        }

        private void SaveSolution()
        {
            savedSourceCatalog = sourceCatalog.Clone();
            Persist();
        }

        #region Event Listeners
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

        public void Hear_FileDeleted(object sender, FileEventArgs args)
        {
            DeleteFile(args.Name);
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
            savedChangeCatalog = changeCatalog.Clone();

            var port = new XmlPort();

            // TODO: the proper expected name
            persister.Save("swept.progress.library", port.ToText( this ));
        }
    }
}