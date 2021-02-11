using System;
using System.Reflection;
using System.Collections.Generic;

namespace Konata.Utils
{
    public static class Reflection
    {
        public static IEnumerable<Type> GetClassesByAttribute<T>()
              where T : Attribute
        {
            var list = new List<Type>();
            var types = typeof(T).Assembly.GetTypes();

            foreach (var type in types)
            {
                if (type?.GetCustomAttribute<T>() != null)
                {
                    list.Add(type);
                }
            }

            return list;
        }

        public static IEnumerable<Type> GetChildClasses<T>()
        {
            var list = new List<Type>();
            var types = typeof(T).Assembly.GetTypes();

            foreach (var type in types)
            {
                if (type.BaseType == typeof(T))
                {
                    list.Add(type);
                }
            }

            return list;
        }

        public static IEnumerable<Type> GetChildClasses(Type t)
        {
            var list = new List<Type>();
            var types = t.Assembly.GetTypes();

            foreach (var type in types)
            {
                if (type.BaseType == t)
                {
                    list.Add(type);
                }
            }

            return list;
        }
    }
}
