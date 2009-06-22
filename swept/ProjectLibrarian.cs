//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace swept
{
    public class ProjectLibrarian
    {
        internal SourceFileCatalog unsavedSourceImage;
        internal SourceFileCatalog savedSourceImage;
        internal ChangeCatalog changeCatalog;
        internal IDialogPresenter showGUI;
        internal ILibraryWriter persister;
        public string SolutionPath { get; internal set; }

        public ProjectLibrarian()
        {
            unsavedSourceImage = new SourceFileCatalog();
            savedSourceImage = new SourceFileCatalog();
            changeCatalog = new ChangeCatalog();
            showGUI = new DialogPresenter();
            persister = new LibraryWriter();
        }

        //TODO:  Reimplement flag in the SourceFileCatalog, make tests pass via more mature methods.
        internal bool unsavedSourceChangesExist;
        internal bool ChangeNeedsPersisting
        {
            get
            {
                return changeCatalog.IsDirty || unsavedSourceChangesExist;
            }
        }


        public void OpenSolution(string solutionPath)
        {
            SolutionPath = solutionPath;

            // TODO: Make this not lame
            changeCatalog = LoadChangeCatalog(SolutionPath);
            unsavedSourceImage = LoadSourceFileCatalog(SolutionPath);
            
            unsavedSourceImage.ChangeCatalog = changeCatalog;
            savedSourceImage = SourceFileCatalog.Clone(unsavedSourceImage);
        }


        public SourceFile FetchUnsavedFile( string fileName )
        {
            return unsavedSourceImage.Fetch( fileName );
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
            unsavedSourceChangesExist = true;
        }

        internal void Persist() 
        {
            unsavedSourceChangesExist = false;
            changeCatalog.MarkClean();



            persister.Save( "swept.progress.library", ToXmlText() );
        }

        private string ToXmlText()
        {
            return string.Format(
@"<SweptProjectData>
{0}
{1}
</SweptProjectData>", 
                changeCatalog.ToXmlText(), 
                savedSourceImage.ToXmlText()
            );
        }

        protected SourceFileCatalog LoadSourceFileCatalog(string solutionPath) { return new SourceFileCatalog(); }
        protected ChangeCatalog LoadChangeCatalog( string solutionPath ) { return new ChangeCatalog(); }
    }
}