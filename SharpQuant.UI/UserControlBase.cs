using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using SharpQuant;

namespace SharpQuant.UI
{
    public class UserControlBase : UserControl 
    {

        public event EventHandler OnSettingsListChanged;

        protected void EmitSettingsListChanged()
        {
            if (OnSettingsListChanged != null) OnSettingsListChanged(this, EventArgs.Empty);
        }

        #region serializable settings

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
                    EmitSettingsListChanged();
                }
            }
        }
      

        /// <summary>
        /// Derived classes should override this, so they can react when settings were changed
        /// </summary>
        protected virtual void OnChangeSettings()
        {
        }




        #endregion

    }
}
