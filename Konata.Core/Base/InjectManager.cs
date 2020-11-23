using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Konata.Core.Base
{
    /// <summary>
    /// 全局对象管理器
    /// </summary>
    public sealed class InjectManager
    {
        private static InjectManager instance;

        public static InjectManager Instance
        {
            get => instance ?? (instance = new InjectManager());
        }

        private InjectManager()
        {

        }

        public static void Release()
        {
            instance = null;
        }

        //所有加载的程序集
        private readonly Dictionary<string, Assembly> loadedassemblies = new Dictionary<string, Assembly>();

        //所有加载的程序集中组件的类型
        private readonly Dictionary<string, List<Type>> loadedcomponenttypes = new Dictionary<string, List<Type>>();

        private readonly Dictionary<string,List<Type>> loaded



        public void Add(string name,Assembly assembly)
        {
            this.loadedassemblies[name] = assembly;

        }


        public Assembly GetAss(string name)
        {
            return this.loadedassemblies[name];
        }

        public Assembly[] GetAllAss()
        {
            return this.loadedassemblies.Values.ToArray();
        }


        ~InjectManager()
        {

        }
    }
}
