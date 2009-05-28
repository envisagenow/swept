//  Swept:  Software Enhancement Progress Tracking.  Copyright 2008 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
using System.Collections.Generic;
using System.Text;
using System;

namespace swept
{
    public class TaskWindow
    {
        private SourceFile currentFile;
        internal ProjectLibrarian librarian;
        public SourceFile File
        {
            get { return currentFile; }
        }

        private String title;
        public String Title
        {
            get { return title; }
        }

        private List<Task> tasks = new List<Task>();
        public List<Task> Tasks
        {
            get { return tasks; }
        }


        public void ChangeFile( SourceFile file, List<Change> changes )
        {
            //  Stash completions in file leaving focus
            if( currentFile != null )
            {
                currentFile.SetCompletionsFromTasks( tasks );
            }

            currentFile = file;
            tasks = new List<Task>();
            if( file == null )
            {
                title = "No source file";
                return;
            }
            title = file.Name;

            BuildTasks(changes);
        }

        public void ChangeFile(object sender, FileEventArgs args)
        {
            SourceFile file = librarian.FetchWorkingFile(args.Name);
            List<Change> changes = librarian.changeCatalog.FindAll(c => c.Language == file.Language);
            ChangeFile(file, changes);
        }


        public void NoSourceFile(object sender, EventArgs args)
        {
            title = "No source file";
            tasks.Clear();
        }


        public void ClickEntry( int index )
        {
            Task toggledChange = this.tasks[index];
            toggledChange.Completed = !toggledChange.Completed;
        }

        public void RefreshChangeList(ChangeCatalog cat)
        {
            if (File == null) return;
            List<Change> changes = cat.FindAll(c => c.Language ==  File.Language);
            RefreshChangeList(changes);
        }

        public void RefreshChangeList(List<Change> changes)     //todo this may go away, preferring ChangeCat overload...
        {
            BuildTasks(changes);
        }

        private void BuildTasks(List<Change> changes)
        {
            if (changes == null) return;

            tasks.Clear();

            foreach (Change change in changes)
            {
                Task entry = Task.FromChange(change);
                entry.Completed = currentFile.Completions.Exists(c => c.ChangeID == entry.ID);
                tasks.Add(entry);
            }
        }
    }
}
