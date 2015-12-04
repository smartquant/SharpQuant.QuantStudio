using System;
using System.Collections.Generic;
using System.Data;

namespace SharpQuant.Common.DB
{
    public interface IRepositoryProvider : IDisposable
    {
        IRepository<T> GetRepository<T>() where T : class;
        
    }

    public interface IUnitOfWork
    {
        IDbTransaction BeginTransaction(IsolationLevel il = IsolationLevel.Unspecified);
    }
}
