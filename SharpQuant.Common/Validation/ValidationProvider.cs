using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpQuant.Common.Validation
{
    public class ValidationProvider : IValidationProvider
    {
        private Dictionary<Type, object> _validators = new Dictionary<Type, object>();

        public IValidator<T> GetValidator<T>() where T : class
        {
            Type t = typeof(T);
            object v = null;
            if (!_validators.TryGetValue(t, out v))
            {
                return null;
            }
            return v as IValidator<T>;
        }

        public void AddValidator<T>(IValidator<T> validator) where T : class
        {
            Type t = typeof(T);
            _validators[t] = validator;
        }

    }
}
