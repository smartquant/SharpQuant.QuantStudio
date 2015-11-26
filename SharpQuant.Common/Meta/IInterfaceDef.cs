using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuant.Common.Meta
{
    public interface IInterfaceDef
    {
        string Name { get; }
        string Description { get; }
        IList<IPropertyDef> Properties { get; }
    }


}
