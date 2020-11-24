using Konata.Core.Base.Event;
using Konata.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Konata.Core.Base
{
    public class EventManager
    {
        private EventManager instance;

        public EventManager Instance
        {
            get => instance ?? (instance = new EventManager());
        }
        private  EventManager()
        {

        }

        public void Release()
        {
            this.instance = null;
        }

        //面向插件等非核心事件的字典列表
        private readonly Dictionary<string, IEvent> commoneventlist = new Dictionary<string, IEvent>();
        //针对核心事件的字典列表,使用固定枚举类型加速执行事件
        private readonly Dictionary<CoreEventType, IEvent> coreeventlist = new Dictionary<CoreEventType, IEvent>(new EnumComparer<CoreEventType>());

        public bool CoreEventLoaded
        {
            get => (this.coreeventlist.Count > 0);
        }

        /// <summary>
        /// 注册普通事件
        /// </summary>
        /// <param name="assemblyname">程序集名称</param>
        /// <param name="types">事件类型</param>
        public void RegisterNewEvent(string assemblyname,IList<Type> types)
        {
            foreach(Type type in types)
            {
                object attribute = type.GetCustomAttributes(typeof(EventAttribute), false).FirstOrDefault();
                if (attribute == null)
                {
                    throw new NullReferenceException("Event type find no attribute(should not be happened)");
                }
                string eventname = (attribute as EventAttribute).EventType;
                string totalname = assemblyname + "." + eventname;
                if (this.commoneventlist.ContainsKey(totalname))
                {
                    throw new ArgumentException($"Find same name event:even set assemblyheader ({totalname})");
                }
                if (!type.IsAssignableFrom(typeof(IEvent)))
                {
                    throw new TypeLoadException($"Event {type.Name} not set IEvent Interface");
                }

                IEvent obj = (IEvent)Activator.CreateInstance(type);

                this.commoneventlist[totalname] = obj;
            }
        }

        /// <summary>
        /// 注册核心事件-只能注册一次
        /// </summary>
        /// <param name="types"></param>
        public void RegisterCoreEvent(IList<Type> types)
        {
            if (this.CoreEventLoaded)
            {
                throw new TypeLoadException("Core Events have been loaded!");
            }
            foreach (Type type in types)
            {
                object attribute = type.GetCustomAttributes(typeof(EventAttribute), false).FirstOrDefault();
                if (attribute == null)
                {
                    throw new NullReferenceException("Event type find no attribute(should not be happened)");
                }
                CoreEventType eventtype = (attribute as CoreEventAttribute).EventType;
                if (this.coreeventlist.ContainsKey(eventtype))
                {
                    throw new ArgumentException($"Find same type core event:even set assemblyheader ({eventtype})");
                }
                if (!type.IsAssignableFrom(typeof(IEvent)))
                {
                    throw new TypeLoadException($"Event {type.Name} not set IEvent Interface");
                }

                IEvent obj = (IEvent)Activator.CreateInstance(type);

                this.coreeventlist[eventtype] = obj;
            }
        }

        /// <summary>
        /// 移除单个普通事件
        /// </summary>
        /// <param name="assemblyname">绝对名称(程序集名)</param>
        /// <param name="name">绝对名称(事件名)</param>
        public void UnloadCommonEvent(string assemblyname,string name)
        {
            string totalname = assemblyname + "." + name;
            if (this.commoneventlist.ContainsKey(totalname))
            {
                this.commoneventlist.Remove(totalname);
            }
        }
        
        /// <summary>
        /// 移除单个/多个普通事件
        /// </summary>
        /// <param name="name">事件名称(部分匹配)</param>
        /// <param name="removeall">是否移除所有符合匹配名的事件</param>
        public bool UnloadCommonEvent(string name,bool removeall=false)
        {
            string[] names = this.commoneventlist.Keys.Where(key => key.Contains(name)).ToArray();
            if (names.Length > 1 && !removeall)
            {
                return false;
            }
            foreach(string truename in names)
            {
                this.commoneventlist.Remove(truename);
            }
            return true;
        }
    }
}
