using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpQuant.Common
{
    /// <summary>
    /// Lifetime is managed within the container
    /// </summary>
    public interface IResolver
    {
        T Resolve<T>();
        object Resolve(Type t);
    }
}
