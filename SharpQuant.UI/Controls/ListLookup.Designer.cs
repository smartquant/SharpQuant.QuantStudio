namespace SharpQuant.UI.Controls
{
    partial class ListLookup<T>
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
            this.lstDetails = new System.Windows.Forms.ListView();
            this.colCODE = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDescription = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.OK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lstDetails
            // 
            this.lstDetails.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colCODE,
            this.colName,
            this.colDescription});
            this.lstDetails.FullRowSelect = true;
            this.lstDetails.Location = new System.Drawing.Point(25, 28);
            this.lstDetails.MultiSelect = false;
            this.lstDetails.Name = "lstDetails";
            this.lstDetails.ShowGroups = false;
            this.lstDetails.Size = new System.Drawing.Size(620, 349);
            this.lstDetails.TabIndex = 0;
            this.lstDetails.UseCompatibleStateImageBehavior = false;
            this.lstDetails.View = System.Windows.Forms.View.Details;
            this.lstDetails.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.lstDetails_ItemSelectionChanged);
            this.lstDetails.DoubleClick += new System.EventHandler(this.lstDetails_DoubleClick);
            // 
            // colCODE
            // 
            this.colCODE.Text = "CODE";
            // 
            // colName
            // 
            this.colName.Text = "Name";
            this.colName.Width = 229;
            // 
            // colDescription
            // 
            this.colDescription.Text = "Description";
            this.colDescription.Width = 263;
            // 
            // OK
            // 
            this.OK.Location = new System.Drawing.Point(565, 388);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(79, 28);
            this.OK.TabIndex = 1;
            this.OK.Text = "OK";
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // ListLookup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(676, 427);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.lstDetails);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ListLookup";
            this.Text = "ListLookup";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lstDetails;
        private System.Windows.Forms.ColumnHeader colCODE;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colDescription;
        private System.Windows.Forms.Button OK;
    }
}