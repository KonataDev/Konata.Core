using Konata.Core.Base;
using Konata.Core.Base.Event;
using Konata.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Konata.Core
{
    public class Eventer
    {
        public int CallCount { get; set; } = 0;
        public bool Enable { get; set; } = true;
        public IEvent Event { get; set; } = null;

        public string Name { get; set; } = "";

        public string Description { get; set; } = "";

        public EventRunType RunType { get; set; } = EventRunType.OnlySymbol;

        public WeakEvent<KonataEventArgs> Listener = new WeakEvent<KonataEventArgs>();

        public override string ToString()
        {
            return $"名称:{Name};启用中:{Enable};被订阅数:{Listener.Count};简介:{Description}";
        }
    }

    public class EventManager
    {
        private static EventManager instance;

        public static EventManager Instance
        {
            get => instance ?? (instance = new EventManager());
        }
        private  EventManager()
        {
            this.concurrent = TaskQueue.CreateGlobalQueue("EventRunnerConcurrent",50);
        }

        public void Release()
        {
            instance = null;
        }


        private TaskQueue concurrent = null;
        //面向插件等非核心事件的字典列表
        private readonly Dictionary<string, Eventer> commoneventlist = new Dictionary<string, Eventer>();
        private object commoneventlock = new object();
        //针对核心事件的字典列表,使用固定枚举类型加速执行事件
        private readonly Dictionary<CoreEventType, Eventer> coreeventlist = new Dictionary<CoreEventType, Eventer>(new EnumComparer<CoreEventType>());
        private object coreeventlock = new object();
        public bool CoreEventLoaded
        {
            get => (this.coreeventlist.Count > 0);
        }

        /// <summary>
        /// 注册普通事件
        /// </summary>
        /// <param name="assemblyname">程序集名称</param>
        /// <param name="types">事件类型</param>
        public void LoadNewEvent(string assemblyname, IList<Type> types)
        {
            if (String.IsNullOrEmpty(assemblyname) || types == null)
            {
                return;
            }
            lock (this.commoneventlock)
            {
                foreach (Type type in types)
                {
                    object attribute = type.GetCustomAttributes(typeof(EventAttribute), false).FirstOrDefault();
                    if (attribute == null)
                    {
                        throw new NullReferenceException("Event type find no attribute(should not be happened)");
                    }
                    EventAttribute eattr = (attribute as EventAttribute);
                    string eventname = eattr.EventType;
                    string totalname = assemblyname + "." + eventname;
                    if (this.commoneventlist.ContainsKey(totalname))
                    {
                        throw new ArgumentException($"Find same name event:even set assemblyheader ({totalname})");
                    }
                    IEvent obj = null;
                    if (!typeof(IEvent).IsAssignableFrom(type))
                    {
                        if (eattr.EventRunType != EventRunType.OnlySymbol)
                        {
                            throw new TypeLoadException($"Event {type.Name} not set IEvent Interface but not set as onlysymbol");
                        }

                    }
                    else
                    {
                        obj = (IEvent)Activator.CreateInstance(type);
                    }


                    this.commoneventlist[totalname] = new Eventer { Event = obj, RunType = eattr.EventRunType,Name=eattr.Name,Description=eattr.Description };
                }
            }
        }

        /// <summary>
        /// 注册核心事件-只能注册一次
        /// </summary>
        /// <param name="types"></param>
        public void LoadCoreEvent(IList<Type> types)
        {
            if (this.CoreEventLoaded)
            {
                throw new TypeLoadException("Core Events have been loaded!");
            }
            lock (this.coreeventlock)
            {
                foreach (Type type in types)
                {
                    object attribute = type.GetCustomAttributes(typeof(EventAttribute), false).FirstOrDefault();
                    if (attribute == null)
                    {
                        throw new NullReferenceException("Event type find no attribute(should not be happened)");
                    }
                    CoreEventAttribute eattr = (attribute as CoreEventAttribute);
                    CoreEventType eventtype = eattr.EventType;
                    if (this.coreeventlist.ContainsKey(eventtype))
                    {
                        throw new ArgumentException($"Find same type core event:even set assemblyheader ({eventtype})");
                    }
                    if (!type.IsAssignableFrom(typeof(IEvent)))
                    {
                        if (eattr.EventRunType != EventRunType.OnlySymbol)
                        {
                            throw new TypeLoadException($"Event {type.Name} not set IEvent Interface but not set as onlysymbol");
                        }
                    }

                    IEvent obj = (IEvent)Activator.CreateInstance(type);

                    this.coreeventlist[eventtype] = new Eventer { Event = obj, RunType = eattr.EventRunType, Name = eattr.Name, Description = eattr.Description };
                }
            }
        }

        /// <summary>
        /// 移除单个普通事件
        /// </summary>
        /// <param name="assemblyname">绝对名称(程序集名)</param>
        /// <param name="name">绝对名称(事件名)</param>
        private void UnloadCommonEvent(string assemblyname,string name)
        {
            string totalname = assemblyname + "." + name;
            lock (this.commoneventlock)
            {
                if (this.commoneventlist.ContainsKey(totalname))
                {
                    this.commoneventlist.Remove(totalname);
                }
            }
        }

        /// <summary>
        /// 移除单个/多个普通事件
        /// </summary>
        /// <param name="name">事件名称(部分匹配)</param>
        /// <param name="removeall">是否移除所有符合匹配名的事件</param>
        private bool UnloadCommonEvent(string name,bool removeall=false)
        {
            lock (this.commoneventlock)
            {
                string[] names = this.commoneventlist.Keys.Where(key => key.EndsWith(name)).ToArray();
                if (names.Length > 1 && !removeall)
                {
                    return false;
                }
                foreach (string truename in names)
                {
                    this.commoneventlist.Remove(truename);
                }
                return true;
            }
        }

        /// <summary>
        /// 移除指定程序集的所有事件
        /// </summary>
        /// <param name="name"></param>
        public bool UnloadAssembly(string assemblyname)
        {
            lock (this.commoneventlock)
            {
                string[] names = this.commoneventlist.Keys.Where(key => key.StartsWith(assemblyname)).ToArray();
                foreach (string truename in names)
                {
                    this.commoneventlist.Remove(truename);
                }
                return true;
            }
        }


        public bool RegisterListener(string eventname,Action<EventArgs> action)
        {
            lock (this.commoneventlock)
            {
                if(!this.commoneventlist.TryGetValue(eventname,out Eventer ever)||!ever.Enable)
                {
                    return false;
                }
                ever.Listener += action;
            }
            return true;
        }
        public bool RegisterListener(CoreEventType eventtype, Action<EventArgs> action)
        {
            lock (this.coreeventlock)
            {
                if (!this.coreeventlist.TryGetValue(eventtype, out Eventer ever) ||ever==null||!ever.Enable)
                {
                    return false;
                }
                ever.Listener += action;
            }
            return true;
        }
        public void RemoveListener(Action<EventArgs> action)
        {
            lock (this.commoneventlock)
            {
                foreach(Eventer ever in this.commoneventlist.Values)
                {
                    ever.Listener -= action;
                }
            }
        }


        public void RunEvent(string eventname,KonataEventArgs arg)
        {
            lock (this.commoneventlock)
            {
                if(this.commoneventlist.TryGetValue(eventname,out Eventer ever))
                {
                    if (ever.Enable)
                    {
                        if (ever.RunType == EventRunType.BeforeListener)
                            ever.Event?.Handle(arg);
                        ever.Listener.Invoke(arg);
                        if (ever.RunType == EventRunType.AfterListener)
                            ever.Event?.Handle(arg);
                    }
                }
            }
        }
        public void RunEvent(CoreEventType eventtype,KonataEventArgs arg)
        {
            lock (this.coreeventlock)
            {
                if (this.coreeventlist.TryGetValue(eventtype, out Eventer ever))
                {
                    if (ever.Enable)
                    {
                        if (ever.RunType == EventRunType.BeforeListener)
                            ever.Event?.Handle(arg);
                        ever.Listener.Invoke(arg);
                        if (ever.RunType == EventRunType.AfterListener)
                            ever.Event?.Handle(arg);
                    }
                }
            }
        }

        public async Task RunEventAsync(string eventname, KonataEventArgs arg)
        {
            Eventer ever = null;
            lock (this.commoneventlock)
            {
                if (!this.commoneventlist.TryGetValue(eventname, out ever))
                {
                    return;
                }
            }

            if (ever.RunType == EventRunType.BeforeListener)
                await this.concurrent.RunAsync(() => { ever.Event.Handle(arg); });
            await this.concurrent.RunAsync(() => { ever.Listener.Invoke(arg); });
            if (ever.RunType == EventRunType.AfterListener)
                await this.concurrent.RunAsync(() => { ever.Event.Handle(arg); });

        }

        public async Task RunEventAsync(CoreEventType eventtype, KonataEventArgs arg)
        {
            Eventer ever = null;
            lock (this.coreeventlock)
            {
                if (!this.coreeventlist.TryGetValue(eventtype, out ever))
                {
                    return;
                }
            }

            if (ever.RunType == EventRunType.BeforeListener)
                await this.concurrent.RunAsync(() => { ever.Event.Handle(arg); });
            await this.concurrent.RunAsync(() => { ever.Listener.Invoke(arg); });
            if (ever.RunType == EventRunType.AfterListener)
                await this.concurrent.RunAsync(() => { ever.Event.Handle(arg); });

        }


        public List<string> GetEventList()
        {
            var data = from v in this.commoneventlist
                       select $"{v.Key}---{v.Value}";
            return data.ToList();

        }
    }
}
