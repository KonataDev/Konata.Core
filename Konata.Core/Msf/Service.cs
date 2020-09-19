using System;
using System.Collections.Generic;

namespace Konata.Msf
{
    using Routine = Dictionary<string, Service>;

    internal abstract partial class Service
    {
        private static readonly Routine map = new Routine();

        static Service()
        {
            TouchServices();
        }

        /// <summary>
        /// 拉起指定的服務並開始執行特定任務
        /// </summary>
        /// <param name="core"></param>
        /// <param name="name"></param>
        /// <param name="method"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        internal static bool Run(Core core, string name, string method, params object[] args)
        {
            try
            {
                return map[name.ToLower()].OnRun(core, method, args);
            }
            catch (Exception e)
            {
                ((Core)args[0]).EmitError(1, e.StackTrace);
                return false;
            }
        }

        /// <summary>
        /// 拉起指定的服務並響應特定指令
        /// </summary>
        /// <param name="core"></param>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        internal static bool Handle(Core core, string name, params object[] args)
        {
            try
            {
                return map[name.ToLower()].OnHandle(core, args);
            }
            catch (Exception e)
            {
                ((Core)args[0]).EmitError(2, e.StackTrace);
                return false;
            }
        }

        internal string name;

        /// <summary>
        /// 需覆寫. 當服務被拉起時執行特定任務
        /// </summary>
        /// <param name="core"></param>
        /// <param name="method"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected abstract bool OnRun(Core core, string method, params object[] args);

        /// <summary>
        /// 需覆寫. 當服務被拉起時並響應特定指令
        /// </summary>
        /// <param name="core"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected abstract bool OnHandle(Core core, params object[] args);

        /// <summary>
        /// 在繼承基類的類構造方法内調用. 注冊服務路由
        /// </summary>
        /// <param name="name">服務名稱</param>
        /// <param name="service">自身實例</param>
        protected static void Register(string name, Service service)
        {
            service.name = name;
            map.Add(name.ToLower(), service);
        }
    }
}
