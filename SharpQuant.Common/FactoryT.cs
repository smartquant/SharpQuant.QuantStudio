using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;
using System.ComponentModel;


namespace SharpQuant.Common
{
    /// <summary>
    /// Class to inject or retrieve Settings from a class' properties for serialization
    /// </summary>
    public class Factory<T> where T:class
    {
        IResolver _resolver;

        public Factory(IResolver resolver)
        {
            _resolver = resolver;
        }

        public T Create(string qualifiedName)
        {
            Type t = Type.GetType(qualifiedName);
            var job = _resolver.Resolve(t) as T;
            return job;
        }

        public T Create(string qualifiedName, string settings)
        {
            Type t = Type.GetType(qualifiedName);
            var job = _resolver.Resolve(t) as T;
            InjectSettings(job, SettingsDictionary.Deserialize(settings));

            return job;
        }

        public T Create(Type t, SettingsDictionary settings)
        {
            var job = _resolver.Resolve(t) as T;
            InjectSettings(job, settings);

            return job;
        }


        public static SettingsDictionary ExtractSettings(T obj)
        {
            SettingsDictionary dict = new SettingsDictionary();
            
            Type t = obj.GetType();

            var properties = t.GetProperties();
            foreach (var prop in properties.Where(p=>p.DeclaringType==t && p.CanWrite))
            {
                object value = prop.GetValue(obj, null);
                if (value != null) //check for nullable properties
                    dict.Add(prop.Name, value.ToString());
                else
                    dict.Add(prop.Name, null);
            }
            return dict;
        }

        public static void InjectSettings(T obj, SettingsDictionary settings)
        {
            Type t = obj.GetType();
            var properties = t.GetProperties();
            foreach (var prop in properties.Where(p => p.DeclaringType == t && p.CanWrite))
            {
                if (settings.ContainsKey(prop.Name))
                    prop.SetValue(obj, settings.GetOrSet(prop.Name, prop.PropertyType), null);
            }
        }


    }
}
