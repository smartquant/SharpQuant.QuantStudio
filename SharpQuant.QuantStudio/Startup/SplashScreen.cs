
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

namespace QuantStudio.Startup
{
    public interface ISplashScreen
    {
        void SetProgressText(string text);
        void CloseThis();
    }

    public class SplashScreen : Form, ISplashScreen
	{
        private IContainer components;
        public Label lblCopyright;

        public Label lblProgress;
        public Label lblInfoText;
        public PictureBox pictureBox;


		public SplashScreen()
		{
			this.InitializeComponent();
		}



		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}



        #region Windows Form Designer generated code
        private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplashScreen));
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.lblProgress = new System.Windows.Forms.Label();
            this.lblCopyright = new System.Windows.Forms.Label();
            this.lblInfoText = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox
            // 
            this.pictureBox.BackColor = System.Drawing.Color.White;
            this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox.ErrorImage = ((System.Drawing.Image)(resources.GetObject("pictureBox.ErrorImage")));
            this.pictureBox.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox.Image")));
            this.pictureBox.Location = new System.Drawing.Point(1, 28);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(398, 156);
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            // 
            // lblProgress
            // 
            this.lblProgress.BackColor = System.Drawing.SystemColors.Control;
            this.lblProgress.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblProgress.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblProgress.Location = new System.Drawing.Point(1, 184);
            this.lblProgress.Name = "lblProgress";
            this.lblProgress.Padding = new System.Windows.Forms.Padding(5, 5, 0, 0);
            this.lblProgress.Size = new System.Drawing.Size(398, 30);
            this.lblProgress.TabIndex = 1;
            this.lblProgress.Text = "Loading";
            // 
            // lblCopyright
            // 
            this.lblCopyright.BackColor = System.Drawing.Color.White;
            this.lblCopyright.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblCopyright.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblCopyright.ForeColor = System.Drawing.Color.SteelBlue;
            this.lblCopyright.Location = new System.Drawing.Point(1, 1);
            this.lblCopyright.Name = "lblCopyright";
            this.lblCopyright.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.lblCopyright.Size = new System.Drawing.Size(398, 27);
            this.lblCopyright.TabIndex = 2;
            this.lblCopyright.Text = "Copyright (c) 2014 Joachim Loebb All rights reserved.";
            this.lblCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblInfoText
            // 
            this.lblInfoText.AutoSize = true;
            this.lblInfoText.Location = new System.Drawing.Point(136, 44);
            this.lblInfoText.Name = "lblInfoText";
            this.lblInfoText.Size = new System.Drawing.Size(16, 13);
            this.lblInfoText.TabIndex = 3;
            this.lblInfoText.Text = "...";
            // 
            // SplashScreen
            // 
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(400, 214);
            this.ControlBox = false;
            this.Controls.Add(this.lblInfoText);
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.lblCopyright);
            this.Controls.Add(this.lblProgress);
            this.Name = "SplashScreen";
            this.Padding = new System.Windows.Forms.Padding(1, 1, 1, 0);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SplashScreen";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
        #endregion


		public void SetProgressText(string text)
		{
            MethodInvoker methodInvoker = () => { lblProgress.Text = text; };
			this.lblProgress.Invoke(methodInvoker);
		}

        public void CloseThis()
        {
            MethodInvoker methodInvoker = () => { this.Close(); };
            this.Invoke(methodInvoker);
        }
	}
}