using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuant.Common.Meta
{
    public interface IPropertyDef
    {
        string Name { get; }
        IPropertyType PropertyType { get; }

        //ComponentModel
        string Category { get; }
        string DisplayName { get; }
        string DisplayFormat { get; } // {0:yyyyMMdd}
        string Description { get; }
        ICollection<IAttributeDef> Attributes { get; }
    }

    [TypeConverter(typeof(PropertyDefConverter))]
    public class PropertyDef : IPropertyDef
    {
        [Browsable(false)]
        public long Id { get; internal set; }
        [Category("@ID")]
        public string CODE { get; set; }
        [Category("@ID")]
        public string Name { get; set; }

        [Category("@Type")]
        public IPropertyType PropertyType { get; set; }
        //FK to PropertyType
        private long _propertyTypeID;
        [Browsable(false)]
        public long PropertyTypeID
        {
            get { var pt = PropertyType as PropertyType; return pt == null ? _propertyTypeID : pt.Id; }
            set { _propertyTypeID = value; }
        }

        [Category("ComponentModel")]
        public string Category { get; set; }
        [Category("ComponentModel")]
        public string DisplayName { get; set; }
        [Category("ComponentModel")]
        public string DisplayFormat { get; set; } // {0:yyyyMMdd}
        [Category("ComponentModel")]
        public string Description { get; set; }

        ICollection<IAttributeDef> _attributes;
        [TypeConverter(typeof(AttributeDefListConverter))]
        public ICollection<IAttributeDef> Attributes
        {
            get { return _attributes ?? (_attributes = new List<IAttributeDef>());}
            set { _attributes = value; }
        }

        //FK to Interface
        [Browsable(false)]
        public IInterfaceDef InterfaceDef { get; set; }

        private long _interfaceID;
        [Browsable(false)]
        public long InterfaceDefID
        {
            get { var idef = InterfaceDef as InterfaceDef; return idef == null ? _interfaceID : idef.Id; }
            set { _interfaceID = value; }
        }

    }

    class PropertyDefConverter : ExpandableObjectConverter
    {

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && typeof(IPropertyDef).IsAssignableFrom(value.GetType()))
            {
                IPropertyDef val = (IPropertyDef)value;
                return val.PropertyType != null ? string.Format("{0}: {1}", val.Name, val.PropertyType.CODE)
                    : val.Name;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

    }

    class PropertyDefListConverter : ExpandableObjectConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && typeof(ICollection<IPropertyDef>).IsAssignableFrom(value.GetType()))
            {
                return "Property definitions";
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class PropertyDefList : List<IPropertyDef>, ICustomTypeDescriptor
    {
        public PropertyDefList() : base() { }
        public PropertyDefList(IEnumerable<IPropertyDef> defs) : base(defs) { }

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
                this.Select((p,i)=>new CustomPropertyDescriptor(p, i)).ToArray());
        }

        #endregion

        #region PropertyDescriptor

        class CustomPropertyDescriptor : PropertyDescriptor
        {
            IPropertyDef _prop;
       
            public CustomPropertyDescriptor(IPropertyDef prop, int index):base("#"+index.ToString(), null )
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


    public class PropertyDefValidator : SharpQuant.Common.Validation.ValidatorBase<PropertyDef>
    {
        public PropertyDefValidator()
        {
            base.UseAttributes = true;
            base.AddPropertyRule<IPropertyType>(pd => pd.PropertyType, (pd, pt) => pt != null, "PropertyType must be set!");
        }
    }

}
