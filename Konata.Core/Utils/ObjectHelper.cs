using System;
using System.Collections.Generic;
using System.Text;

namespace Konata.Core.Utils
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
	}
}
