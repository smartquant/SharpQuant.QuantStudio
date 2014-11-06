using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;
using SharpQuant.Batchjobs;

using SharpQuant.UI;

namespace SharpQuant.Batchjobs.UI
{
	public partial class BatchjobListView : UserControl
	{


        IPropertyEditableObjectEditor _editor;
        Action<BatchJob> _scheduleJob;

        public BatchjobListView()
		{
			InitializeComponent();

			this.BackColor = VisualStyleInformation.TextControlBorder;
			this.Padding = new Padding(1);

		}


        public void InitJobs(IEnumerable<BatchJob> jobs, IPropertyEditableObjectEditor editor, Action<BatchJob> scheduleJob)
        {

            _editor = editor;
            _scheduleJob = scheduleJob;

            var groups = jobs.GroupBy(p => p.Group, q => q, (group, code) => group).ToList();

            foreach (string group in groups)
            {
                TreeNode groupNode = new TreeNode(group);
                groupNode.Tag = new ListViewItem(new string[] { "", group });

                var groupJobs = jobs.Where(p => p.Group == group).OrderBy(p => p.Name).ToList();

                foreach (var groupJob in groupJobs)
                {
                    TreeNode node = new TreeNode(groupJob.Name);
                    var task = new TasklistViewItem(groupJob);
                    task.Tag = node; //use to give a reference for updating the treenode
                    node.Tag = task;
                    groupNode.Nodes.Add(node);
                }


                this.TreeView.Nodes.Add(groupNode);



            }
        }



		[Description("TreeView associated with the control"), Category("Behavior")]
		public TreeView TreeView
		{
			get
			{
				return this.treeView1;
			}
		}

		[Description("Columns associated with the control"), Category("Behavior")]
		public ListView.ColumnHeaderCollection Columns
		{
			get
			{
				return this.listView1.Columns;
			}
		}


        [Description("ListView associated with the control"), Category("Behavior")]
        public ListView ListView
        {
            get
            {
                return this.listView1;
            }
        }

		private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
		{
			this.treeView1.Focus();
		}

		private void treeView1_Click(object sender, EventArgs e)
		{
			Point p = this.treeView1.PointToClient(Control.MousePosition);
			TreeNode tn = this.treeView1.GetNodeAt(p);
			if (tn != null)
				this.treeView1.SelectedNode = tn;
		}

		private void listView1_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
		{
			this.treeView1.Focus();
			this.treeView1.Invalidate();
		}

		private void listView1_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
		{
			this.treeView1.Focus();
			this.treeView1.Invalidate();
		}

		private void treeView1_DrawNode(object sender, DrawTreeNodeEventArgs e)
		{
			e.DrawDefault = true;
            
			Rectangle rect = e.Bounds;
            
			if ((e.State & TreeNodeStates.Selected) != 0)
			{
				if ((e.State & TreeNodeStates.Focused) != 0)
					e.Graphics.FillRectangle(SystemBrushes.Highlight, rect);
				else
					e.Graphics.FillRectangle(SystemBrushes.Control, rect);
			}
			else
				e.Graphics.FillRectangle(Brushes.White, rect);

			e.Graphics.DrawRectangle(SystemPens.Control, rect);

			for (int intColumn = 1; intColumn < this.listView1.Columns.Count; intColumn++)
			{
				rect.Offset(this.listView1.Columns[intColumn - 1].Width, 0);
				rect.Width = this.listView1.Columns[intColumn].Width;

				e.Graphics.DrawRectangle(SystemPens.Control, rect);

				string strColumnText;
                ListViewItem list = e.Node.Tag as ListViewItem;
                TasklistViewItem tasklist = list as TasklistViewItem;

				if (list.SubItems.Count>2)
                    strColumnText = list.SubItems[intColumn+1].Text;
                else if (intColumn<2 && list.SubItems.Count <= 2)
                    strColumnText = list.SubItems[intColumn].Text;
                else
                    strColumnText = string.Empty;// intColumn + " " + e.Node.Text; // dummy

                TextFormatFlags flags = TextFormatFlags.EndEllipsis | TextFormatFlags.Left;
                //switch(this.listView1.Columns[intColumn].TextAlign)
                //{
                //    case HorizontalAlignment.Center:
                //        flags |= TextFormatFlags.HorizontalCenter;
                //        break;
                //    case HorizontalAlignment.Left:
                //        flags |= TextFormatFlags.Left;
                //        break;
                //    case HorizontalAlignment.Right:
                //        flags |= TextFormatFlags.Right;
                //        break;
                //    default:
                //        break;
                //}

                if (intColumn == 3 && tasklist!=null)
                {                 
                    ProgressBarRenderer.DrawHorizontalBar(e.Graphics, rect);
                    int width = (int)((double)tasklist.CurrentOp / (double)tasklist.TotalOps * (double)rect.Size.Width);
                    Rectangle prect = new Rectangle(rect.Location, new Size(width, rect.Size.Height));
                    ProgressBarRenderer.DrawHorizontalChunks(e.Graphics, prect);
                }
                else
                {
                    rect.Y++;
                    if ((e.State & TreeNodeStates.Selected) != 0 &&
                        (e.State & TreeNodeStates.Focused) != 0)
                        TextRenderer.DrawText(e.Graphics, strColumnText, e.Node.NodeFont, rect, SystemColors.HighlightText, flags);
                    else
                        TextRenderer.DrawText(e.Graphics, strColumnText, e.Node.NodeFont, rect, e.Node.ForeColor, e.Node.BackColor, flags);
                    rect.Y--;
                }
			}
		}

        private void treeView1_MouseClick(object sender, MouseEventArgs e)
        {
            TreeNode tn = this.treeView1.GetNodeAt(e.Location);

            if (tn != null)
            {
                var task = tn.Tag as TasklistViewItem;
                if (task != null)
                {
                    if (_editor != null)
                    {
                        var obj = new PropertyEditableObject<BatchJob>(task.Batchjob);
                        _editor.EditObject<BatchJob>(obj);
                    }
                }
            }

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                
                if (tn != null)
                {
                    var task = tn.Tag as TasklistViewItem;
                    if (task != null)
                    {
                        if (task.Batchjob.IsBusy)
                        {
                            ctxTasklist.Items[0].Enabled = false;
                            ctxTasklist.Items[1].Enabled = true && task.Batchjob.CanCancel;
                            ctxTasklist.Items[2].Enabled = _scheduleJob!=null;
                        }
                        else
                        {
                            ctxTasklist.Items[0].Enabled = true;
                            ctxTasklist.Items[1].Enabled = false;
                            ctxTasklist.Items[2].Enabled = false;
                        }
                        ctxTasklist.Show(this, e.Location);
                    }
                }
            }
        }

        private void menuRun_Click(object sender, EventArgs e)
        {
            var item = this.treeView1.SelectedNode.Tag as TasklistViewItem;
            item.Batchjob.RunAsync();
            item.UpdateStatus();
        }

        private void menuCancel_Click(object sender, EventArgs e)
        {
            var item = this.treeView1.SelectedNode.Tag as TasklistViewItem;
            if (item.Batchjob.IsBusy) item.Batchjob.Cancel();
            item.UpdateStatus();
        }

        private void menuSchedule_Click(object sender, EventArgs e)
        {
            var item = this.treeView1.SelectedNode.Tag as TasklistViewItem;

            if (_scheduleJob != null) _scheduleJob(item.Batchjob);
        }

        private void menuAbort_Click(object sender, EventArgs e)
        {
            var item = this.treeView1.SelectedNode.Tag as TasklistViewItem;
            if (item.Batchjob.IsBusy) item.Batchjob.Abort(3000);
            item.UpdateStatus();
        }


        



    }
}
