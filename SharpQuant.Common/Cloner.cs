using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SharpQuant.Common
{

    public static class Cloner
    {
        /// <summary>
        /// Deep copy to object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toClone">The obj to clone.</param>
        /// <returns></returns>
        public static T DeepCopy<T>(this Object toClone)
        {
            using (var ms = new MemoryStream())
            {
                
                var bf = new BinaryFormatter();
                bf.Serialize(ms, toClone);
                ms.Position = 0;
                return (T)(bf.Deserialize(ms));
            }
        }
    }
}
