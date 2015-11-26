using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace SharpQuant.Common.Meta
{
    public interface IAttributeDef
    {
        string CODE { get; }
        string Name { get; }
        string QualifiedName { get; }
        TypeEnum ValueType { get; }
        string Value { get; }
        Attribute Attribute { get; }
    };
   
}
