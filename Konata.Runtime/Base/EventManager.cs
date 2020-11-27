using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using System.Collections.Generic;

using Konata.Runtime.Base;
using Konata.Runtime.Base.Event;
using Konata.Runtime.Utils;

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

        public static EventManager Instance
        {
            get => instance ?? (instance = new EventManager());
        }
        private EventManager()
        {
        }

        private readonly Dictionary<CoreEventType, EventInfo> coreeventlist
            = new Dictionary<CoreEventType, EventInfo>(new EnumComparer<CoreEventType>());

        private readonly Dictionary<long, EventComponent> bindcomponentlist
            = new Dictionary<long, EventComponent>();

        private ReaderWriterLockSlim coreeventlock = new ReaderWriterLockSlim();

        public bool CoreEventLoaded
        {
            get => (this.coreeventlist.Count > 0);
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

            coreeventlock.EnterWriteLock();

            try
            {
                foreach (Type type in types)
                {
                    object attribute = type.GetCustomAttributes(typeof(CoreEventAttribute), false).FirstOrDefault();
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
                    EventInfo info = new EventInfo { RunType = eattr.EventRunType, Name = eattr.Name, Description = eattr.Description };
                }
            }
            finally
            {
                coreeventlock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 向事件管理器注册新的实体对象
        /// <para>这将为其创建新的事件组件并挂载到目标对象</para>
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public void RegisterNewEntity(Entity entity)
        {
            coreeventlock.EnterWriteLock();
            try
            {
                if (bindcomponentlist.ContainsKey(entity.Id))
                {
                    throw new ArgumentException($"Entity already Registered");
                }
                EventComponent component = entity.GetComponent<EventComponent>();
                if (component == null)
                {
                    throw new ArgumentException();
                }
                bindcomponentlist.Add(entity.Id, component);
            }
            finally
            {
                coreeventlock.ExitWriteLock();
            }
        }

        public void UnRegisterEntity(Entity entity)
        {
            coreeventlock.EnterWriteLock();
            try
            {
                if (bindcomponentlist.TryGetValue(entity.Id, out EventComponent component))
                {
                    bindcomponentlist.Remove(entity.Id);
                }
            }
            finally
            {
                coreeventlock.ExitWriteLock();
            }
        }

        /// <summary>
        /// 直接向目标实体添加事件输出源
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="output"></param>
        public void LinkPipeLineToEntity(Entity entity, ISourceBlock<KonataEventArgs> output)
        {
            coreeventlock.EnterReadLock();
            try
            {
                if (bindcomponentlist.ContainsKey(entity.Id))
                {
                    var component = bindcomponentlist[entity.Id];
                    component.AddNewSource(output);
                }
            }
            finally
            {
                coreeventlock.ExitReadLock();
            }
        }

        public IReadOnlyDictionary<CoreEventType, EventInfo> GetCoreEventInfo()
        {
            if (!CoreEventLoaded)
            {
                return null;
            }
            coreeventlock.EnterReadLock();
            try
            {
                return coreeventlist;
            }
            finally
            {
                coreeventlock.ExitReadLock();
            }
        }
    }
}
