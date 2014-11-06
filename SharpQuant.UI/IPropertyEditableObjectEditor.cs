using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpQuant.UI
{
    public interface IPropertyEditableObjectEditor
    {
        void EditObject<T>(IPropertyEditableObject value) where T:class;
    }
}
