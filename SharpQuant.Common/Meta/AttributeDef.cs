using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace SharpQuant.Common.Meta
{
    public interface IAttributeDef
    {
        string Name { get; }
        string QualifiedName { get; }
        TypeEnum ValueType { get; }
        string Value { get; }
        Attribute Attribute { get; }
    };
    
    [TypeConverter(typeof(AttributeDefConverter))]
    public class AttributeDef : IAttributeDef
    {

        public static Func<IAttributeDef, Attribute> AttributeFactory { get; set; }

        TypeEnum _valueType;
        string _value;
        Lazy<Attribute> _attribute;

        [Browsable(false)]
        [Category("@ID")]
        public long Id { get; internal set; }
        [Category("@ID")]
        public string CODE { get; set; }


        public string Name { get; set; }
        public string QualifiedName { get; set; }

        public TypeEnum ValueType
        {
            get { return _valueType; }
            set
            {
                _valueType = value;
                if (_attribute.IsValueCreated)
                    _attribute = new Lazy<Attribute>(() => AttributeFactory(this));
            }
        }
        public string Value
        {
            get { return _value; }
            set
            {
                _value = value;
                if (_attribute.IsValueCreated)
                    _attribute = new Lazy<Attribute>(() => AttributeFactory(this));
            }
        }
        public Attribute Attribute 
        { 
            get { return _attribute.Value; }
            set { _attribute = new Lazy<Attribute>(() => value); }
        }

        public AttributeDef()
        {
            _attribute = new Lazy<Attribute>(() => GetAttribute(this));
        }

        // simple default attribute factory
        // ugly: for an unknown reason to create the type one needs to give a fully qualified name, if it comes from System.ComponenentModel ?!?
        // remedy: replace with a factory which basically takes only a code and uses the value as a serialized parameterization

        static Attribute GetAttribute(IAttributeDef def)
        {
            var t = Type.GetType(def.QualifiedName);

            if (string.IsNullOrEmpty(def.Value))
            {
                return (t.GetConstructor(Type.EmptyTypes) != null) ? (Attribute)Activator.CreateInstance(t, new object[0])
                    : (Attribute)Activator.CreateInstance(t);
            }
            else
            {
                var tt = Type.GetType(string.Format("System.{0}", def.ValueType));
                object value = tt == typeof(Type) ? Type.GetType(def.Value) : Convert.ChangeType(def.Value, tt);

                return (Attribute)Activator.CreateInstance(t, new object[] { value });
            }

        }

    }

    class AttributeDefConverter : ExpandableObjectConverter
    {

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && typeof(IAttributeDef).IsAssignableFrom(value.GetType()))
            {
                IAttributeDef val = (IAttributeDef)value;
                return string.Format("{0}: {1}", val.Name, val.Value);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

    }

    class AttributeDefListConverter : ExpandableObjectConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && typeof(ICollection<IAttributeDef>).IsAssignableFrom(value.GetType()))
            {
                return "Attribute definitions";
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class AttributeDefList : List<IAttributeDef>, ICustomTypeDescriptor
    {
        public AttributeDefList() : base() { }
        public AttributeDefList(IEnumerable<IAttributeDef> defs) : base(defs) { }

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


        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            return GetProperties();
        }

        public PropertyDescriptorCollection GetProperties()
        {
            return new PropertyDescriptorCollection(
                this.Select((p, i) => new CustomPropertyDescriptor(p, i)).ToArray());
        }

        #endregion

        #region PropertyDescriptor

        class CustomPropertyDescriptor : PropertyDescriptor
        {
            IAttributeDef _prop;

            public CustomPropertyDescriptor(IAttributeDef prop, int index)
                : base("#" + index.ToString(), null)
            {
                _prop = prop;
            }

            public override bool CanResetValue(object component)
            {
                return true;
            }

            public override Type ComponentType
            {
                get { return typeof(PropertyDefList); }
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
                    return _prop.Name;
                }
            }
        }

        #endregion
    }

}
