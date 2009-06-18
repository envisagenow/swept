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

        public void HearSolutionOpened(object sender, FileEventArgs args)
        {
            OpenSolution(args.Name);
        }

        public ProjectLibrarian()
        {
            unsavedSourceImage = new SourceFileCatalog();
            savedSourceImage = new SourceFileCatalog();
            changeCatalog = new ChangeCatalog();
            showGUI = new DialogPresenter();
            persister = new LibraryWriter();
        }

        internal bool ChangeNeedsPersisting
        {
            get
            {
                return changeCatalog.IsDirty || unsavedSourceChangesExist;
            }
        }

        //TODO:  Reimplement flag in the SourceFileCatalog, make tests pass via more mature methods.
        internal bool unsavedSourceChangesExist;

        public string SolutionPath { get; internal set; }
        public void OpenSolution(string solutionPath)
        {
            SolutionPath = solutionPath;
            changeCatalog = LoadChangeCatalog(SolutionPath);
            unsavedSourceImage = LoadSourceFileCatalog(SolutionPath);
            unsavedSourceImage.ChangeCatalog = changeCatalog;
            savedSourceImage = SourceFileCatalog.Clone(unsavedSourceImage);
        }


        public SourceFile FetchUnsavedFile( string fileName )
        {
            return unsavedSourceImage.Fetch( fileName );
        }

        #region Event Listeners
        internal void HearFileSaved(object sender, FileEventArgs args)
        {
            SourceFile workingFile = unsavedSourceImage.Fetch( args.Name );
            SourceFile diskFile = savedSourceImage.Fetch(args.Name);
            diskFile.CopyCompletionsFrom( workingFile );

            Persist();
        }

        public void HearFilePasted(object sender, FileEventArgs args)
        {
            string fileName = args.Name;
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

        internal void HearFileSavedAs(object sender, FileListEventArgs args)
        {
            string oldName = args.Names[0];
            string newName = args.Names[1];

            SourceFile workingOriginalFile = unsavedSourceImage.Fetch(oldName);
            SourceFile diskOriginalFile = savedSourceImage.Fetch(oldName);
            SourceFile workingNewFile = unsavedSourceImage.Fetch(newName);
            SourceFile diskNewFile = savedSourceImage.Fetch(newName);

            diskNewFile.CopyCompletionsFrom(workingOriginalFile);
            workingOriginalFile.CopyCompletionsFrom(diskOriginalFile);
            workingNewFile.CopyCompletionsFrom(diskNewFile);

            Persist();
        }

        internal void HearFileChangesAbandoned(object sender, FileEventArgs args)
        {
            SourceFile workingFile = unsavedSourceImage.Fetch( args.Name );
            SourceFile diskFile = savedSourceImage.Fetch(args.Name);
            workingFile.CopyCompletionsFrom(diskFile);
        }

        public void HearFileDeleted(object sender, FileEventArgs args )
        {
            unsavedSourceImage.Delete( args.Name );
            savedSourceImage.Delete( args.Name );

            Persist();
        }

        public void HearFileRenamed(object sender, FileListEventArgs args)
        {
            string oldName = args.Names[0];
            string newName = args.Names[1];

            unsavedSourceImage.Rename(oldName, newName);
            savedSourceImage.Rename(oldName, newName);

            Persist();
        }

        public void HearChangeAdded(object sender, ChangeEventArgs args)
        {
            Change change = args.change;
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

        public void HearTaskCompletionChanged(object sender, EventArgs args)
        {
            SourceCatalogChanged();
        }

        public void HearSolutionSaved(object sender, EventArgs args)
        {
            savedSourceImage = SourceFileCatalog.Clone(unsavedSourceImage);
            Persist();
        }

        #endregion

        public void SourceCatalogChanged()
        {
            unsavedSourceChangesExist = true;
        }

        virtual internal void Persist() 
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

        virtual protected SourceFileCatalog LoadSourceFileCatalog(string solutionPath) { return new SourceFileCatalog(); }
        virtual protected ChangeCatalog LoadChangeCatalog( string solutionPath ) { return new ChangeCatalog(); }
    }
}