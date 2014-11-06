using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SharpQuant.Common;

namespace SharpQuant.UI
{
    public interface IUserControlFactory<T> where T:UserControlBase
    {
        T Create(SettingsDictionary settings);
    }
}
