using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SharpQuant.UI;

namespace SharpQuant.UI.Docking
{
    public class QViewForm:QDockingWindow
    {
        UserControl _control;
        UserControlBase _controlbase;

        public QViewForm(UserControl control)
        {
            this.Controls.Add(control);
            _control = control;
            _controlbase = control as UserControlBase;
            if (_controlbase!=null)
                _controlbase.OnSettingsListChanged += new EventHandler(OnSettingsListChanged);

            control.Dock = DockStyle.Fill;         
        }

        void OnSettingsListChanged(object sender, EventArgs e)
        {
            this.TabText = _controlbase.Settings.GetOrSet("tabtext", this.TabText);
        }

        protected override void OnClosed(EventArgs e)
        {
            if (_controlbase!=null)
                _controlbase.OnSettingsListChanged -= new EventHandler(OnSettingsListChanged);
            base.OnClosed(e);
        }

        protected override void OnChangeSettings()
        {
            if (_controlbase != null)
            {
                _controlbase.Settings = this.Settings;
            }
        }

        protected override string GetPersistString()
        {
            // Add extra information into the persist string for this document
            // so that it is available when deserialized.
            if (_settings == null)
                return _control.GetType().ToString() + "$";
            else
                return _control.GetType().ToString() + "$" + _settings.Serialize();
        }

    }
}
