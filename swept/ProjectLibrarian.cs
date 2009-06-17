//  Swept:  Software Enhancement Progress Tracking.  Copyright 2008 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace swept
{
    public class ProjectLibrarian
    {
        internal ChangeCatalog changeCatalog;
        internal SourceFileCatalog InMemorySourceFiles;
        internal SourceFileCatalog LastSavedSourceFiles;
        internal IDialogPresenter talker;
        internal ILibraryWriter writer;
        
        public ProjectLibrarian()
        {
            InMemorySourceFiles = new SourceFileCatalog();
            LastSavedSourceFiles = new SourceFileCatalog();
            changeCatalog = new ChangeCatalog();
            talker = new DialogPresenter();
            writer = new LibraryWriter();
        }

        internal bool ChangeNeedsPersisting
        {
            get
            {
                return changeCatalog.IsDirty || sourceIsDirty;
            }
        }

        internal bool sourceIsDirty;

        public string SolutionPath { get; internal set; }
        public void OpenSolution(string solutionPath)
        {
            SolutionPath = solutionPath;
            changeCatalog = LoadChangeCatalog(SolutionPath);
            InMemorySourceFiles = LoadSourceFileCatalog(SolutionPath);
            InMemorySourceFiles.ChangeCatalog = changeCatalog;
            LastSavedSourceFiles = new SourceFileCatalog(InMemorySourceFiles);
        }


        public SourceFile FetchWorkingFile( string fileName )
        {
            return InMemorySourceFiles.FetchFile( fileName );
        }

        internal void SaveFile( object sender, FileEventArgs args )
        {
            SourceFile workingFile = InMemorySourceFiles.FetchFile( args.Name );
            SourceFile diskFile = LastSavedSourceFiles.FetchFile(args.Name);
            diskFile.CopyCompletionsFrom( workingFile );

            Persist();
        }

        public void PasteFile(object sender, FileEventArgs args)
        {
            string fileName = args.Name;
            SourceFile pastedWorkingFile = InMemorySourceFiles.FetchFile(fileName);
            SourceFile pastedDiskFile = LastSavedSourceFiles.FetchFile(fileName);

            string copyPrefix = "Copy of ";
            if (fileName.StartsWith(copyPrefix))
            {
                string baseFileName = fileName.Substring(copyPrefix.Length);
                SourceFile baseDiskFile = LastSavedSourceFiles.FetchFile(baseFileName);

                pastedDiskFile.CopyCompletionsFrom(baseDiskFile);
            }

            pastedWorkingFile.CopyCompletionsFrom(pastedDiskFile);

            Persist();
        }

        internal void SaveFileAs(object sender, FileListEventArgs args)
        {
            string oldName = args.Names[0];
            string newName = args.Names[1];

            SourceFile workingOriginalFile = InMemorySourceFiles.FetchFile(oldName);
            SourceFile diskOriginalFile = LastSavedSourceFiles.FetchFile(oldName);
            SourceFile workingNewFile = InMemorySourceFiles.FetchFile(newName);
            SourceFile diskNewFile = LastSavedSourceFiles.FetchFile(newName);

            diskNewFile.CopyCompletionsFrom(workingOriginalFile);
            workingOriginalFile.CopyCompletionsFrom(diskOriginalFile);
            workingNewFile.CopyCompletionsFrom(diskNewFile);

            Persist();
        }

        internal void RevertFile(object sender, FileEventArgs args)
        {
            SourceFile workingFile = InMemorySourceFiles.FetchFile( args.Name );
            SourceFile diskFile = LastSavedSourceFiles.FetchFile(args.Name);
            workingFile.CopyCompletionsFrom(diskFile);
        }

        public void DeleteFile(object sender, FileEventArgs args )
        {
            InMemorySourceFiles.Delete( args.Name );
            LastSavedSourceFiles.Delete( args.Name );

            Persist();
        }

        public void RenameFile(object sender, FileListEventArgs args)
        {
            string oldName = args.Names[0];
            string newName = args.Names[1];

            InMemorySourceFiles.Rename(oldName, newName);
            LastSavedSourceFiles.Rename(oldName, newName);

            Persist();
        }

        public void AddChange(object sender, ChangeEventArgs args)
        {
            Change change = args.change;
            changeCatalog.Add(change);

            //if we have any completions pre-existing for this ID
            List<SourceFile> filesWithHistory = InMemorySourceFiles.Files.FindAll(file => file.Completions.Exists(c => c.ChangeID == change.ID));
            if (filesWithHistory.Count > 0)
            {
                bool keep = talker.KeepHistoricalCompletionsForChange(change);

                //if discard, sweep them out of the file catalogs.
                if (!keep)
                {
                    filesWithHistory.ForEach(file => file.Completions.RemoveAll(c => c.ChangeID == change.ID));
                }
            }
        }

        public void SourceCatalogChanged()
        {
            sourceIsDirty = true;
        }

        virtual internal void Persist() 
        {
            sourceIsDirty = false;
            changeCatalog.MarkClean();

            writer.Save( "swept.progress.library", ToXmlText() );
        }

        private string ToXmlText()
        {
            return string.Format(
@"<SweptProjectData>
{0}
{1}
</SweptProjectData>", 
                changeCatalog.ToXmlText(), 
                LastSavedSourceFiles.ToXmlText()
            );
        }

        virtual protected SourceFileCatalog LoadSourceFileCatalog(string solutionPath) { return new SourceFileCatalog(); }
        virtual protected ChangeCatalog LoadChangeCatalog( string solutionPath ) { return new ChangeCatalog(); }
    }
}