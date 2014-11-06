using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpQuant.UI.Command
{
    public interface ICheckedCommand: ICommand
    {
        event EventHandler IsCheckedChanged;

        bool IsChecked(Object parameter);
    }
}
