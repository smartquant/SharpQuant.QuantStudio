using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        
        private static Dictionary<long, T> _cache;
        private static object sync = new object();
        private static bool _get_all_called = false;
        //private static string _tableName;
        private static Type _type;
        private string _searchField;
        Func<T, long> _getID;
        private IRepository<T> _repository;

        public CachedRepository(IRepository<T> repository, string searchField = "CODE", Func<T, long> getID = null)
        {
            _searchField = searchField;

            //type members
            if (_type == null)
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
            _repository = repository;
        }


        private Dictionary<long, T> Cache
        {
            get
            {
                if (_cache == null)
                    lock (sync)
                    _cache = new Dictionary<long, T>();
                return _cache; 
            }
        }

        public void ClearCache()
        {
            lock (sync)
                _cache = new Dictionary<long, T>();
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
                    if (!cache.TryGetValue(_getID(p), out entity))
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
                        cache.Add(_getID(p), p);
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
                    if (!cache.TryGetValue(_getID(p), out entity))
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
                Cache.Add(_getID(entity), entity);
        }

        public void Delete(T entity)
        {
            _repository.Delete(entity);
            lock (sync)
                Cache.Remove(_getID(entity));
        }

        public T GetSingle(long ID)
        {
            T entity = null;
            if (!Cache.TryGetValue(ID, out entity))
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

    }
}
