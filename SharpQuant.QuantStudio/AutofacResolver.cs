using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Autofac;
using SharpQuant.Common;

namespace QuantStudio
{
    public class AutofacResolver: IResolver
    {
        ILifetimeScope _container;

        public AutofacResolver(ILifetimeScope container)
        {
            _container = container;
        }
             
        public T Resolve<T>()
        {
            return _container.Resolve<T>();
        }

        public object Resolve(Type t)
        {
            return _container.Resolve(t);
        }
    }
}
