using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpQuant.UI.Command
{
    /// <summary>
    /// Same as WPF Command, so code can be recompiled against WPF if needed
    /// </summary>
    public interface ICommand
    {
        event EventHandler CanExecuteChanged;

        bool CanExecute(Object parameter);
        void Execute(Object parameter);

    }

}
