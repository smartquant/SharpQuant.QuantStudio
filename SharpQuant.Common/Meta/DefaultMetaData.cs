using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace SharpQuant.Common.Meta
{
    public static class DefaultMetaData
    {
        //This is used for the type converter
        public static IDictionary<string, IPropertyType> PropertyTypes { get; set; }
        public static IDictionary<string, IAttributeDef> AttributeDefs { get; set; }

        static DefaultMetaData()
        {
            InitDefaultPropertyTypes();
            InitDefaultAttributes();
        }

        static void InitDefaultPropertyTypes()
        {
            PropertyTypes = new Dictionary<string, IPropertyType>()
            {
                {"string",new PropertyType(){Id = 1, CODE = "string", QualifiedName="System.String"}},
                {"boolean",new PropertyType(){Id = 2, CODE = "boolean", QualifiedName="System.Boolean"}},
                {"byte",new PropertyType(){Id = 3, CODE = "byte", QualifiedName="System.Byte"}},
                {"int",new PropertyType(){Id = 4, CODE = "int", QualifiedName="System.Int32"}},
                {"long",new PropertyType(){Id = 5, CODE = "long", QualifiedName="System.Int64"}},
                //{"int32",new PropertyType(){ID = 4, CODE = "int32", QualifiedName="System.Int32"}},
                //{"int64",new PropertyType(){ID = 5, CODE = "int64", QualifiedName="System.Int64"}},
                {"double",new PropertyType(){Id = 6, CODE = "double", QualifiedName="System.Double"}},
                {"decimal",new PropertyType(){Id = 7, CODE = "decimal", QualifiedName="System.Decimal"}},
                {"guid",new PropertyType(){Id = 8, CODE = "guid", QualifiedName="System.Guid"}},

            };
            //make them read only so we cannot accidentally change them in PropertyGrid
            foreach(var pt in PropertyTypes.Values)
            {
                TypeDescriptor.AddAttributes(pt, new ReadOnlyAttribute(true));
            }
        }

        static void InitDefaultAttributes()
        {
            AttributeDefs = new Dictionary<string, IAttributeDef>()
            {
                {"required", new AttributeDef(){Id=1,CODE="required", Name = "Required", QualifiedName=typeof(RequiredAttribute).AssemblyQualifiedName}},
                {"browsable", new AttributeDef(){Id=2,CODE="browsable", Name = "Browsable", QualifiedName=typeof(BrowsableAttribute).AssemblyQualifiedName,ValueType=TypeEnum.Boolean,Value="True"}},
 
            };
        }
    }
}
