using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace SharpQuant.Common.Meta
{
    [TypeConverter(typeof(PropertyTypeConverter))]
    public interface IPropertyType
    {
        string CODE { get; }
        string QualifiedName { get; }
        Type Type { get; }
    };

    [TypeConverter(typeof(PropertyTypeConverter))]
    public class PropertyType : IPropertyType
    {

        [Browsable(false)]
        public long Id { get; internal set; }
        public string CODE { get; set; }

        string _qualifiedName;
        public string QualifiedName
        {
            get { return _qualifiedName; }
            set 
            { 
                _qualifiedName = value; 
                if (_type.IsValueCreated) 
                    _type = new Lazy<Type>(() => Type.GetType(_qualifiedName)); 
            }
        }

        public PropertyType()
        {
            _qualifiedName = "System.String";
            _type = new Lazy<Type>(() => Type.GetType(_qualifiedName));
        }

        Lazy<Type> _type;
        public Type Type { get { return _type.Value; } }

    }

    public class PropertyTypeConverter : ExpandableObjectConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(DefaultMetaData.PropertyTypes.Values.ToList());
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is IPropertyType)
            {
                IPropertyType val = (IPropertyType)value;
                return val.CODE;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string)) return true;
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string)
            {
                string CODE = (string)value;
                IPropertyType type;
                return DefaultMetaData.PropertyTypes.TryGetValue(CODE, out type) ? type : null;
            }
            return base.ConvertFrom(context, culture, value);
        }

    }
}
