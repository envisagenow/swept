using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace swept.Addin
{
    [GuidAttribute("9ED54F84-A89D-4fcd-A854-44251E925F09")]
    public partial class TaskWindow_GUI : UserControl
    {
        public TaskWindow_GUI()
        {
            InitializeComponent();
        }

        public void Hear_TaskListReset(object objNewTasks, EventArgs e)
        {
            List<Task> newTasks = (List<Task>)objNewTasks;

            // TODO: When tearing down the current taskGridView, properly Dispose of all.
            _taskGridView.Rows.Clear();

            foreach (Task task in newTasks)
            {
                int i = _taskGridView.Rows.Add();
                var row = _taskGridView.Rows[i];

                var menu = new ContextMenuStrip();
                foreach (var seeAlso in task.SeeAlsos)
                {
                    string label = string.Format("See also: {0}", seeAlso.Description);
                    var taskAction = new ToolStripMenuItem { Text = label, Tag = seeAlso };
                    taskAction.Click += when_ContextItemClicked;
                    menu.Items.Add(taskAction);
                }

                row.ContextMenuStrip = menu;

                row.Cells["Description"].Value = task.Description;
                row.Cells["Done"].Value = task.Completed;
                // TODO--0.3:  Populate Line number column
            }

            Refresh();
        }

        private void when_ContextItemClicked(object sender, EventArgs args)
        {
            ToolStripMenuItem item = (ToolStripMenuItem)sender;
            SeeAlso seeAlso = (SeeAlso)item.Tag;
            Raise_SeeAlsoFollowed(seeAlso);
        }

        public event EventHandler<SeeAlsoEventArgs> Event_SeeAlsoFollowed;
        public void Raise_SeeAlsoFollowed(SeeAlso seeAlso)
        {
            if (Event_SeeAlsoFollowed != null)
                Event_SeeAlsoFollowed(this, new SeeAlsoEventArgs { SeeAlso = seeAlso });
        }

        private void TaskWindow_GUI_Load(object sender, EventArgs e)
        {
        }
    }
}
