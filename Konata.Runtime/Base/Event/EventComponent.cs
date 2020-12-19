using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks.Dataflow;

using Konata.Runtime.Base.Event;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Konata.Runtime.Utils;

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

        private Dictionary<string, ConcurrentQueue<TaskCompletionSource<KonataEventArgs>>> _entityTasks;
        private Dictionary<string, ConcurrentQueue<Action>> _entitySyncMethod;

        public void Load()
        {
            if (_eventInstance == null)
            {
                _instanceLock = new ReaderWriterLockSlim();
                var eventinfos = EventManager.Instance.GetCoreEventInfo();

                _eventInstance = new Dictionary<CoreEventType, EventContainer>(new EnumComparer<CoreEventType>());
                //需要针对当前子容器进行事件列表初始化
                foreach (KeyValuePair<CoreEventType, EventInfo> valuePair in eventinfos)
                {
                    IEvent obj = null;
                    if (typeof(IEvent).IsAssignableFrom(valuePair.Value.Type))
                    {
                        obj = (IEvent)Activator.CreateInstance(valuePair.Value.Type);
                    }

                    _eventInstance.Add(valuePair.Key, new EventContainer
                    {
                        Event = obj,
                        Info = valuePair.Value
                    });
                }

                _blockCancel = new CancellationTokenSource();

                _endBlock = new ActionBlock<KonataEventArgs>((arg) => { ContainerFilter(arg); },
                    new ExecutionDataflowBlockOptions { CancellationToken = _blockCancel.Token });

                _entityTasks = new Dictionary<string, ConcurrentQueue<TaskCompletionSource<KonataEventArgs>>>();
                _entitySyncMethod = new Dictionary<string, ConcurrentQueue<Action>>();

                this.AddNewListener(CoreEventType.TaskComplate, (o, arg) => { this.ComplateSyncTask(arg.EventName,arg); });
            }
            EventManager.Instance.RegisterNewEntity(Parent);
        }

        /// <summary>
        /// 注册监听事件
        /// </summary>
        /// <param name="type"></param>
        /// <param name="listener"></param>
        public void AddNewListener(CoreEventType type, EventCallback listener)
        {
            if (_eventInstance != null)
            {
                if (_eventInstance.TryGetValue(type, out var value))
                {
                    value.Listener += listener;
                }
            }
        }

        /// <summary>
        /// 注册等待回复Task
        /// <para>并行，非同步执行</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public TaskCompletionSource<KonataEventArgs> RegisterTaskSource(string name,CancellationToken token)
        {
            if (_entityTasks != null&&_entitySyncMethod!=null&&!_entitySyncMethod.ContainsKey(name))
            {
                TaskCompletionSource<KonataEventArgs> task = new TaskCompletionSource<KonataEventArgs>();
                token.Register(() => { task.TrySetCanceled(); });
                if(_entityTasks.TryGetValue(name,out var queue))
                {
                    queue.Enqueue(task);
                }
                else
                {
                    queue = new ConcurrentQueue<TaskCompletionSource<KonataEventArgs>>();
                    queue.Enqueue(task);
                    _entityTasks.Add(name, queue);
                }
                return task;
            }
            return null;
        }
        /// <summary>
        /// 注册等待回复Task
        /// <para>任务以同步队列方式处理</para>
        /// <para>如果目标任务列表为空,</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="action"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public TaskCompletionSource<KonataEventArgs> RegisterSyncTaskSource(string name,Action action,CancellationToken token)
        {
            _instanceLock.EnterWriteLock();
            try
            {
                if (_entityTasks != null && _entitySyncMethod != null)
                {
                    TaskCompletionSource<KonataEventArgs> task = new TaskCompletionSource<KonataEventArgs>();
                    token.Register(() => { task.TrySetCanceled(); });
                    //save task and action
                    if(!_entityTasks.TryGetValue(name,out var queue))
                    {
                        queue = new ConcurrentQueue<TaskCompletionSource<KonataEventArgs>>();
                        _entityTasks.Add(name, queue);
                    }
                    if (!_entitySyncMethod.TryGetValue(name, out var syncmethods))
                    {
                        syncmethods = new ConcurrentQueue<Action>();
                        _entitySyncMethod.Add(name, syncmethods);
                    }

                    if (queue.IsEmpty)
                    {
                        action.Invoke();
                    }
                    else
                    {
                        syncmethods.Enqueue(action);
                    }
                    queue.Enqueue(task);

                    return task;
                }
                return null;
            }
            finally
            {
                _instanceLock.ExitWriteLock();
            }

        }

        /// <summary>
        /// 完成一个同步执行的task
        /// <para>之后将会自动执行同名任务表</para>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="arg"></param>
        private void ComplateSyncTask(string name,KonataEventArgs arg)
        {
            if (_entityTasks != null && _entitySyncMethod != null)
            {
                if(_entityTasks.TryGetValue(name,out var q))
                {
                    if (q.TryDequeue(out var task))
                    {
                        task.TrySetResult(arg);
                    }
                    if (_entitySyncMethod.TryGetValue(name,out var sq))
                    {
                        if(sq.TryDequeue(out var action))
                        {
                            action.Invoke();
                        }
                    }
                }
            }
        }


        public ITargetBlock<KonataEventArgs> GetPipe()
        {
            if (_endBlock != null)
            {
                return _endBlock;
            }
            return null;
        }


        /// <summary>
        /// 用于子容器处理队列事件分发过滤
        /// </summary>
        /// <param name="arg"></param>
        private void ContainerFilter(KonataEventArgs arg)
        {
            if (arg != null)
            {
                if (arg.CoreEventType != CoreEventType.Custom || arg.CoreEventType != CoreEventType.UnDefined)
                {
                    if (_eventInstance.TryGetValue(arg.CoreEventType, out var container))
                    {
                        container.Invoke(arg);
                    }
                }
                else//Using EventName
                {

                }
            }
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

            if (_entitySyncMethod.Count > 0)
            {
                var stemp = _entitySyncMethod;
                _entitySyncMethod = null;
                foreach(var sq in stemp.Values)
                {
                    while (!sq.IsEmpty)
                    {
                        sq.TryDequeue(out _);
                    }
                }
                stemp.Clear();
            }

            if (_entityTasks.Count > 0)
            {
                var temp = _entityTasks;
                _entityTasks = null;
                foreach (var q in temp.Values)
                {
                    while (!q.IsEmpty)
                    {
                        q.TryDequeue(out var task);
                        task.TrySetCanceled();
                    }
                }
                temp.Clear();
            }
            EventManager.Instance.UnRegisterEntity(Parent);
            base.Dispose();
        }
    }
}
