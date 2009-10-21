//  Swept:  Software Enhancement Progress Tracking.
//  Copyright (c) 2009 Jason Cole and Envisage Technologies Corp.
//  This software is open source, MIT license.  See the file LICENSE for details.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

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
            tasks.Items.Clear();

            foreach( Task task in newTasks )
                tasks.Items.Add( task, task.Completed );

            this.Refresh();
        }

        private void tasks_SelectedIndexChanged( object sender, EventArgs e )
        {

        }

        private void TaskWindow_GUI_Load( object sender, EventArgs e )
        {

        }
    }
}
