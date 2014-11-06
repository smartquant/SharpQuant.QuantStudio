using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SharpQuant.UI.Command
{
    public class ToolStripButtonCommandBinding : CommandBindingBase
    {
        private ToolStripButton _button;
        private Func<object> _commandParameterCallback;


        public ToolStripButtonCommandBinding(ToolStripButton button, ICommand command, Func<object> commandParameterCallback)
            : base(button, command)
        {
            this._button = button;
            this._commandParameterCallback = commandParameterCallback;
            UpdateEnabledProperty();
            this._button.Click += ButtonClick;
        }


        protected override void OnCommandCanExecuteChanged()
        {
            UpdateEnabledProperty();
        }

        protected override void OnComponentDisposed()
        {
            _button.Click -= ButtonClick;
            _button = null;
            _commandParameterCallback = null;
        }

        private void UpdateEnabledProperty()
        {
            _button.Enabled = Command.CanExecute(_commandParameterCallback());
        }

        private void ButtonClick(object sender, EventArgs e)
        {
            Command.Execute(_commandParameterCallback());
        }
    }
}
