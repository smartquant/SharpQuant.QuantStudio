using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using System.IO;

using SharpQuant.UI.Docking;

namespace QuantStudio
{
    public partial class MainForm : Form
    {

        private bool m_bSaveLayout = true;
        private WindowManager _vm;

        public MainForm()
        {
            InitializeComponent();

            this.menuMinimizeToTray.Enabled = !SharpQuant.Common.Platform.IsUnix;

            dockPanel.SupportDeeplyNestedContent = true;
        }

        public void SetWindowmanager(WindowManager vm)
        {
            _vm=vm;
        }


        private void MainForm_Load(object sender, System.EventArgs e)
        {
            string configFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DockPanel.config");
            _vm.LoadLayoutFromXML(configFile);
        }

        private void MainForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string configFile = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DockPanel.config");
            if (m_bSaveLayout)
                dockPanel.SaveAsXml(configFile);
            else if (File.Exists(configFile))
                File.Delete(configFile);
        }

        #region static menus

        private void menuItemExit_Click(object sender, System.EventArgs e)
        {
            Close();
        }

        private void menuItemClose_Click(object sender, System.EventArgs e)
        {
            _vm.CloseActiveDocument();
        }

        private void menuItemCloseAll_Click(object sender, System.EventArgs e)
        {
            _vm.CloseAllDocuments();
        }



        private void menuItemToolBar_Click(object sender, System.EventArgs e)
        {
            toolBar.Visible = menuItemToolBar.Checked = !menuItemToolBar.Checked;
        }

        private void menuItemStatusBar_Click(object sender, System.EventArgs e)
        {
            statusBar.Visible = menuItemStatusBar.Checked = !menuItemStatusBar.Checked;
        }



        private void menuItemTools_Popup(object sender, System.EventArgs e)
        {
            menuItemLockLayout.Checked = !this.dockPanel.AllowEndUserDocking;
        }

        private void menuItemLockLayout_Click(object sender, System.EventArgs e)
        {
            dockPanel.AllowEndUserDocking = !dockPanel.AllowEndUserDocking;
        }

        private void menuItemCloseAllButThisOne_Click(object sender, System.EventArgs e)
        {
            _vm.CloseAllButThisOne();
        }

        private void menuItemShowDocumentIcon_Click(object sender, System.EventArgs e)
        {
            dockPanel.ShowDocumentIcon = menuItemShowDocumentIcon.Checked = !menuItemShowDocumentIcon.Checked;
        }

       

        private void exitWithoutSavingLayout_Click(object sender, EventArgs e)
        {
            m_bSaveLayout = false;
            Close();
            m_bSaveLayout = true;
        }

        #endregion





        #region Tray

        bool minimizedToTray;
        NotifyIcon notifyIcon;

        protected override void WndProc(ref Message message)
        {
            if (message.Msg == Startup.SingleInstance.WM_SHOWFIRSTINSTANCE)
            {
                ShowWindow();
            }
            base.WndProc(ref message);
        }
        private void menuMinimizeToTray_Click(object sender, EventArgs e)
        {
            MinimizeToTray();
        }
        void MinimizeToTray()
        {
            notifyIcon = new NotifyIcon();
            //notifyIcon.Click += new EventHandler(NotifyIconClick);
            notifyIcon.DoubleClick += new EventHandler(NotifyIconClick);
            notifyIcon.Icon = this.Icon;
            notifyIcon.Text = Startup.ProgramInfo.AssemblyTitle;
            notifyIcon.Visible = true;
            this.WindowState = FormWindowState.Minimized;
            this.Hide();
            minimizedToTray = true;
        }
        public void ShowWindow()
        {
            if (minimizedToTray)
            {
                notifyIcon.Visible = false;
                this.Show();
                this.WindowState = FormWindowState.Normal;
                minimizedToTray = false;
            }
            else
            {
                Startup.WinApi.ShowToFront(this.Handle);
            }
        }
        void NotifyIconClick(Object sender, System.EventArgs e)
        {
            ShowWindow();
        }


        #endregion



    }
}