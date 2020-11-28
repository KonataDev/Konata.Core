using System;
using System.Diagnostics;

namespace Konata.Runtime.Utils
{
    public static class ObjectHelper
    {
        /// <summary>
        /// 两个对象进行交换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        public static void Swap<T>(ref T t1, ref T t2)
        {
            T t3 = t1;
            t1 = t2;
            t2 = t3;
        }

        /// <summary>
        /// 获取方法调用链指定深度方法名
        /// </summary>
        /// <param name="index">
        /// 深度层级
        /// <para>深度定义：</para>
        /// <para>0:该函数本身,1:调用该函数的调用方,2:调用方的调用方...</para>
        /// </param>
        /// <param name="onlymethodname">是否仅返回方法名称</param>
        /// <returns></returns>
        public static string GetCurrentMethodTraceName(int index = 2, bool onlymethodname = true)
        {
            if (index < 0)
            {
                return null;
            }
            try
            {
                StackTrace trace = new StackTrace();
                string methodName = trace.GetFrame(index).GetMethod().Name;
                if (!onlymethodname)
                {
                    string className = trace.GetFrame(index).GetMethod().DeclaringType.ToString();
                    methodName = className + "." + methodName;
                }
                return methodName;
            }
            catch
            {
                return null;
            }
        }
    }
}
