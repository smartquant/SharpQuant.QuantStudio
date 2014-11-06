using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SharpQuant.UI.Command
{
    public class ButtonCommandBinding : CommandBindingBase
    {
        private ButtonBase button;
        private Func<object> commandParameterCallback;


        public ButtonCommandBinding(ButtonBase button, ICommand command, Func<object> commandParameterCallback)
            : base(button, command)
        {
            this.button = button;
            this.commandParameterCallback = commandParameterCallback;
            UpdateEnabledProperty();
            this.button.Click += ButtonClick;
        }


        protected override void OnCommandCanExecuteChanged()
        {
            UpdateEnabledProperty();
        }

        protected override void OnComponentDisposed()
        {
            button.Click -= ButtonClick;
            button = null;
            commandParameterCallback = null;
        }

        private void UpdateEnabledProperty()
        {
            button.Enabled = Command.CanExecute(commandParameterCallback());
        }

        private void ButtonClick(object sender, EventArgs e)
        {
            Command.Execute(commandParameterCallback());
        }
    }
}
