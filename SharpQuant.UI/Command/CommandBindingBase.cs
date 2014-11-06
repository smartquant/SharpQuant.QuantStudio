using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace SharpQuant.UI.Command
{
    public abstract class CommandBindingBase
    {
        private Component component;
        private ICommand command;


        protected CommandBindingBase(Component component, ICommand command)
        {
            this.component = component;
            this.command = command;
            this.command.CanExecuteChanged += CommandCanExecuteChanged;
            this.component.Disposed += ComponentDisposed;
        }


        protected ICommand Command { get { return command; } }

        internal Component Component { get { return component; } }


        protected abstract void OnCommandCanExecuteChanged();

        protected abstract void OnComponentDisposed();

        private void CommandCanExecuteChanged(object sender, EventArgs e)
        {
            OnCommandCanExecuteChanged();
        }

        private void ComponentDisposed(object sender, EventArgs e)
        {
            OnComponentDisposed();
            command.CanExecuteChanged -= CommandCanExecuteChanged;
            component.Disposed -= ComponentDisposed;
            component = null;
            command = null;
        }
    }
}
