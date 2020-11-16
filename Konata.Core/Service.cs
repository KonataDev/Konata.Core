using System;
using System.Collections.Generic;
using Konata.Events;

namespace Konata
{
    using ServiceRoutine = Dictionary<string, Service>;

    public partial class Service : EventComponent
    {
        private readonly ServiceRoutine serviceRoutine;
        private readonly List<Events.EventHandler> serviceHandler;

        public Service(EventPumper eventPumper)
            : base(eventPumper)
        {
            TouchServices();
        }


        private EventParacel OnEvent(EventParacel eventParacel)
        {

        }

        /// <summary>
        /// 拉起指定的服務並開始執行特定任務
        /// </summary>
        /// <param name="core"></param>
        /// <param name="name"></param>
        /// <param name="method"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static bool Run(Core core, string name, string method, params object[] args)
        {
            try
            {
                return serviceRoutine[name.ToLower()].OnRun(core, method, args);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 拉起指定的服務並開始執行特定任務
        /// </summary>
        /// <param name="core"></param>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static bool Run(Core core, string name, params object[] args)
        {
            return Run(core, name, "", args);
        }

        /// <summary>
        /// 拉起指定的服務並響應特定指令
        /// </summary>
        /// <param name="core"></param>
        /// <param name="name"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static bool Handle(Core core, string name, params object[] args)
        {
            try
            {
                return serviceRoutine[name.ToLower()].OnHandle(core, args);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                Console.Error.WriteLine(e.StackTrace);
                return false;
            }
        }

        public string name;

        /// <summary>
        /// 需覆寫. 當服務被拉起時執行特定任務
        /// </summary>
        /// <param name="core"></param>
        /// <param name="method"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public bool OnRun(Core core, string method, params object[] args)
            => false;

        /// <summary>
        /// 需覆寫. 當服務被拉起時並響應特定指令
        /// </summary>
        /// <param name="core"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public bool OnHandle(Core core, params object[] args)
            => false;

        /// <summary>
        /// 在繼承基類的類構造方法内調用. 注冊服務路由
        /// </summary>
        /// <param name="name">服務名稱</param>
        /// <param name="service">自身實例</param>
        public void Register(string name, Service service)
        {
            service.name = name;
            serviceRoutine.Add(name.ToLower(), service);
        }
    }
}
