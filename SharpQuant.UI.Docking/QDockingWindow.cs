using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

using SharpQuant.UI;

namespace SharpQuant.UI.Docking
{
    public partial class QDockingWindow : DockContent
    {
        private object _key;
        public object Key { get { return _key; } }


        #region serialized settings

        protected SettingsList _settings;
        public SettingsList Settings
        {
            get { if (_settings == null)_settings = new SettingsList(); return _settings; }
            set
            {
                if (value != null && value != _settings)
                {
                    this._settings = value;
                    this.OnChangeSettings();
                }
            }
        }

        /// <summary>
        /// Derived classes should override this, so they can react when settings were changed
        /// </summary>
        protected virtual void OnChangeSettings()
        {
        }

        /// <summary>
        /// Use this if a re-draw is sent from the dock manager
        /// </summary>
        public void UpdateUI()
        {
            this.OnUpdateUI();
        }

        protected virtual void OnUpdateUI()
        {
        }

        protected override string GetPersistString()
        {
            // Add extra information into the persist string for this document
            // so that it is available when deserialized.
            if (_settings == null)
                return GetType().ToString() + "$";
            else
                return GetType().ToString() + "$" + _settings.Serialize();
        }

        #endregion

    }
}