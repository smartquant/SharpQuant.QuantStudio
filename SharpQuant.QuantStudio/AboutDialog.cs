using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using WeifenLuo.WinFormsUI.Docking;

namespace QuantStudio
{
    public partial class AboutDialog : Form
    {
        public AboutDialog()
        {
            InitializeComponent();

            this.rtfCopyrightsBox.Rtf =
@"{\rtf1\ansi\ansicpg1252\deff0\deflang1033{\fonttbl{\f0\fnil\fcharset0 Microsoft Sans Serif;}}
\viewkind4\uc1\pard\f0\fs17
{\ul\b Icons}\par\par
{\b SweetiePlus Icons:}\tx2000\tab Copyright (C) 2010 Joseph North.\par
{\b Silk icon set 1.3:}\tx2000\tab Mark James http://www.famfamfam.com/lab/icons/silk/\par
\par
{\ul\b Assemblies}\par\par
{\b Autofac:}\tx2000\tab Copyright (c) 2014 Autofac Project (MIT License)\par
{\b DockPanel Suite:}\tx2000\tab Copyright (c) 2007-2014 Weifen Luo (MIT License)\par
}";

        }

        private void AboutDialog_Load(object sender, EventArgs e)
        {
            labelAppVersion.Text = typeof(MainForm).Assembly.GetName().Version.ToString();
        }

    }
}