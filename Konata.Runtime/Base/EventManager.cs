using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using System.Collections.Generic;

using Konata.Runtime.Base;
using Konata.Runtime.Base.Event;
using Konata.Runtime.Utils;
using System.Threading.Tasks;

namespace Konata.Runtime
{
    /// <summary>
    /// 用于描述事件类信息的信息表
    /// </summary>
    public class EventInfo
    {
        public bool Enable { get; set; } = true;

        public string Name { get; set; } = "";

        public EventRunType RunType { get; set; } = EventRunType.OnlySymbol;

        public string Description { get; set; } = "";

        public Type Type { get; set; } = null;
    }

    /// <summary>
    /// 事件管理器
    /// </summary>
    public class EventManager
    {
        private static EventManager instance;
        private readonly ReaderWriterLockSlim coreEventLock;
        private readonly Dictionary<CoreEventType, EventInfo> coreEventList;
        private readonly Dictionary<long, EventComponent> bindComponentList;

        public static EventManager Instance
        {
            get => instance ?? (instance = new EventManager());
        }

        private EventManager()
        {
            coreEventLock = new ReaderWriterLockSlim();
            bindComponentList = new Dictionary<long, EventComponent>();
            coreEventList = new Dictionary<CoreEventType, EventInfo>
                (new EnumComparer<CoreEventType>());
        }

        public bool CoreEventLoaded
        {
            get => (this.coreEventList.Count > 0);
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

            coreEventLock.EnterWriteLock();
            try
            {
                foreach (Type type in types)
                {
                    object attribute = type.GetCustomAttributes(typeof(CoreEventAttribute), false).FirstOrDefault();

                    if (attribute == null)
                    {
                        throw new NullReferenceException("Event type find no attribute(should not be happened)");
                    }

                    CoreEventAttribute eventAttr = (attribute as CoreEventAttribute);
                    CoreEventType eventType = eventAttr.EventType;

                    if (this.coreEventList.ContainsKey(eventType))
                    {
                        throw new ArgumentException($"Find same type core event:even set assemblyheader ({eventType})");
                    }

                    //EventInfo info = new EventInfo
                    //{
                    //    RunType = eventAttr.EventRunType,
                    //    Name = eventAttr.Name,
                    //    Description = eventAttr.Description
                    //};
                }
            }
            finally
            {
                coreEventLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 向事件管理器注册新的实体对象
        /// <para>该方法不应被EventComponent以外调用</para>
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public void RegisterNewEntity(Entity entity)
        {
            coreEventLock.EnterWriteLock();
            try
            {
                if (bindComponentList.ContainsKey(entity.Id))
                {
                    throw new ArgumentException($"Entity already Registered");
                }

                EventComponent component = entity.GetComponent<EventComponent>();
                if (component == null)
                {
                    throw new ArgumentException($"Target Entity has no EventComponent");
                }
                bindComponentList.Add(entity.Id, component);
            }
            finally
            {
                coreEventLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 从事件管理器移除目标实体[这不会销毁目标实体上事件组件]
        /// </summary>
        /// <param name="entity"></param>
        public void UnRegisterEntity(Entity entity)
        {
            coreEventLock.EnterWriteLock();
            try
            {
                if (bindComponentList.TryGetValue(entity.Id, out EventComponent component))
                {
                    bindComponentList.Remove(entity.Id);
                }
            }
            finally
            {
                coreEventLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 直接获取目标实体事件输出管道
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="output"></param>
        /// 
        public ITargetBlock<KonataEventArgs> EntityEventPipe(Entity entity)
        {
            coreEventLock.EnterReadLock();
            try
            {
                if (bindComponentList.TryGetValue(entity.Id,out var component))
                {
                    return component.GetPipe();
                }
                return null;
            }
            finally
            {
                coreEventLock.ExitReadLock();
            }
        }

        /// <summary>
        /// 向目标实体发送事件
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="arg"></param>
        public async Task SendEventToEntity(Entity entity,KonataEventArgs arg)
        {
            coreEventLock.EnterReadLock();
            try
            {
                if (bindComponentList.TryGetValue(entity.Id, out var component))
                {
                    await component.GetPipe().SendAsync(arg);
                }
            }
            finally
            {
                coreEventLock.ExitReadLock();
            }
        }

        public IReadOnlyDictionary<CoreEventType, EventInfo> GetCoreEventInfo()
        {
            if (!CoreEventLoaded)
            {
                return null;
            }
            coreEventLock.EnterReadLock();
            try
            {
                return coreEventList;
            }
            finally
            {
                coreEventLock.ExitReadLock();
            }
        }
    }
}
