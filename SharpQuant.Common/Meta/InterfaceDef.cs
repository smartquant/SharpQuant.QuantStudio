using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpQuant.Common.Meta
{
    public interface IInterfaceDef
    {
        string Name { get; }
        string Description { get; }
        List<IPropertyDef> Properties { get; }
    }

    public class InterfaceDef : IInterfaceDef
    {

        [Browsable(false)]
        [Category("@ID")]
        public long Id { get; internal set; }
        [Category("@ID")]
        public string CODE { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        List<IPropertyDef> _properties;

        [TypeConverter(typeof(PropertyDefListConverter))]
        public List<IPropertyDef> Properties
        {
            get { return _properties ?? (_properties = new List<IPropertyDef>()); }
            set { _properties = value; }
        }

    }

    class InterfaceDefConverter : ExpandableObjectConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is IInterfaceDef)
            {
                IInterfaceDef def = (IInterfaceDef)value;
                return def.Name;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
