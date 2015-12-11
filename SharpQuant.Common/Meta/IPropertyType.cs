using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;

namespace SharpQuant.Common.Meta
{
    public interface IPropertyType
    {
        string CODE { get; }
        string QualifiedName { get; }
        Type Type { get; }
        DbType DbType { get; }
    };


}
