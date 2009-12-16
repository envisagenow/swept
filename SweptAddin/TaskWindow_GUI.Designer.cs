namespace swept.Addin
{
    partial class TaskWindow_GUI
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this._taskGridView = new System.Windows.Forms.DataGridView();
            this.taskActionsMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.Done = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Line = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this._taskGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // _taskGridView
            // 
            this._taskGridView.AllowUserToAddRows = false;
            this._taskGridView.AllowUserToDeleteRows = false;
            this._taskGridView.AllowUserToOrderColumns = true;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this._taskGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this._taskGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._taskGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Done,
            this.Description,
            this.Line});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this._taskGridView.DefaultCellStyle = dataGridViewCellStyle2;
            this._taskGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._taskGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this._taskGridView.Location = new System.Drawing.Point(0, 0);
            this._taskGridView.Name = "_taskGridView";
            this._taskGridView.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this._taskGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this._taskGridView.Size = new System.Drawing.Size(519, 350);
            this._taskGridView.TabIndex = 3;
            // 
            // taskActionsMenu
            // 
            this.taskActionsMenu.Name = "taskActionsMenu";
            this.taskActionsMenu.Size = new System.Drawing.Size(61, 4);
            // 
            // Done
            // 
            this.Done.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Done.FillWeight = 15.22843F;
            this.Done.HeaderText = "Done";
            this.Done.Name = "Done";
            this.Done.ReadOnly = true;
            this.Done.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Done.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Done.ToolTipText = "Set to mean the issue is fixed, despite Swept still detecting it.";
            this.Done.Width = 50;
            // 
            // Description
            // 
            this.Description.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Description.FillWeight = 184.7716F;
            this.Description.HeaderText = "Description";
            this.Description.Name = "Description";
            this.Description.ReadOnly = true;
            // 
            // Line
            // 
            this.Line.HeaderText = "Line";
            this.Line.Name = "Line";
            this.Line.ReadOnly = true;
            this.Line.Visible = false;
            this.Line.Width = 40;
            // 
            // TaskWindow_GUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._taskGridView);
            this.Name = "TaskWindow_GUI";
            this.Size = new System.Drawing.Size(519, 350);
            ((System.ComponentModel.ISupportInitialize)(this._taskGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        internal System.Windows.Forms.DataGridView _taskGridView;
        private System.Windows.Forms.ContextMenuStrip taskActionsMenu;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Done;
        private System.Windows.Forms.DataGridViewTextBoxColumn Description;
        private System.Windows.Forms.DataGridViewTextBoxColumn Line;
    }
}
