using System.Windows.Forms;
namespace swept.Addin
{
    partial class TaskWindow_GUI
    {
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if( disposing && (components != null) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tasks = new System.Windows.Forms.CheckedListBox();
            this.SuspendLayout();
            // 
            // tasks
            // 
            this.tasks.CheckOnClick = true;
            this.tasks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tasks.FormattingEnabled = true;
            this.tasks.Location = new System.Drawing.Point( 0, 0 );
            this.tasks.Name = "tasks";
            this.tasks.Size = new System.Drawing.Size( 682, 364 );
            this.tasks.TabIndex = 0;
            this.tasks.SelectedIndexChanged += new System.EventHandler( this.tasks_SelectedIndexChanged );
            // 
            // TaskWindow_GUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF( 6F, 13F );
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size( 682, 369 );
            this.Controls.Add( this.tasks );
            this.Name = "TaskWindow_GUI";
            this.Text = "Swept Tasks";
            this.Load += new System.EventHandler( this.TaskWindow_GUI_Load );
            this.ResumeLayout( false );

        }

        #endregion

        internal CheckedListBox tasks;
    }
}