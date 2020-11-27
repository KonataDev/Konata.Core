using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks.Dataflow;

using Konata.Core.Base.Event;

namespace Konata.Core.Base
{
    public delegate void EventCallback(object sender, KonataEventArgs arg);
    internal class EventContainer
    {
        
        public EventInfo Info { get; set; }

        public IEvent Event { get; set; }

        private readonly WeakEvent<KonataEventArgs> listener = new WeakEvent<KonataEventArgs>();
        
        public event EventCallback Listener
        {
            add => listener.Add(value, value.Invoke);
            remove => listener.Remove(value);
        }

        public void Invoke(KonataEventArgs arg) => listener.Invoke(this, arg);
    }


    [Component(name: "事件组件", des: "面向当前实体的事件子容器")]
    public class EventComponent : Component, ILoad
    {
        private ActionBlock<KonataEventArgs> endblock = null;

        private CancellationTokenSource blockcancel = null;

        private Dictionary<CoreEventType, EventContainer> eventinstance = null;

        private ReaderWriterLockSlim instancelock = null;

        public void Load()
        {
            if (eventinstance == null)
            {
                instancelock = new ReaderWriterLockSlim();
                IReadOnlyDictionary<CoreEventType, EventInfo> eventinfos = EventManager.Instance.GetCoreEventInfo();

                //需要针对当前子容器进行事件列表初始化
                foreach(KeyValuePair<CoreEventType,EventInfo> valuePair in eventinfos)
                {
                    IEvent obj = null;
                    if (typeof(IEvent).IsAssignableFrom(valuePair.Value.Type))
                    {
                        obj = (IEvent)Activator.CreateInstance(valuePair.Value.Type);
                    }
                    EventContainer container = new EventContainer { Info = valuePair.Value, Event = obj };
                    eventinstance.Add(valuePair.Key, container);
                }
            }
            EventManager.Instance.RegisterNewEntity(Parent);
        }

        public void AddNewListener(CoreEventType type, EventCallback listener)
        {
            if (eventinstance != null)
            {
                instancelock.EnterReadLock();
                try
                {
                    if(eventinstance.TryGetValue(type,out var value))
                    {
                        value.Listener += listener;
                    }
                }
                finally
                {
                    instancelock.ExitReadLock();
                }
            }
        }

        /// <summary>
        /// 向当前子容器处理队列添加新的数据源
        /// <para>该添加法等同于向该子容器直接发送消息</para>
        /// </summary>
        /// <param name="output"></param>
        public void AddNewSource(ISourceBlock<KonataEventArgs> output)
        {
            if (endblock == null)
            {
                blockcancel = new CancellationTokenSource();

                endblock = new ActionBlock<KonataEventArgs>((arg) => { ContainerFilter(arg); },
                    new ExecutionDataflowBlockOptions { CancellationToken=blockcancel.Token,MaxDegreeOfParallelism = 2 });
            }
            output.LinkTo(endblock);
        }

        /// <summary>
        /// 用于子容器处理队列事件分发过滤
        /// </summary>
        /// <param name="arg"></param>
        private void ContainerFilter(KonataEventArgs arg)
        {

        }

        public override void Dispose()
        {
            //标记子容器事件队列终结[拒绝继续获取/处理事务]
            if (endblock != null)
            {
                endblock.Complete();
                blockcancel.Cancel();
            }
            EventManager.Instance.UnRegisterEntity(Parent);
            base.Dispose();
        }
    }
}
