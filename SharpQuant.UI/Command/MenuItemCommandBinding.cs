using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SharpQuant.UI.Command
{
    public class MenuItemCommandBinding : CommandBindingBase
    {
        private ToolStripMenuItem _menuItem;
        private Func<object> _commandParameterCallback;
        private ICheckedCommand _checkedCommand;


        public MenuItemCommandBinding(ToolStripMenuItem menuItem, ICommand command, Func<object> commandParameterCallback)
            : base(menuItem, command)
        {
            this._menuItem = menuItem;
            this._commandParameterCallback = commandParameterCallback;
            UpdateEnabledProperty();
            this._menuItem.Click += ButtonClick;

            _checkedCommand = command as ICheckedCommand;
            if (_checkedCommand != null)
            {
                this._menuItem.Checked = true;
                _checkedCommand.IsCheckedChanged += new EventHandler(IsCheckedChanged);
                UpdateCheckedProperty();
            }
        }

        void IsCheckedChanged(object sender, EventArgs e)
        {
            UpdateCheckedProperty();
        }

        protected override void OnCommandCanExecuteChanged()
        {
            UpdateEnabledProperty();
        }

        protected override void OnComponentDisposed()
        {
            _menuItem.Click -= ButtonClick;
            _menuItem = null;
            _commandParameterCallback = null;
            if (_checkedCommand!=null)
                _checkedCommand.IsCheckedChanged -= new EventHandler(IsCheckedChanged);
        }

        private void UpdateCheckedProperty()
        {
            var result = _checkedCommand.IsChecked(_commandParameterCallback());
            if (result)
                this._menuItem.CheckState = CheckState.Checked;
            else
                this._menuItem.CheckState = CheckState.Unchecked;
        }

        private void UpdateEnabledProperty()
        {
            _menuItem.Enabled = Command.CanExecute(_commandParameterCallback());
        }

        private void ButtonClick(object sender, EventArgs e)
        {
            Command.Execute(_commandParameterCallback());
        }
    }
}
