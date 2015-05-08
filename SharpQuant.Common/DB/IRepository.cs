using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace SharpQuant.Common.DB
{
    /// <summary>
    /// Generic repository pattern
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T> : IRepository
    {
        IList<T> GetAll();

        //CRUD
        T Create();
        void Delete(T entity);
        T GetSingle(long ID);
        T GetSingle(string CODE);
        void Insert(T entity);
        void Update(T entity);

        //searching
        IQueryable<T> SearchFor(Expression<Func<T, bool>> predicate);
        IQueryable<T> SearchFor(string where_clause);

        //cache
        void ClearCache();

        //transactions
        IDbTransaction BeginTransaction(IsolationLevel il = IsolationLevel.Unspecified);

    }

    public interface IRepository
    {
    }

}
