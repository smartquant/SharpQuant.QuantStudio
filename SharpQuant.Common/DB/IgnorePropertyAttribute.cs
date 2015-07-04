using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpQuant.Common.DB
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnorePropertyAttribute : Attribute
    {
        public IgnorePropertyAttribute()
        {
        }
    }
}
