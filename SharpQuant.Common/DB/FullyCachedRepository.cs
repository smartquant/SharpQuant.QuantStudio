using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;


namespace SharpQuant.Common.DB
{

    /*
     * If we want good performance, then we need a dictionary for the cache with the IDs as the key
     * therefore the cacheable DB objects must implement an interface/baseclass having an ID
     * 
     * this implementation is best for small lookup value type of objects, where we want the
     * application use the same list of objects everywhere
     * 
     * note: cache is on type => only one cache per type
     */


    public class FullyCachedRepository<T> : IRepository<T> where T : class
    {
       
        private static Dictionary<int, T> _cache;
        private static object sync = new object();
        private static Type _type;
        private string _searchField;

        private IRepository<T> _repository;

        public FullyCachedRepository(IRepository<T> repository, string searchField = "CODE")
        {
            _searchField = searchField;
            //type members
            if (_type == null)
                _type = typeof(T);

            _repository = repository;
        }


        private Dictionary<int, T> Cache
        {
            get
            {
                if (_cache==null)
                lock (sync)
                {
                    _cache = new Dictionary<int, T>();
                    //We fetch all the objects at once
                    _repository.GetAll().Select(p => { _cache.Add(p.GetHashCode(), p); return 1; }).Sum();
                }
                return _cache; 
            }
        }

        public void ClearCache()
        {
            lock (sync)
                _cache = new Dictionary<int, T>();
        }

        public IQueryable<T> SearchFor(Expression<Func<T, bool>> predicate)
        {
            return Cache.Values.AsQueryable().Where(predicate);
        }

        public IList<T> GetAll()
        {
            return Cache.Values.ToList();
        }

        public void Update(T entity)
        {
             _repository.Update(entity);            
        }

        public void Insert(T entity)
        {
            _repository.Insert(entity);
            if (_cache != null)
            lock (sync)
                Cache.Add(entity.GetHashCode(), entity);
        }

        public void Delete(T entity)
        {
            _repository.Delete(entity);
            if (_cache != null)
            lock(sync)
                Cache.Remove(entity.GetHashCode());
        }

        public T Create()
        {
            return _repository.Create();
        }

        public T GetSingle(long ID)
        {
            T record;
            if (Cache.TryGetValue((int)ID,out record))
                return record;
            return default(T);
        }
        public virtual T GetSingle(string CODE)
        {
            return SearchFor(string.Format("({0}.{1}==\"{2}\")", _type.Name,_searchField, CODE)).FirstOrDefault();
        }

        public IQueryable<T> SearchFor(string where_clause)
        {
            if (string.IsNullOrEmpty(where_clause))
                return GetAll().AsQueryable();
            var p = Expression.Parameter(_type, _type.Name);
            var e = System.Linq.Dynamic.DynamicExpression.ParseLambda(new[] { p }, null, where_clause);
            return SearchFor(e as Expression<Func<T, bool>>);
        }

        public IDbTransaction BeginTransaction(IsolationLevel il = IsolationLevel.Unspecified)
        {
            using (var trans = _repository.BeginTransaction(il))
            {
                return new Transaction(() => trans.Commit(),
                    () => { trans.Rollback(); ClearCache(); }, il, trans.Connection);
            }

        }
    }
}
