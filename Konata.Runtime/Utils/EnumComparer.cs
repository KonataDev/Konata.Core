using System;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace Konata.Runtime.Utils
{
    /// <summary>
    /// 针对未实现IEqualityComparer的枚举
    /// <para>避免枚举类型进行比较时产生额外gc</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EnumComparer<T> : IEqualityComparer<T> where T : struct
    {
        public bool Equals(T first, T second)
        {
            var firstParam = Expression.Parameter(typeof(T), "first");
            var secondParam = Expression.Parameter(typeof(T), "second");
            var equalExpression = Expression.Equal(firstParam, secondParam);

            return Expression.Lambda<Func<T, T, bool>>
                (equalExpression, firstParam, secondParam).
                Compile().Invoke(first, second);
        }

        public int GetHashCode(T instance)
        {
            var parameter = Expression.Parameter(typeof(T), "instance");
            var convertExpression = Expression.Convert(parameter, typeof(int));

            return Expression.Lambda<Func<T, int>>
                (convertExpression, parameter).
                Compile().Invoke(instance);
        }
    }
}
