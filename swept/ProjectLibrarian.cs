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
        //  There are two major data collections per solution:  The Change and Source File catalogs.

        //  The Change Catalog holds things the team wants to improve in this solution.
        internal ChangeCatalog changeCatalog;

        //  The Source File Catalog holds all the completions, grouped per file.
        internal SourceFileCatalog savedSourceImage;

        //  Another copy of the source files are kept, to hold progress in multiple unsaved files.
        internal SourceFileCatalog unsavedSourceImage;

        internal IDialogPresenter showGUI;
        internal ILibraryPersister persister;
        public string SolutionPath { get; internal set; }
        public string LibraryPath
        {
            get { return Path.ChangeExtension(SolutionPath, "swept.library"); }
        }

        public ProjectLibrarian()
        {
            unsavedSourceImage = new SourceFileCatalog();
            savedSourceImage = new SourceFileCatalog();
            changeCatalog = new ChangeCatalog();
            showGUI = new DialogPresenter();
            persister = new LibraryPersister();
        }

        //TODO:  Reimplement flag in the SourceFileCatalog, make tests pass via more mature methods.
        internal bool SourceFileChangesUnsaved
        {
            get
            {
                return !unsavedSourceImage.Equals(savedSourceImage);
            }
        }

        internal bool ChangeNeedsPersisting
        {
            get
            {
                return changeCatalog.IsDirty || SourceFileChangesUnsaved;
            }
        }


        private void OpenSolution(string solutionPath)
        {
            SolutionPath = solutionPath;
            XmlPort port = new XmlPort();

            string libraryXmlText = GetLibraryXmlText();

            changeCatalog = port.ChangeCatalog_FromText(libraryXmlText);
            savedSourceImage = port.SourceFileCatalog_FromText(libraryXmlText);
            savedSourceImage.ChangeCatalog = changeCatalog;
            unsavedSourceImage = SourceFileCatalog.Clone(savedSourceImage);
        }

        private string GetLibraryXmlText()
        {
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
            SourceFile workingFile = unsavedSourceImage.Fetch(fileName);
            SourceFile diskFile = savedSourceImage.Fetch(fileName);
            diskFile.CopyCompletionsFrom(workingFile);

            Persist();
        }

        private void PasteFile(string fileName)
        {
            SourceFile pastedWorkingFile = unsavedSourceImage.Fetch(fileName);
            SourceFile pastedDiskFile = savedSourceImage.Fetch(fileName);

            string copyPrefix = "Copy of ";
            if (fileName.StartsWith(copyPrefix))
            {
                string baseFileName = fileName.Substring(copyPrefix.Length);
                SourceFile baseDiskFile = savedSourceImage.Fetch(baseFileName);

                pastedDiskFile.CopyCompletionsFrom(baseDiskFile);
            }

            pastedWorkingFile.CopyCompletionsFrom(pastedDiskFile);

            Persist();
        }

        private void SaveFileAs(string oldName, string newName)
        {
            SourceFile workingOriginalFile = unsavedSourceImage.Fetch(oldName);
            SourceFile diskOriginalFile = savedSourceImage.Fetch(oldName);
            SourceFile workingNewFile = unsavedSourceImage.Fetch(newName);
            SourceFile diskNewFile = savedSourceImage.Fetch(newName);

            diskNewFile.CopyCompletionsFrom(workingOriginalFile);
            workingOriginalFile.CopyCompletionsFrom(diskOriginalFile);
            workingNewFile.CopyCompletionsFrom(diskNewFile);

            Persist();
        }

        private void AbandonFileChanges(string fileName)
        {
            SourceFile workingFile = unsavedSourceImage.Fetch(fileName);
            SourceFile diskFile = savedSourceImage.Fetch(fileName);
            workingFile.CopyCompletionsFrom(diskFile);
        }

        private void DeleteFile(string fileName)
        {
            unsavedSourceImage.Delete(fileName);
            savedSourceImage.Delete(fileName);
            Persist();
        }

        private void AddChange(Change change)
        {
            changeCatalog.Add(change);

            //if we have any completions pre-existing for this ID
            List<SourceFile> filesWithHistory = unsavedSourceImage.Files.FindAll(file => file.Completions.Exists(c => c.ChangeID == change.ID));
            if (filesWithHistory.Count > 0)
            {
                bool keep = showGUI.KeepHistoricalCompletionsForChange(change);

                //if discard, sweep them out of the file catalogs.
                if (!keep)
                {
                    filesWithHistory.ForEach(file => file.Completions.RemoveAll(c => c.ChangeID == change.ID));
                }
            }
        }

        private void RenameFile(string oldName, string newName)
        {
            unsavedSourceImage.Rename(oldName, newName);
            savedSourceImage.Rename(oldName, newName);

            Persist();
        }

        private void SaveSolution()
        {
            savedSourceImage = SourceFileCatalog.Clone(unsavedSourceImage);
            Persist();
        }

        #region Event Listeners
        public void HearSolutionOpened(object sender, FileEventArgs arg)
        {
            OpenSolution(arg.Name);
        }

        public void HearFileSaved(object sender, FileEventArgs args)
        {
            SaveFile(args.Name);
        }

        public void HearChangeListUpdated(object sender, EventArgs e)
        {
            Persist();
        }

        public void HearFilePasted(object sender, FileEventArgs args)
        {
            PasteFile(args.Name);
        }

        public void HearFileSavedAs(object sender, FileListEventArgs args)
        {
            SaveFileAs(args.Names[0], args.Names[1]);
        }

        public void HearFileChangesAbandoned(object sender, FileEventArgs args)
        {
            AbandonFileChanges(args.Name);
        }

        public void HearFileDeleted(object sender, FileEventArgs args)
        {
            DeleteFile(args.Name);
        }

        public void HearFileRenamed(object sender, FileListEventArgs args)
        {
            RenameFile(args.Names[0], args.Names[1]);
        }

        public void HearChangeAdded(object sender, ChangeEventArgs args)
        {
            AddChange(args.change);
        }

        public void HearTaskCompletionChanged(object sender, EventArgs args)
        {
            SourceCatalogChanged();
        }

        public void HearSolutionSaved(object sender, EventArgs args)
        {
            SaveSolution();
        }

        #endregion

        public void SourceCatalogChanged()
        {
        }

        internal void Persist()
        {
            changeCatalog.MarkClean();

            var port = new XmlPort();

            persister.Save("swept.progress.library", port.ToText( this ));
        }
    }
}