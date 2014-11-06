using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpQuant.UI.Command
{

    public sealed class DelegateCommand : ICommand
    {

        #region  Declarations

        readonly Func<Boolean> _canExecuteMethod;
        readonly Action _executeMethod;

        #endregion

        #region  Events


        public event EventHandler CanExecuteChanged;

        private void EmitCanExecuteChanged()
        {
            if (CanExecuteChanged!=null) CanExecuteChanged(this,EventArgs.Empty);
        }

        #endregion

        #region  Constructor


        public DelegateCommand(Action executeMethod)
            : this(executeMethod, null) {
        }

        public DelegateCommand(Action executeMethod, Func<Boolean> canExecuteMethod) {
            if(executeMethod == null) {
                throw new ArgumentNullException("executeMethod");
            }
            _executeMethod = executeMethod;
            _canExecuteMethod = canExecuteMethod;
        }

        #endregion

        #region  Methods


        public Boolean CanExecute(Object parameter) {
            return _canExecuteMethod == null || _canExecuteMethod();
        }


        public void Execute(Object parameter) {
            if(_executeMethod == null) {
                return;
            }
            _executeMethod();
        }

        #endregion
    }

}
