using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace Konata.Core.Utils;

public static class Reflection
{
    public static void EnumAttributes<TAttr>(Action<Type, TAttr> action)
        where TAttr : Attribute
    {
        foreach (var i in typeof(TAttr).Assembly.GetTypes())
        {
            // Filter the attribute
            var j = i?.GetCustomAttribute<TAttr>();

            // Call action
            if (j != null)
            {
                action(i, j);
            }
        }
    }

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

    public static IEnumerable<Type> GetClassesByAttribute(Type t)
    {
        var list = new List<Type>();
        var types = t.Assembly.GetTypes();

        foreach (var type in types)
        {
            if (type?.GetCustomAttribute(t) != null)
            {
                list.Add(type);
            }
        }

        return list;
    }

    public static IEnumerable<Type> GetChildClasses<Ttype>()
        => GetChildClasses(typeof(Ttype));

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

    public static IEnumerable<Type> GetChildClasses<Ttype>(Assembly a)
        => GetChildClasses(typeof(Ttype), a);

    public static IEnumerable<Type> GetChildClasses(Type t, Assembly a)
    {
        var list = new List<Type>();
        var types = a.GetTypes();

        foreach (var type in types)
        {
            if (type.BaseType == t)
            {
                list.Add(type);
            }
        }

        return list;
    }

    public static IEnumerable<Type> GetImplementedClasses<Ttype>(Assembly a)
        => GetImplementedClasses(typeof(Ttype), a);

    public static IEnumerable<Type> GetImplementedClasses(Type t, Assembly a)
        => a.GetTypes().Where(t.IsAssignableFrom);

    /// <summary>
    /// Deep clone object
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t"></param>
    /// <returns></returns>
    [Obsolete]
    public static T Clone<T>(T t)
    {
        if (t is string || t.GetType().IsValueType)
        {
            return t;
        }

        var type = t.GetType();

        var retval = Activator.CreateInstance(type);
        {
            FieldInfo[] fields = type.GetFields(BindingFlags.Public |
                                                BindingFlags.NonPublic |
                                                BindingFlags.Instance |
                                                BindingFlags.Static);

            foreach (FieldInfo field in fields)
            {
                try
                {
                    field.SetValue(retval, Clone(field.GetValue(t)));
                }
                catch
                {
                    /* Always ignore exceptions */
                }
            }
        }

        return (T) retval;
    }
}
