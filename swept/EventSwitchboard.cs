//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2010 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System.Collections.Generic;
using System;

namespace swept
{
    public class EventSwitchboard
    {
        public ProjectLibrarian Librarian { get; set; }

        public void Hear_TaskLocationSought( object sender, TaskEventArgs args )
        {
            // TODO: transmit this to the IDE.
        }


        #region Events originating in GUI

        public event EventHandler<FileEventArgs> Event_SolutionOpened;
        public void Raise_SolutionOpened(string solutionPath)
        {
            if (Event_SolutionOpened != null)
                Event_SolutionOpened(this, new FileEventArgs { Name = solutionPath });
        }

        public event EventHandler<FileEventArgs> Event_SolutionClosed;
        public void Raise_SolutionClosed(string solutionPath)
        {
            if (Event_SolutionClosed != null)
                Event_SolutionClosed(this, new FileEventArgs { Name = solutionPath });
        }

        public event EventHandler<FileEventArgs> Event_FileSaved;
        public void Raise_FileSaved(string fileName)
        {
            if (Event_FileSaved != null)
                Event_FileSaved(this, new FileEventArgs { Name = fileName });
        }

        public event EventHandler<FileEventArgs> Event_FileOpened;
        public void Raise_FileOpened(string fileName)
        {
            if (Event_FileOpened != null)
                Event_FileOpened(this, new FileEventArgs { Name = fileName });
        }

        public event EventHandler<FileEventArgs> Event_FileClosing;
        public void Raise_FileClosing(string fileName)
        {
            if (Event_FileClosing != null)
                Event_FileClosing(this, new FileEventArgs { Name = fileName });
        }

        #endregion

        #region Events originating in Swept

        public event EventHandler<ExceptionEventArgs> Event_SweptException;
        public void Raise_SweptException( Exception ex )
        {
            if (Event_SweptException != null)
                Event_SweptException( this, new ExceptionEventArgs { Exception = ex } );
        }

        public event EventHandler<TasksEventArgs> Event_TaskListChanged;
        public void Raise_TaskListChanged(List<Task> tasks)
        {
            if (Event_TaskListChanged != null)
                Event_TaskListChanged( this, new TasksEventArgs { Tasks = tasks } );
        }

        #endregion

    }
}
