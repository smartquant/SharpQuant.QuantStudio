namespace QuantStudio
{
    partial class AboutDialog
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
            this.buttonOK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lblCopyright = new System.Windows.Forms.Label();
            this.labelAppVersion = new System.Windows.Forms.Label();
            this.rtfCopyrightsBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonOK.Location = new System.Drawing.Point(428, 304);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 0;
            this.buttonOK.Text = "OK";
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "QuantStudio Version:";
            // 
            // lblCopyright
            // 
            this.lblCopyright.Location = new System.Drawing.Point(12, 40);
            this.lblCopyright.Name = "lblCopyright";
            this.lblCopyright.Size = new System.Drawing.Size(272, 21);
            this.lblCopyright.TabIndex = 2;
            this.lblCopyright.Text = "Copyright 2014, Joachim Loebb";
            // 
            // labelAppVersion
            // 
            this.labelAppVersion.Location = new System.Drawing.Point(127, 9);
            this.labelAppVersion.Name = "labelAppVersion";
            this.labelAppVersion.Size = new System.Drawing.Size(97, 13);
            this.labelAppVersion.TabIndex = 5;
            // 
            // rtfCopyrightsBox
            // 
            this.rtfCopyrightsBox.Location = new System.Drawing.Point(15, 64);
            this.rtfCopyrightsBox.Name = "rtfCopyrightsBox";
            this.rtfCopyrightsBox.ReadOnly = true;
            this.rtfCopyrightsBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.rtfCopyrightsBox.Size = new System.Drawing.Size(471, 221);
            this.rtfCopyrightsBox.TabIndex = 6;
            this.rtfCopyrightsBox.Text = "";
            // 
            // AboutDialog
            // 
            this.AcceptButton = this.buttonOK;
            this.CancelButton = this.buttonOK;
            this.ClientSize = new System.Drawing.Size(515, 339);
            this.Controls.Add(this.rtfCopyrightsBox);
            this.Controls.Add(this.labelAppVersion);
            this.Controls.Add(this.lblCopyright);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About";
            this.Load += new System.EventHandler(this.AboutDialog_Load);
            this.ResumeLayout(false);

		}
		#endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Label labelAppVersion;
        public System.Windows.Forms.Label lblCopyright;
        public System.Windows.Forms.RichTextBox rtfCopyrightsBox;
    }
}