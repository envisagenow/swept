//  Swept:  Software Enhancement Progress Tracking.  Copyright 2008 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
using System;
using System.Collections.Generic;
using System.Text;

namespace swept
{
    public class Librarian
    {
        internal ChangeCatalog changeCatalog;
        internal SourceFileCatalog InMemorySourceFiles;
        internal SourceFileCatalog LastSavedSourceFiles;

        public Librarian()
        {
            InMemorySourceFiles = new SourceFileCatalog();
            LastSavedSourceFiles = new SourceFileCatalog();
            changeCatalog = new ChangeCatalog();
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

        internal void SaveFile( string fileName )
        {
            SourceFile workingFile = InMemorySourceFiles.FetchFile( fileName );
            SourceFile diskFile = LastSavedSourceFiles.FetchFile( fileName );
            diskFile.CopyCompletionsFrom( workingFile );

            Persist();
        }

        public void PasteFile(string fileName)
        {
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

        internal void SaveFileAs(string originalName, string newName)
        {
            SourceFile workingOriginalFile = InMemorySourceFiles.FetchFile(originalName);
            SourceFile diskOriginalFile = LastSavedSourceFiles.FetchFile(originalName);
            SourceFile workingNewFile = InMemorySourceFiles.FetchFile(newName);
            SourceFile diskNewFile = LastSavedSourceFiles.FetchFile(newName);

            diskNewFile.CopyCompletionsFrom(workingOriginalFile);
            workingOriginalFile.CopyCompletionsFrom(diskOriginalFile);
            workingNewFile.CopyCompletionsFrom(diskNewFile);

            Persist();
        }

        internal void RevertFile(string fileName)
        {
            SourceFile workingFile = InMemorySourceFiles.FetchFile( fileName );
            SourceFile diskFile = LastSavedSourceFiles.FetchFile( fileName );
            workingFile.CopyCompletionsFrom(diskFile);
        }

        public void DeleteFile( string fileName )
        {
            InMemorySourceFiles.Delete( fileName );
            LastSavedSourceFiles.Delete( fileName );

            Persist();
        }

        public void RenameFile(string oldName, string newName)
        {
            InMemorySourceFiles.Rename(oldName, newName);
            LastSavedSourceFiles.Rename(oldName, newName);

            Persist();
        }

        public void SourceCatalogChanged()
        {
            sourceIsDirty = true;
        }

        virtual internal void Persist() 
        {
            sourceIsDirty = false;
            changeCatalog.MarkClean();

            //eventually, actually persist to disk
        }



        virtual protected SourceFileCatalog LoadSourceFileCatalog( string solutionPath ) { return new SourceFileCatalog(); }
        virtual protected ChangeCatalog LoadChangeCatalog( string solutionPath ) { return new ChangeCatalog(); }
    }
}
