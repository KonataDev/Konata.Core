using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Konata.Core.Utils
{
    public static class DllHelper
    {
        private static IEnumerable<Assembly> GetDependentAssemblies(Assembly analyzedAssembly)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => GetNamesOfAssembliesReferencedBy(a)
                                    .Contains(analyzedAssembly.FullName));
        }

        /// <summary>
        /// 获取指定程序集的其他程序集的依赖项
        /// </summary>
        /// <param name="assembly"></param>
        /// <param name="IgnoreSystemAssembly">是否忽略系统级程序集依赖</param>
        /// <returns></returns>
        public static IEnumerable<string> GetNamesOfAssembliesReferencedBy(Assembly assembly,bool IgnoreSystemAssembly=true)
        {
            IEnumerable<string> asslist = assembly.GetReferencedAssemblies()
                .Select(assemblyName => assemblyName.FullName);
            if (IgnoreSystemAssembly)
            {
                return asslist.Where(assemblyName => !(assemblyName.StartsWith("System.") || assemblyName.StartsWith("Microsoft.")));
            }
            return asslist;
        }
    }
}
