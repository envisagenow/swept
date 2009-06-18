//  Swept:  Software Enhancement Progress Tracking.  Copyright 2009 Envisage Technologies, some rights reserved.
//  This software is open source, under the terms of the MIT License.
//  The MIT License, roughly:  Keep this notice.  Beyond that, do whatever you want with this code.
using System.Collections.Generic;
using System.Text;
using System;

namespace swept
{
    public class TaskWindow
    {
        private SourceFile currentFile;
        internal SourceFileCatalog FileCatalog;
        internal ChangeCatalog ChangeCatalog;
        
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

        public bool Visible { get; set; }

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

        public void HearFileGotFocus(object sender, FileEventArgs args)
        {
            SourceFile file = FileCatalog.Fetch(args.Name);

            // TODO: Tuck this implementation into the ChangeCatalog
            List<Change> changes = ChangeCatalog.FindAll(c => c.Language == file.Language);
            ChangeFile(file, changes);
        }


        public void HearNonSourceGotFocus(object sender, EventArgs args)
        {
            ChangeFile(null, null);
        }


        public void ClickEntry( int index )
        {
            Task toggledChange = this.tasks[index];
            toggledChange.Completed = !toggledChange.Completed;
        }

        public void RefreshChangeList()
        {
            if (File == null) return;
            List<Change> changes = ChangeCatalog.FindAll(c => c.Language ==  File.Language);
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

        public void HearTaskWindowToggled(object sender, EventArgs e)
        {
            Visible = !Visible;
        }
    }
}
