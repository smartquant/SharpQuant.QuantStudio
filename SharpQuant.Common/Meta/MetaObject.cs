using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuant.Common.Meta
{
    public interface IMetaObject
    {
        IEnumerable<IInterfaceDef> InterfaceDefs();
    }

    public class MetaObject : DynamicObject, IDictionary<string, object>, ICustomTypeDescriptor, IMetaObject
    {
        protected class ObjectDef
        {
            public Type Type;
            public object Object;
            public IInterfaceDef Interface;
            public IPropertyDef PropertyDef;
            public Lazy<Attribute[]> Attributes;

            public ObjectDef()
            {
                Attributes = new Lazy<Attribute[]>(() => GetCustomAttributes(this));
            }

            static Attribute[] GetCustomAttributes(ObjectDef def)
            {
                var list = new List<Attribute>();
                if (def.Interface != null && !string.IsNullOrEmpty(def.Interface.Name))
                    list.Add(new CategoryAttribute(def.Interface.Name));
                else if (!string.IsNullOrEmpty(def.PropertyDef.Category))
                    list.Add(new CategoryAttribute(def.PropertyDef.Category));
                if (!string.IsNullOrEmpty(def.PropertyDef.Description))
                    list.Add(new DescriptionAttribute(def.PropertyDef.Description));
                if (!string.IsNullOrEmpty(def.PropertyDef.DisplayName))
                    list.Add(new DisplayNameAttribute(def.PropertyDef.DisplayName));

                if (def.PropertyDef.Attributes!=null)
                    foreach (var item in def.PropertyDef.Attributes)
                    {
                        list.Add(item.Attribute);
                    }

                return list.ToArray();
            }
        }

        protected IDictionary<string, ObjectDef> _properties;
        protected IEnumerable<IInterfaceDef> _interfaces;

        protected MetaObject()
        {
            _properties = new Dictionary<string, ObjectDef>();
            _interfaces = new List<IInterfaceDef>();
        }

        public static MetaObject Create(IInterfaceDef idef)
        {
            return Create(new[] { idef });
        }
        public static MetaObject Create(IEnumerable<IInterfaceDef> interfaces)
        {
            var obj = new MetaObject();
            obj._interfaces = interfaces;

            foreach (var idef in interfaces)
            {
                foreach (var prop in idef.Properties)
                {
                    obj._properties.Add(prop.Name, new ObjectDef()
                    {
                        Type = prop.PropertyType.Type,
                        Interface = idef,
                        PropertyDef = prop,
                    });

                }
            }

            return obj;

        }
        public static MetaObject Create(IEnumerable<IPropertyDef> props)
        {
            var obj = new MetaObject();

            foreach (var prop in props)
            {
                obj._properties.Add(prop.Name, new ObjectDef()
                {
                    Type = prop.PropertyType.Type,
                    PropertyDef = prop,
                });

            }
            return obj;
        }

        public IEnumerable<IInterfaceDef> InterfaceDefs()
        {
            return _interfaces;
        }
        public IEnumerable<IPropertyDef> PropertyDefs()
        {
            return _properties.Select(p => p.Value.PropertyDef);
        }

        #region DynamicObject overrides

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return _properties.Select(kvp=>kvp.Key);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            ObjectDef def;
            if (_properties.TryGetValue(binder.Name, out def))
                if ((value==null && !def.Type.IsValueType) || (value.GetType()==def.Type))
                {
                    def.Object = value;
                    return true;
                }
            return false;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            ObjectDef def;
            if (_properties.TryGetValue(binder.Name, out def))
            {
                result = def.Object;
                return true;
            }
            result = null;
            return false;
        }

        #endregion

        #region IDictionary

        public void Add(string key, object value)
        {
            //not allowed
            throw new NotImplementedException();
        }

        public bool ContainsKey(string key)
        {
            return _properties.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { return _properties.Keys; }
        }

        public bool Remove(string key)
        {
            //not allowed
            return false;
        }

        public bool TryGetValue(string key, out object value)
        {
            ObjectDef def;
            if (_properties.TryGetValue(key,out def))
            {
                value = def.Object;
                return true;
            }
            value = null;
            return false;              
        }

        public ICollection<object> Values
        {
            get { return _properties.Select(kvp => kvp.Value.Object).ToList(); }
        }

        public object this[string key]
        {
            get
            {
                return _properties[key].Object;
            }
            set
            {
                ObjectDef def;
                if (_properties.TryGetValue(key, out def))
                    if ((value == null && !def.Type.IsValueType) || (value.GetType() == def.Type))
                    {
                        def.Object = value;
                        return;
                    }
                    else
                        throw new ArgumentException("Member type mismatch!");

                throw new KeyNotFoundException();
            }
        }

        public void Add(KeyValuePair<string, object> item)
        {
            //not allowed
            throw new NotImplementedException();
        }

        public void Clear()
        {
            //not allowed
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return _properties.ContainsKey(item.Key);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return _properties.Count; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            //not allowed
            return false;
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _properties.Select(kvp=>new KeyValuePair<string,object>(kvp.Key,kvp.Value.Object)).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _properties.Select(kvp => new KeyValuePair<string, object>(kvp.Key, kvp.Value.Object)).GetEnumerator();
        }
        #endregion

        #region ICustomTypeDescriptor

        public AttributeCollection GetAttributes()
        {
            return TypeDescriptor.GetAttributes(this, true);
        }

        public string GetClassName()
        {
            return TypeDescriptor.GetClassName(this, true);
        }

        public string GetComponentName()
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

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return GetProperties();
        }

        public PropertyDescriptorCollection GetProperties()
        {
            var properties = _properties
                .Select(kvp => new DynamicPropertyDescriptor(this,
                    kvp.Key, kvp.Value.Type, kvp.Value.Attributes.Value));
            return new PropertyDescriptorCollection(properties.ToArray());
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            return this;
        }
        #endregion

        #region DynamicPropertyDescriptor

        protected class DynamicPropertyDescriptor : PropertyDescriptor
        {
            private MetaObject metaObject;
            private Type propertyType;

            public DynamicPropertyDescriptor(MetaObject businessObject,
                string propertyName, Type propertyType, Attribute[] propertyAttributes)
                : base(propertyName, propertyAttributes)
            {
                this.metaObject = businessObject;
                this.propertyType = propertyType;
            }

            public override bool CanResetValue(object component)
            {
                return true;
            }

            public override object GetValue(object component)
            {
                return metaObject._properties[Name].Object;
            }

            public override void ResetValue(object component)
            {
            }

            public override void SetValue(object component, object value)
            {
                metaObject._properties[Name].Object = value;
            }

            public override bool ShouldSerializeValue(object component)
            {
                return false;
            }

            public override Type ComponentType
            {
                get { return typeof(MetaObject); }
            }

            public override bool IsReadOnly
            {
                get { return false; }
            }

            public override Type PropertyType
            {
                get { return propertyType; }
            }
        } 

        #endregion

    }

    public class MetaObjectConverter : ExpandableObjectConverter
    {

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is IMetaObject)
            {
                return "Meta data";
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
