using Konata.Core.Base;
using Konata.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Konata.Core
{
    /// <summary>
    /// 全局对象加载/卸载管理器
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

        private static void Release()
        {
            instance = null;
        }

        public bool IsLoadingOrUnloading
        {
            get => true;
        }

        public bool IsCoreLoaded { get; private set; } = false;

        //所有加载的程序集
        private readonly Dictionary<string, Assembly> loadedassemblies = new Dictionary<string, Assembly>();
        //对应程序集下的所有组件类型
        private readonly UnOrderMultiMap<string, Type> loadedcomponenttypes = new UnOrderMultiMap<string, Type>();
        //对应程序集下的所有事件类型
        private readonly UnOrderMultiMap<string, Type> loadedeventtypes = new UnOrderMultiMap<string, Type>();
        //对应程序集下的所有服务类型
        private readonly UnOrderMultiMap<string, Type> loadedservicetypes = new UnOrderMultiMap<string, Type>();
        //对应程序集下的所有实体类型
        private readonly UnOrderMultiMap<string, Type> loadedentitytypes = new UnOrderMultiMap<string, Type>();

        public void LoadNewAssembly(string name,Assembly assembly)
        {
            if (this.loadedassemblies.ContainsKey(name))
            {
                throw new TypeLoadException($"Target assembly {name} already exist");
            }
            this.loadedassemblies[name] = assembly;

            UnOrderMultiMap<Type, Type> tempAttribute = new UnOrderMultiMap<Type, Type>();

            //读取该程序集中所有拥有继承于BaseAttribute的特性的特性标签
            foreach (Type type in assembly.GetTypes())
            {
                Attribute attribute = type.GetCustomAttributes(typeof(BaseAttribute)).FirstOrDefault();
                if (attribute != null)
                {
                    tempAttribute.Add((attribute as BaseAttribute).AttributeType,type);
                }
                //实体对象无特性标签[无意义]但还是先读取用于之后的插件卸载
                if(type.IsSubclassOf(typeof(Entity)))
                {
                    this.loadedentitytypes[name].Add(type);
                }
            }


            //记录该程序集的所有事件类型
            //不包括核心事件
            if (tempAttribute.ContainsKey(typeof(EventAttribute)))
            {
                this.loadedeventtypes[name] = tempAttribute[typeof(EventAttribute)];
                //将事件注册并初始化到事件管理器
                EventManager.Instance.LoadNewEvent(name, tempAttribute[typeof(EventAttribute)]);
            }

            //记录该程序集的所有组件类型
            if (tempAttribute.ContainsKey(typeof(ComponentAttribute)))
            {
                this.loadedcomponenttypes[name] = tempAttribute[typeof(ComponentAttribute)];
            }

            //记录该程序集的所有服务类型
            if (tempAttribute.ContainsKey(typeof(ServiceAttribute)))
            {
                this.loadedservicetypes[name] = tempAttribute[typeof(ServiceAttribute)];
                ServiceManager.Instance.LoadServices(tempAttribute[typeof(ServiceAttribute)]);
            }



            /*
             *程序集类型注册完毕,准备二次实例化等操作
             *事件:应实现IEvent接口,如果仅为标识可设置OnlySymbol,此时管理器不会加载其IEvent接口
             *组件:应实现ILoad,IStart接口并继承自Component
             *实体:继承自Entity,如果有实体被初始化时需要执行的方法 可以选择实现ILoad接口
             *服务:应实现ILoad IDisposable接口
             *针对组件/实体 建议重写Dispose方法用于对象卸载时正确释放
             *根据框架定义：
             *1.实例化事件并进行注册[事件作为组件/服务/实体/跨插件之间的通讯中间层优先注册]
             *2.实例化服务[服务作为对应程序集单例唯一供应者，其将被直接实例化并执行ILoad方法]
             *针对实体与组件：
             *当实体/组件被创建时将会尝试执行ILoad接口方法进行初始化
             *当组件创建完毕并挂载到实体后 将会尝试执行IStart接口方法标记其正式启用
             *针对生命周期结束：
             *对于组件,在其被remove的时候将会执行Dispose用于类型再循环
             *对于实体,同理组件，默认建议执行Dispose用于再循环
             *对于服务：当对应程序集被卸载时，将会调用其IDisposable接口用于卸载
             */

        }

        /// <summary>
        /// 装载核心程序集
        /// 仅可执行一次
        /// </summary>
        /// <param name="assembly"></param>
        public void LoadCoreAssembly(Assembly assembly)
        {
            if (this.IsCoreLoaded)
            {
                return;
            }

            UnOrderMultiMap<Type, Type> tempAttribute = new UnOrderMultiMap<Type, Type>();

            foreach (Type type in assembly.GetTypes())
            {
                Attribute attribute = type.GetCustomAttributes(typeof(BaseAttribute)).FirstOrDefault();
                if (attribute != null)
                {
                    tempAttribute.Add((attribute as BaseAttribute).AttributeType, type);
                }
            }

            if (tempAttribute.ContainsKey(typeof(EventAttribute)))
            {
                EventManager.Instance.LoadNewEvent("Core",tempAttribute[typeof(EventAttribute)]);
            }

            if (tempAttribute.ContainsKey(typeof(CoreEventAttribute)))
            {
                EventManager.Instance.LoadCoreEvent(tempAttribute[typeof(CoreEventAttribute)]);
            }

            if (tempAttribute.ContainsKey(typeof(ServiceAttribute)))
            {
                ServiceManager.Instance.LoadServices(tempAttribute[typeof(ServiceAttribute)]);
            }

            this.IsCoreLoaded = true;
        }

        /// <summary>
        /// 卸载指定程序集
        /// </summary>
        /// <param name="name"></param>
        public void UnloadAssembly(string name)
        {
            //卸载程序集
            if (!this.loadedassemblies.ContainsKey(name))
            {
                throw new TypeUnloadedException($"Target assembly {name} doesn't exist");
            }
            this.loadedassemblies.Remove(name);

            //卸载所有组件对象
            if (this.loadedcomponenttypes.ContainsKey(name))
            {
                foreach (Type type in this.loadedcomponenttypes[name])
                {
                    Root.Instance.RemoveEntities(type, false);
                }
            }
            this.loadedcomponenttypes.Remove(name);
            //卸载所有实体对象
            if (this.loadedentitytypes.ContainsKey(name))
            {
                foreach (Type type in this.loadedentitytypes[name])
                {
                    Root.Instance.RemoveEntities(type, false);
                }
            }
            this.loadedentitytypes.Remove(name);
            //卸载所有服务
            if (this.loadedservicetypes.ContainsKey(name))
            {
                foreach (Type type in this.loadedservicetypes[name])
                {
                    ServiceManager.Instance.RemoveService(type);
                }
            }
            this.loadedservicetypes.Remove(name);
            //卸载所有事件
            if (this.loadedeventtypes.ContainsKey(name))
            {
                EventManager.Instance.UnloadAssembly(name);
            }
            this.loadedeventtypes.Remove(name);
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
