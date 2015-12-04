using System;
using System.Data;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace SharpQuant.Common.DB
{
    public class MemoryRepository<T> : IUnitOfWork, IRepository<T> where T:class
    {
        private Dictionary<long, T> _list;
        private Dictionary<long, T> _transactions;      
        private IFactory<T> _factory;
        Func<T, long> _getID;

        private static Type _type;
        private string _searchField;

        public MemoryRepository(IEnumerable<T> items, string searchField = "CODE", IFactory<T> factory = null, Func<T, long> getID = null)
        {
                  
            _factory = (factory != null) ? factory : new DefaultFactory<T>();
            if (_type==null)
                _type = typeof(T);

            if (getID == null)
            {
                var prop = TypeDescriptor.GetProperties(_type).Find("id", true);
                if (prop == null)
                    _getID = t => t.GetHashCode();
                else
                    _getID = t => { var id = Convert.ToInt64(prop.GetValue(t)); return id == 0 ? t.GetHashCode() : id; };
            }
            else
                _getID = getID;
            _list = items.ToDictionary(t => _getID(t),t=>t);   
            _searchField = searchField;
        }

        public void ClearCache()
        {
            //Not needed
        }

        public IDbTransaction BeginTransaction(System.Data.IsolationLevel il = IsolationLevel.Unspecified)
        {
            
            if (_transactions==null)
                _transactions = new Dictionary<long, T>(_list);
            return new Transaction(() => CommitTransaction(), () => RollbackTransaction(),null, il, null);
        }

        void CommitTransaction()
        {
            if (_transactions==null)
                return;
            _list = _transactions;
            _transactions = null;
        }

        void RollbackTransaction()
        {
            if (_transactions == null)
                return;
            _transactions = null;
        }

        public IList<T> GetAll()
        {
            return _list.Values.ToList();
        }

        public T Create()
        {
            return _factory.CreateInstance();
        }

        public void Delete(T entity)
        {
            if (_transactions == null)
                _list.Remove(_getID(entity));
            else
                _transactions.Remove(_getID(entity));
        }

        public T GetSingle(long ID)
        {
            T item = default(T);
            if (_transactions == null)
                _list.TryGetValue((int)ID, out item);
            else
                _transactions.TryGetValue((int)ID, out item);
            return item;
        }

        public T GetSingle(string CODE)
        {
            return SearchFor(string.Format("({0}.{1}==\"{2}\")", _type.Name, _searchField, CODE)).FirstOrDefault();
        }

        public void Insert(T entity)
        {
            if (_transactions==null)
                _list.Add(_getID(entity), entity);
            else
                _transactions.Add(_getID(entity), entity);
        }

        public void Update(T entity)
        {
            if (_transactions == null)
                _list[_getID(entity)] = entity;
            else
                _transactions[_getID(entity)] = entity;
        }

        public IQueryable<T> SearchFor(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            if (_transactions == null)
                return _list.Values.AsQueryable().Where(predicate);
            else
                return _transactions.Values.AsQueryable().Where(predicate);
        }

        public IQueryable<T> SearchFor(string where_clause)
        {
            if (string.IsNullOrEmpty(where_clause))
                return GetAll().AsQueryable();
            var p = Expression.Parameter(_type, _type.Name);
            var e = System.Linq.Dynamic.DynamicExpression.ParseLambda(new[] { p }, null, where_clause);
            return SearchFor(e as Expression<Func<T, bool>>);
        }
    }
}
