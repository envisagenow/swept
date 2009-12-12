//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel;

namespace swept.Addin
{
    public partial class TaskWindow_GUI : Form
    {
        public TaskWindow_GUI()
        {
            InitializeComponent();
        }

        public void Hear_TaskListReset( object objNewTasks, EventArgs e )
        {
            List<Task> newTasks = (List<Task>)objNewTasks;  //here

            _taskGridView.Rows.Clear();

            foreach (Task task in newTasks)
            {
                int i = _taskGridView.Rows.Add();
                var row = _taskGridView.Rows[i];

                var menu = new ContextMenuStrip();
                foreach (var seeAlso in task.SeeAlsos)
                {
                    string label = string.Format( "See also: {0}", seeAlso.Description );
                    var taskAction = new ToolStripMenuItem{ Text = label, Tag = seeAlso };
                    taskAction.Click += when_ContextItemClicked;
                    menu.Items.Add( taskAction );
                }

                row.ContextMenuStrip = menu;

                row.Cells["Description"].Value = task.Description;
                row.Cells["Done"].Value = task.Completed;
                // TODO--0.3:  Populate Line number column
            }

            Refresh();
        }

        private void when_ContextItemClicked( object sender, EventArgs args )
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            MessageBox.Show( sender.ToString() );
            SeeAlso seeAlso = (SeeAlso)item.Tag;
            Raise_SeeAlsoFollowed( seeAlso );
        }

        public event EventHandler<SeeAlsoEventArgs> Event_SeeAlsoFollowed;
        public void Raise_SeeAlsoFollowed( SeeAlso seeAlso )
        {
            if (Event_SeeAlsoFollowed != null)
                Event_SeeAlsoFollowed( this, new SeeAlsoEventArgs { SeeAlso = seeAlso } );
        }




        //private void tasks_SelectedIndexChanged( object sender, EventArgs e )
        //{
        //}

        private void TaskWindow_GUI_Load( object sender, EventArgs e )
        {
        }
    }
}
