using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpQuant.Common.DB
{
    public interface IRepositoryProvider : IDisposable
    {
        IRepository<T> GetRepository<T>() where T : class;
    }
}
