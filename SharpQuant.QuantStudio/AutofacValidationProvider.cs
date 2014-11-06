using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Autofac;
using SharpQuant.Common.Validation;

namespace QuantStudio
{
    class AutofacValidationProvider:IValidationProvider
    {

        ILifetimeScope _container;

        public AutofacValidationProvider(ILifetimeScope container)
        {
            _container = container;
        }

        public IValidator<T> GetValidator<T>() where T : class
        {
            IValidator<T> instance = null;
            if (_container.TryResolve<IValidator<T>>(out instance)) return instance;
            return instance;
        }
    }
}
