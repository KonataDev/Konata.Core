using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks.Dataflow;

using Konata.Runtime.Base.Event;

namespace Konata.Runtime.Base
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

    [Component(name: "事件组件", description: "面向当前实体的事件子容器")]
    public class EventComponent : Component, ILoad
    {
        private ReaderWriterLockSlim _instanceLock;
        private CancellationTokenSource _blockCancel;
        private ActionBlock<KonataEventArgs> _endBlock;
        private Dictionary<CoreEventType, EventContainer> _eventInstance;

        public void Load()
        {
            if (_eventInstance == null)
            {
                _instanceLock = new ReaderWriterLockSlim();
                IReadOnlyDictionary<CoreEventType, EventInfo> eventinfos = EventManager.Instance.GetCoreEventInfo();

                //需要针对当前子容器进行事件列表初始化
                foreach (KeyValuePair<CoreEventType, EventInfo> valuePair in eventinfos)
                {
                    IEvent obj = null;
                    if (typeof(IEvent).IsAssignableFrom(valuePair.Value.Type))
                    {
                        obj = (IEvent)Activator.CreateInstance(valuePair.Value.Type);
                    }
                    EventContainer container = new EventContainer { Info = valuePair.Value, Event = obj };
                    _eventInstance.Add(valuePair.Key, container);
                }
            }
            EventManager.Instance.RegisterNewEntity(Parent);
        }

        public void AddNewListener(CoreEventType type, EventCallback listener)
        {
            if (_eventInstance != null)
            {
                _instanceLock.EnterReadLock();
                try
                {
                    if (_eventInstance.TryGetValue(type, out var value))
                    {
                        value.Listener += listener;
                    }
                }
                finally
                {
                    _instanceLock.ExitReadLock();
                }
            }
        }

        public ITargetBlock<KonataEventArgs> GetPipe()
        {
            if (_endBlock == null)
            {
                _blockCancel = new CancellationTokenSource();

                _endBlock = new ActionBlock<KonataEventArgs>((arg) => { ContainerFilter(arg); },
                    new ExecutionDataflowBlockOptions { CancellationToken = _blockCancel.Token});
            }

            return _endBlock;
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
            if (_endBlock != null)
            {
                _endBlock.Complete();
                _blockCancel.Cancel();
                _endBlock = null;
            }
            EventManager.Instance.UnRegisterEntity(Parent);
            base.Dispose();
        }
    }
}
