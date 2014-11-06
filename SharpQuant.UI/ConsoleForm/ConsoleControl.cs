using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Reflection;



namespace SharpQuant.UI.Console
{
    public partial class ConsoleControl : UserControlBase
    {

        private OutputWriter standardOut;
        public bool _wordWrap = false;

        public ConsoleControl()
        {
            InitializeComponent();

            this.standardOut = new OutputWriter(this.txtOutput);

            System.Console.SetOut(this.standardOut);
            this.txtOutput.WordWrap = this.WordWrap;
            this.Disposed += new EventHandler(OutputWindow_Disposed);
            
        }

        protected override void OnChangeSettings()
        {
            //throw new NotImplementedException();
        }

        void OutputWindow_Disposed(object sender, EventArgs e)
        {
            System.Console.SetOut(TextWriter.Null);
        }

        public bool WordWrap
        {
            get
            {
                return _wordWrap;
            }
            set
            {
                if (this._wordWrap != value)
                {
                    _wordWrap = value;
                    this.txtOutput.WordWrap = value;
                }
            }
        }

        private void menuClearOutput_Click(object sender, EventArgs e)
        {
            this.txtOutput.Clear();
        }

        private void menuCopyToClipboard_Click(object sender, EventArgs e)
        {
            string text = this.txtOutput.Text;
            if (!string.IsNullOrEmpty(text))
            {
                Clipboard.SetText(text, TextDataFormat.Text);
            }

        }

        private void menuWordWrap_Click(object sender, EventArgs e)
        {
            this.WordWrap = this.menuWordWrap.Checked;
        }

        private void ctxOutput_Opening(object sender, CancelEventArgs e)
        {
            this.WordWrap = this.menuWordWrap.Checked;
        }





    }
}