using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace SharpQuant.Common.ComponenModel
{
    public abstract class ExpandableListBase<T> : IList<T>, IList, ICustomTypeDescriptor
    {
        protected IList<T> _list = new List<T>();
        protected Func<T, string> _displayName;


        #region ICustomTypeDescriptor

        public String GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public String GetComponentName()
        {
            return TypeDescriptor.GetComponentName(this, true);
        }

        public TypeConverter GetConverter()
        {
            return TypeDescriptor.GetConverter(this, true);
        }

        public EventDescriptor GetDefaultEvent()
        {
            return TypeDescriptor.GetDefaultEvent(this, true);
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            return TypeDescriptor.GetDefaultProperty(this, true);
        }

        public object GetEditor(Type editorBaseType)
        {
            return TypeDescriptor.GetEditor(this, editorBaseType, true);
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            return TypeDescriptor.GetEvents(this, attributes, true);
        }

        public EventDescriptorCollection GetEvents()
        {
            return TypeDescriptor.GetEvents(this, true);
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }


        /// <summary>
        /// Called to get the properties of this type. Returns properties with certain
        /// attributes. this restriction is not implemented here.
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return GetProperties();
        }

        /// <summary>
        /// Called to get the properties of this type.
        /// </summary>
        /// <returns></returns>
        public PropertyDescriptorCollection GetProperties()
        {
            return new PropertyDescriptorCollection(
                this.Select((p, i) => new CustomPropertyDescriptor(this, p, i, _displayName(p))).ToArray());
        }

        #endregion

        #region PropertyDescriptor

        class CustomPropertyDescriptor : PropertyDescriptor
        {
            T _prop;
            string _displayName;
            ExpandableListBase<T> _parent;

            public CustomPropertyDescriptor(ExpandableListBase<T> parent, T prop, int index, string displayName)
                : base("#" + index.ToString(), null)
            {
                _prop = prop;
                _displayName = displayName;
                _parent = parent;
            }

            public override bool CanResetValue(object component)
            {
                return true;
            }

            public override Type ComponentType
            {
                get { return _parent.GetType(); }
            }

            public override object GetValue(object component)
            {
                return _prop;
            }

            public override bool IsReadOnly
            {
                get { return true; }
            }

            public override Type PropertyType
            {
                get { return _prop.GetType(); }
            }

            public override void ResetValue(object component)
            {
            }

            public override void SetValue(object component, object value)
            {
            }


            public override bool ShouldSerializeValue(object component)
            {
                return true;
            }
            public override string DisplayName
            {
                get
                {
                    return _displayName;
                }
            }
        }

        #endregion

        #region simplistic change tracking

        IList<T> _removedItems;
        public IList<T> RemovedItems
        {
            get { return _removedItems ?? (_removedItems = new List<T>()); }
        }
        public bool Remove(T item)
        {
            var val = _list.Remove(item);
            if (val)
                RemovedItems.Add(item);
            return val;
        }
        public void Remove(object value)
        {
            ((IList)_list).Remove(value);
            RemovedItems.Add((T)value);
        }
        #endregion

        #region IList<T>

        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        public T this[int index]
        {
            get
            {
                return _list[index];
            }
            set
            {
                _list[index] = value;
            }
        }

        public void Add(T item)
        {
            _list.Add(item);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _list.Count; }
        }

        public bool IsReadOnly
        {
            get { return _list.IsReadOnly; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion

        #region IList

        public int Add(object value)
        {
            return ((IList)_list).Add(value);
        }

        public bool Contains(object value)
        {
            return ((IList)_list).Contains(value);
        }

        public int IndexOf(object value)
        {
            return ((IList)_list).IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            ((IList)_list).Insert(index, value);
        }

        public bool IsFixedSize
        {
            get { return ((IList)_list).IsFixedSize; }
        }



        object IList.this[int index]
        {
            get
            {
                return ((IList)_list)[index];
            }
            set
            {
                ((IList)_list)[index] = value;
            }
        }

        public void CopyTo(Array array, int index)
        {
            ((IList)_list).CopyTo(array, index);
        }

        public bool IsSynchronized
        {
            get { return ((IList)_list).IsSynchronized; }
        }

        public object SyncRoot
        {
            get { return ((IList)_list).SyncRoot; }
        }
        #endregion
    }
}
