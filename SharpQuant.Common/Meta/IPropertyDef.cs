using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuant.Common.Meta
{
    public interface IPropertyDef
    {
        string Name { get; }
        IPropertyType PropertyType { get; }

        //ComponentModel
        string Category { get; }
        string DisplayName { get; }
        string DisplayFormat { get; } // {0:yyyyMMdd}
        string Description { get; }
        IList<IAttributeDef> Attributes { get; }
    }

}
