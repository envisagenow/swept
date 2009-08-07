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
        internal SourceFileCatalog FileCatalog;
        internal ChangeCatalog ChangeCatalog;

        public String Title
        {
            get { return CurrentFile.Name; }
        }

        private SourceFile _currentFile;
        public SourceFile CurrentFile
        {
            get
            {
                //SELF:  Keep an eye on this, it may be too cute.
                // A lazy-init "Null Object" pattern.  Cleans null checks and branches out of the code.
                if (_currentFile == null)
                    _currentFile = new SourceFile("No source file") { Language = FileLanguage.None };
                return _currentFile;
            }
            private set
            {
                _currentFile = value;
            }
        }


        private List<Task> tasks = new List<Task>();
        public List<Task> Tasks
        {
            get { return tasks; }
        }

        public bool Visible { get; set; }
        private void ToggleWindowVisibility()
        {
            Visible = !Visible;
        }

        public void ToggleTaskCompletion(int index)
        {
            Task task = this.tasks[index];
            task.Completed = !task.Completed;
            CurrentFile.AdjustCompletionFrom(task);
        }

        private void ShowFile(string fileName)
        {
            ShowFile(FileCatalog.Fetch(fileName));
        }

        public void ShowFile(SourceFile file)
        {
            List<Change> changes = ChangeCatalog.GetChangesForFile(file);
            ShowFile(file, changes);
        }


        internal void ShowFile(SourceFile file, List<Change> changes)
        {
            CurrentFile = file;
            ListTasks(changes);
        }

        public void RefreshChangeList()
        {
            List<Change> changes = ChangeCatalog.GetChangesForFile( CurrentFile );
            ListTasks(changes);
        }

        private void ListTasks(List<Change> changes)
        {
            tasks.Clear();
            if (changes == null) return;

            foreach (Change change in changes)
            {
                Task entry = Task.FromChange(change);
                entry.Completed = CurrentFile.Completions.Exists(c => c.ChangeID == entry.ID);
                tasks.Add(entry);
            }
        }

        #region Raise events

        public event EventHandler<EventArgs> EventTaskWindowToggled;
        public void RaiseTaskWindowToggled()
        {
            if (EventTaskWindowToggled != null)
                EventTaskWindowToggled(this, new EventArgs { });
        }

        #endregion

        #region Event listeners

        public void HearFileGotFocus(object sender, FileEventArgs args)
        {
            ShowFile(args.Name);
        }

        public void HearNonSourceGotFocus(object sender, EventArgs args)
        {
            ShowFile(null, null);
        }

        public void HearChangeListUpdated(object sender, EventArgs args)
        {
            RefreshChangeList();
        }

        public void HearTaskWindowToggled(object sender, EventArgs args)
        {
            ToggleWindowVisibility();
        }

        #endregion

    }
}
