using System;
using System.Collections.Generic;
using System.ComponentModel;
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
       
        private static Dictionary<long, T> _cache;
        private static object sync = new object();
        private static Type _type;
        private string _searchField;
        private Func<T, long> _getID;
        private IRepository<T> _repository;

        public FullyCachedRepository(IRepository<T> repository, string searchField = "CODE", Func<T, long> getID=null)
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
                if (_cache==null)
                lock (sync)
                {
                    _cache = new Dictionary<long, T>();
                    //We fetch all the objects at once
                    _repository.GetAll().Select(p => { _cache.Add(_getID(p), p); return 0; }).Sum();
                }
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
                Cache.Add(_getID(entity), entity);
        }

        public void Delete(T entity)
        {
            _repository.Delete(entity);
            if (_cache != null)
            lock(sync)
                Cache.Remove(_getID(entity));
        }

        public T Create()
        {
            return _repository.Create();
        }

        public T GetSingle(long ID)
        {
            T record;
            if (Cache.TryGetValue(ID,out record))
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

    }
}
