namespace SharpQuant.UI.Console
{
    partial class ConsoleControl
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

        #region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConsoleControl));
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.ctxOutput = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuClearOutput = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCopyToClipboard = new System.Windows.Forms.ToolStripMenuItem();
            this.menuWordWrap = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxOutput.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtOutput
            // 
            this.txtOutput.ContextMenuStrip = this.ctxOutput;
            this.txtOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtOutput.Location = new System.Drawing.Point(0, 2);
            this.txtOutput.Multiline = true;
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtOutput.Size = new System.Drawing.Size(663, 361);
            this.txtOutput.TabIndex = 2;
            this.txtOutput.WordWrap = false;
            // 
            // ctxOutput
            // 
            this.ctxOutput.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuClearOutput,
            this.menuCopyToClipboard,
            this.menuWordWrap});
            this.ctxOutput.Name = "ctxOutput";
            this.ctxOutput.Size = new System.Drawing.Size(170, 70);
            this.ctxOutput.Opening += new System.ComponentModel.CancelEventHandler(this.ctxOutput_Opening);
            // 
            // menuClearOutput
            // 
            this.menuClearOutput.Image = SharpQuant.UI.Resources.page_delete;
            this.menuClearOutput.Name = "menuClearOutput";
            this.menuClearOutput.Size = new System.Drawing.Size(169, 22);
            this.menuClearOutput.Text = "Clear output";
            this.menuClearOutput.Click += new System.EventHandler(this.menuClearOutput_Click);
            // 
            // menuCopyToClipboard
            // 
            this.menuCopyToClipboard.Image = SharpQuant.UI.Resources.page_copy;
            this.menuCopyToClipboard.Name = "menuCopyToClipboard";
            this.menuCopyToClipboard.Size = new System.Drawing.Size(169, 22);
            this.menuCopyToClipboard.Text = "Copy to clipboard";
            this.menuCopyToClipboard.Click += new System.EventHandler(this.menuCopyToClipboard_Click);
            // 
            // menuWordWrap
            // 
            this.menuWordWrap.Name = "menuWordWrap";
            this.menuWordWrap.Size = new System.Drawing.Size(169, 22);
            this.menuWordWrap.Text = "Word wrap";
            this.menuWordWrap.Click += new System.EventHandler(this.menuWordWrap_Click);
            // 
            // OutputWindow
            // 
            this.ClientSize = new System.Drawing.Size(663, 365);
            this.Controls.Add(this.txtOutput);
            //this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft)
            //            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight)
            //            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop)
            //            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            //this.HideOnClose = true;
            //this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "OutputWindow";
            this.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            //this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.DockBottomAutoHide;
            //this.TabText = "Output";
            this.Text = "Output";
            this.ctxOutput.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.ContextMenuStrip ctxOutput;
        private System.Windows.Forms.ToolStripMenuItem menuClearOutput;
        private System.Windows.Forms.ToolStripMenuItem menuCopyToClipboard;
        private System.Windows.Forms.ToolStripMenuItem menuWordWrap;
    }
}