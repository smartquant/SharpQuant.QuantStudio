using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpQuant.Common.Validation
{
    public interface IValidator<T> where T : class
    {
        IList<string> GetErrorMessages(T entity);
        IList<string> GetErrorMessages(T entity, string fieldname);
        bool IsValid(T entity);
        bool IsValid(T entity, string fieldname);
    }


}
