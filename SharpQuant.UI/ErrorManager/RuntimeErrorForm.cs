
using System;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Windows.Forms;


namespace SharpQuant.UI.ErrorManager
{
    internal class RuntimeErrorForm : Form
    {
        private Button btnClose;
        private Container container;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label lblDescription;
        private Label lblLevel;
        private Label lblSource;
        private TextBox tbxDetails;

        internal RuntimeErrorForm()
        {
            this.InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.container != null))
            {
                this.container.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblLevel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblSource = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tbxDetails = new System.Windows.Forms.TextBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(16, 64);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "Description:";
            // 
            // lblDescription
            // 
            this.lblDescription.Location = new System.Drawing.Point(80, 64);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(552, 32);
            this.lblDescription.TabIndex = 1;
            this.lblDescription.Text = "Description";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(16, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Level:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblLevel
            // 
            this.lblLevel.Location = new System.Drawing.Point(80, 40);
            this.lblLevel.Name = "lblLevel";
            this.lblLevel.Size = new System.Drawing.Size(552, 16);
            this.lblLevel.TabIndex = 3;
            this.lblLevel.Text = "Level";
            this.lblLevel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(16, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 16);
            this.label3.TabIndex = 4;
            this.label3.Text = "Source:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblSource
            // 
            this.lblSource.Location = new System.Drawing.Point(80, 16);
            this.lblSource.Name = "lblSource";
            this.lblSource.Size = new System.Drawing.Size(552, 16);
            this.lblSource.TabIndex = 5;
            this.lblSource.Text = "Source";
            this.lblSource.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(16, 104);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 16);
            this.label4.TabIndex = 6;
            this.label4.Text = "Details:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tbxDetails
            // 
            this.tbxDetails.Location = new System.Drawing.Point(16, 120);
            this.tbxDetails.Multiline = true;
            this.tbxDetails.Name = "tbxDetails";
            this.tbxDetails.ReadOnly = true;
            this.tbxDetails.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbxDetails.Size = new System.Drawing.Size(632, 185);
            this.tbxDetails.TabIndex = 7;
            this.tbxDetails.Text = "Details";
            this.tbxDetails.WordWrap = false;
            // 
            // btnClose
            // 
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(568, 311);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(80, 24);
            this.btnClose.TabIndex = 8;
            this.btnClose.Text = "Close";
            // 
            // RuntimeErrorForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(664, 347);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.tbxDetails);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblSource);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblLevel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RuntimeErrorForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Runtime Error";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        internal void ShowError(RuntimeError error)
        {
            this.lblLevel.Text = error.Level.ToString();
            this.lblDescription.Text = error.Description;
            this.lblSource.Text = (error.Source == null) ? "Not defined." : error.Source.ToString();
            this.tbxDetails.Text = error.Details;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.tbxDetails.Select(0, 0);
        }
    }
}
