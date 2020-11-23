using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.DependencyModel.Resolution;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace Konata.Console
{
    /// <summary>
    /// <para>解决以插件形式加载程序集时插件之间相互引用问题</para>
    /// <para>使用AssemblyLoadContext以解决动态热插拔问题</para>
    /// <para>适用于:</para>
    /// <para>[.net Core 3.1 3.0 2.2 2.1 2.0 1.1 1.0]</para>
    /// <para>[.net 5]</para>
    /// <para>[Xamarin.Android 7.1+]</para>
    /// <para>[Xamarin.iOS 10.8+]</para>
    /// </summary>
    public static class AssemblyLoader
    {
        private static readonly ICompilationAssemblyResolver AssemblyResolver;
        private static readonly ConcurrentDictionary<string, CompilationLibrary> DependencyDLL;

        static AssemblyLoader()
        {
            AssemblyLoadContext.Default.Resolving += Default_Resolving;
            AssemblyResolver = new CompositeCompilationAssemblyResolver(
                new ICompilationAssemblyResolver[]{
                    new AppBaseCompilationAssemblyResolver(AppDomain.CurrentDomain.BaseDirectory),
                    new ReferenceAssemblyPathResolver(),
                    new PackageCompilationAssemblyResolver()
                });
            DependencyDLL = new ConcurrentDictionary<string, CompilationLibrary>();
        }

        private static Assembly Default_Resolving(AssemblyLoadContext assemblyLoadContext, AssemblyName assemblyName)
        {
            if (DependencyDLL.ContainsKey(assemblyName.Name))
            {
                var compilationLibrary = DependencyDLL[assemblyName.Name];
                var assemblies = new List<string>();
                if (AssemblyResolver.TryResolveAssemblyPaths(compilationLibrary, assemblies) && assemblies.Count > 0)
                {
                    var assembly = assemblyLoadContext.LoadFromAssemblyPath(assemblies[0]);
                    FindDependency(assembly);
                    return assembly;
                }
            }
            throw new NullReferenceException($"Warning:assembly {assemblyName.Name} not loaded yet,make sure {assemblyName.Name} loaded before you load current assembly");
            //return null;
        }

        /// <summary>
        /// 加载当前目录下指定DLL
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public static Assembly LoadAssembly(string assemblyName)
        {
            string assemblyFileName = AppDomain.CurrentDomain.BaseDirectory + assemblyName + ".dll";
            Assembly assembly = AppDomain.CurrentDomain
                .GetAssemblies()
                .FirstOrDefault(a => a.FullName?.Split(',')[0] == assemblyName) 
                ?? AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyFileName);
            FindDependency(assembly);
            return assembly;
        }

        /// <summary>
        /// 加载指定目录下的DLL
        /// </summary>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public static Assembly LoadAssemblyFromDiffPath(string absassemblyPath,string assemblyName)
        {
            string assemblyFileName = absassemblyPath + assemblyName + ".dll";
            Assembly assembly = AppDomain.CurrentDomain
                .GetAssemblies()
                .FirstOrDefault(a => a.FullName?.Split(',')[0] == assemblyName)
                ?? AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyFileName);
            FindDependency(assembly);
            return assembly;
        }
        /// <summary>
        /// 从流中加载DLL
        /// </summary>
        /// <param name="dllbytes"></param>
        /// <param name="assemblyName"></param>
        /// <returns></returns>
        public static Assembly LoadAssemblyFromStream(Stream dllbytes,string assemblyName="")
        {
            Assembly assembly = AppDomain.CurrentDomain
                .GetAssemblies()
                .FirstOrDefault(a => a.FullName?.Split(',')[0] == assemblyName)
                ?? AssemblyLoadContext.Default.LoadFromStream(dllbytes);
            FindDependency(assembly);
            return assembly;
        }

        private static void FindDependency(Assembly assembly)
        {
            DependencyContext dependencyContext = DependencyContext.Load(assembly);
            if (dependencyContext != null)
            {
                foreach (var compilationLibrary in dependencyContext.CompileLibraries)
                {
                    if (!DependencyDLL.ContainsKey(compilationLibrary.Name)
                    && !AppDomain.CurrentDomain.GetAssemblies().Any(a => a.FullName.Split(',')[0] == compilationLibrary.Name))
                    {
                        RuntimeLibrary library = dependencyContext.RuntimeLibraries.FirstOrDefault(runtime => runtime.Name == compilationLibrary.Name);
                        var cb = new CompilationLibrary(
                            library.Type,
                            library.Name,
                            library.Version,
                            library.Hash,
                            library.RuntimeAssemblyGroups.SelectMany(g => g.AssetPaths),
                            library.Dependencies,
                            library.Serviceable);

                        DependencyDLL[library.Name] = cb;
                    }
                }
            }
        }
    }
}
