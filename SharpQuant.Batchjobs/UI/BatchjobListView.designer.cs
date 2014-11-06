namespace SharpQuant.Batchjobs.UI
{
    partial class BatchjobListView
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
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.ctxTasklist = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuRun = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCancel = new System.Windows.Forms.ToolStripMenuItem();
            this.menuSchedule = new System.Windows.Forms.ToolStripMenuItem();
            this.menuAbort = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxTasklist.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6});
            this.listView1.Dock = System.Windows.Forms.DockStyle.Top;
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.Location = new System.Drawing.Point(0, 0);
            this.listView1.Name = "listView1";
            this.listView1.Scrollable = false;
            this.listView1.Size = new System.Drawing.Size(736, 20);
            this.listView1.TabIndex = 3;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView1_ColumnClick);
            this.listView1.ColumnWidthChanged += new System.Windows.Forms.ColumnWidthChangedEventHandler(this.listView1_ColumnWidthChanged);
            this.listView1.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.listView1_ColumnWidthChanging);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Group / Name";
            this.columnHeader1.Width = 160;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Description";
            this.columnHeader3.Width = 208;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Status";
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Progress";
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Last message";
            this.columnHeader6.Width = 195;
            // 
            // treeView1
            // 
            this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawAll;
            this.treeView1.HideSelection = false;
            this.treeView1.Location = new System.Drawing.Point(0, 20);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(736, 327);
            this.treeView1.TabIndex = 2;
            this.treeView1.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.treeView1_DrawNode);
            this.treeView1.Click += new System.EventHandler(this.treeView1_Click);
            this.treeView1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.treeView1_MouseClick);
            // 
            // ctxTasklist
            // 
            this.ctxTasklist.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuRun,
            this.menuCancel,
            this.menuAbort,
            this.menuSchedule});
            this.ctxTasklist.Name = "contextMenuStrip1";
            this.ctxTasklist.Size = new System.Drawing.Size(153, 114);
            // 
            // menuRun
            // 
            this.menuRun.Image = SharpQuant.UI.Resources.cog_go;
            this.menuRun.Name = "menuRun";
            this.menuRun.Size = new System.Drawing.Size(131, 22);
            this.menuRun.Text = "Run";
            this.menuRun.Click += new System.EventHandler(this.menuRun_Click);
            // 
            // menuCancel
            // 
            this.menuCancel.Enabled = false;
            this.menuCancel.Image = SharpQuant.UI.Resources.cog_delete;
            this.menuCancel.Name = "menuCancel";
            this.menuCancel.Size = new System.Drawing.Size(131, 22);
            this.menuCancel.Text = "Cancel";
            this.menuCancel.Click += new System.EventHandler(this.menuCancel_Click);
            // 
            // menuSchedule
            // 
            this.menuSchedule.Image = SharpQuant.UI.Resources.database_add;
            this.menuSchedule.Name = "menuSchedule";
            this.menuSchedule.Size = new System.Drawing.Size(131, 22);
            this.menuSchedule.Text = "Schedule...";
            this.menuSchedule.Click += new System.EventHandler(this.menuSchedule_Click);
            // 
            // menuAbort
            // 
            this.menuAbort.Enabled = false;
            this.menuAbort.Image = SharpQuant.UI.Resources.cancel;
            this.menuAbort.Name = "menuAbort";
            this.menuAbort.Size = new System.Drawing.Size(152, 22);
            this.menuAbort.Text = "Abort";
            this.menuAbort.Click += new System.EventHandler(this.menuAbort_Click);
            // 
            // TreeViewColumns
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.listView1);
            this.Name = "TreeViewColumns";
            this.Size = new System.Drawing.Size(736, 347);
            this.ctxTasklist.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ContextMenuStrip ctxTasklist;
        private System.Windows.Forms.ToolStripMenuItem menuRun;
        private System.Windows.Forms.ToolStripMenuItem menuCancel;
        private System.Windows.Forms.ToolStripMenuItem menuSchedule;
        private System.Windows.Forms.ToolStripMenuItem menuAbort;
	}
}
