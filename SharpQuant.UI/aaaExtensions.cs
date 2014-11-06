using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpQuant.UI
{
    public static class aaaExtensions
    {
        public static string QualifiedName(this Type type)
        {
            string[] parts = type.AssemblyQualifiedName.Split(',');

            return string.Format("{0},{1}", parts);
        }
    }
}
