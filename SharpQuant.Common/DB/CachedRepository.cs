using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;


//using System.Linq.Dynamic;

namespace SharpQuant.Common.DB
{

    /*
     * If we want good performance, then we need a dictionary for the cache with the IDs as the key
     * therefore the cacheable DB objects must implement an interface/baseclass having an ID
     * 
     * - or - we can override int GetHashCode() 
     * => mind int < 2^31 <2'147'483'648
     * 
     * 
     * This cache adds only the once requested objects into memory
     * 
     */


    public class CachedRepository<T>: IRepository<T> where T : class
    {

        
        private static Dictionary<int, T> _cache;
        private static object sync = new object();
        private static bool _get_all_called = false;
        //private static string _tableName;
        private static Type _type;
        private string _searchField;

        private IRepository<T> _repository;

        public CachedRepository(IRepository<T> repository, string searchField = "CODE")
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
                if (_cache == null)
                    lock (sync)
                    _cache = new Dictionary<int, T>();
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
            if (_get_all_called)
                return Cache.Values.AsQueryable().Where(predicate);
            else
            {
                var result = _repository.SearchFor(predicate);
                var cache = Cache;
                lock(sync)
                result.ToList().ForEach(p =>
                {
                    T entity = null;
                    if (!cache.TryGetValue(p.GetHashCode(), out entity))
                    {
                        entity = p;
                    }
                    //cache.Add(entity.ID, entity);
                    cache.Add(entity.GetHashCode(), entity);
                });
                return cache.Values.AsQueryable();
            }
        }

        public IList<T> GetAll()
        {
            if (_get_all_called)
                return Cache.Values.ToList();
            else if (_cache == null || _cache.Count == 0)
            {
                var all = _repository.GetAll();
                var cache = Cache;
                lock (sync)
                    all.Select(p =>
                    {
                        cache.Add(p.GetHashCode(), p);
                        return 0;
                    }).Sum();
            }
            else //only some values are in cache
            {
                var all = _repository.GetAll();
                var cache = Cache;
                lock (sync)
                all.Select(p =>
                {
                    T entity = null;
                    if (!cache.TryGetValue(p.GetHashCode(), out entity))
                    {
                        entity = p;
                    }
                    cache.Add(entity.GetHashCode(), entity);
                    return 0;
                }).Sum();
            }
            _get_all_called = true;
            return Cache.Values.ToList();
        }

        public void Update(T entity)
        {
            _repository.Update(entity);            
        }

        public void Insert(T entity)
        {
            _repository.Insert(entity);
            lock(sync)
            Cache.Add(entity.GetHashCode(), entity);
        }

        public void Delete(T entity)
        {
            _repository.Delete(entity);
            lock (sync)
            Cache.Remove(entity.GetHashCode());
        }

        public T GetSingle(long ID)
        {
            T entity = null;
            if (!Cache.TryGetValue((int)ID, out entity))
            {
                entity = _repository.GetSingle(ID);
                lock (sync)
                Cache.Add((int)ID, entity);
            }

            return entity;
        }


        public T Create()
        {
            return _repository.Create();
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

        public void BeginTransaction(IsolationLevel il = IsolationLevel.Unspecified)
        {
            _repository.BeginTransaction(il);
        }

        public void CommitTransaction()
        {
            _repository.CommitTransaction();
        }

        public void RollbackTransaction()
        {
            _repository.RollbackTransaction();
            ClearCache();
        }
    }
}
