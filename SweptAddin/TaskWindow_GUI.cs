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
            List<Task> newTasks = (List<Task>)objNewTasks;
            tasks.Items.Clear();
            newTasks.ForEach( task => tasks.Items.Add( task ) );
        }

        private void tasks_SelectedIndexChanged( object sender, EventArgs e )
        {

        }

        private void TaskWindow_GUI_Load( object sender, EventArgs e )
        {

        }
    }
}
