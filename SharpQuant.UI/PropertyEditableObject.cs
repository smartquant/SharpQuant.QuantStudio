using System;
using SharpQuant.Common.DB;

namespace SharpQuant.UI
{

    public class PropertyEditableObject<T> : IPropertyEditableObject where T:class
    {
        private object _object;
        private bool _isdirty;
        private bool _isnew;
        private EPropertyEditAction _action;
        Action _updateMethod;

        public Action UpdateMethod
        {
            get { return _updateMethod; }
            set { _updateMethod = value; }
        }
        public object Object
        {
            get { return _object; }
            //set { _object = value; }
        }
        public bool IsNew
        {
            get { return _isnew; }
            set { _isnew = value; }
        }
       



        public EPropertyEditAction Action
        {
            get { return _action; }
            set { _action = value; }
        }



        public bool IsDirty
        {
            get { return _isdirty; }
            set { _isdirty = value; }
        }

        public PropertyEditableObject(T obj, bool isnew, Action updateMethod, EPropertyEditAction autosave)
        {
            _object = obj;
            _updateMethod = updateMethod;
            _isnew = isnew;
            _action = autosave;
        }
        
        public PropertyEditableObject(T obj, IRepository<T> dbcontext, bool isnew, EPropertyEditAction autosave)
        { 
            _object = obj;

            _updateMethod = () => { if (IsNew) dbcontext.Insert(obj); else dbcontext.Update(obj); };

            _isnew = isnew;
            _action = autosave;
        }
        public PropertyEditableObject(T obj, IRepository<T> dbcontext, bool isnew)
            : this(obj, dbcontext, isnew, EPropertyEditAction.None)
        {
        }
        public PropertyEditableObject(T obj)
            : this(obj, false,null, EPropertyEditAction.None)
        {
        }

    }
}
