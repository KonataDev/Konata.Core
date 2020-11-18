using System;
using System.Threading;
using System.Collections.Concurrent;

namespace Konata.Events
{
    using EventWorkers = ThreadPool;
    using EventQueue = ConcurrentQueue<EventParacel>;
    using EventComponents = ConcurrentDictionary<Type, EventComponent>;

    public class EventPumper
    {
        private int isExit;
        private EventQueue eventQueue;
        private EventComponents eventComponents;

        public EventPumper()
        {
            isExit = 1;
            eventQueue = new EventQueue();
            eventComponents = new EventComponents();
        }

        public virtual void Run()
        {
            if (isExit == 0) return;
            isExit = 0;

            Thread[] threads = new Thread[Environment.ProcessorCount];
            for (int i = 0; i < threads.Length; ++i)
            {
                threads[i] = new Thread(DaemonThread);
                threads[i].Start();
            }

            for (int i = 0; i < threads.Length; ++i)
            {
                threads[i].Join();
            }
        }

        private void DaemonThread()
        {
            while (isExit == 0)
            {
                var next = GetEvent();
                {
                    if (next is EventPumperCtl inter)
                        switch (inter.Type)
                        {
                            case EventPumperCtl.CtlType.Idle:
                                Thread.Sleep(1);
                                continue;
                            case EventPumperCtl.CtlType.Exit:
                                Interlocked.Exchange(ref isExit, 1);
                                return;
                        }

                    EventWorkers.QueueUserWorkItem((_) => { ProcessEvent(next); });
                }
            }
        }

        private EventParacel ProcessEvent(EventParacel eventParacel, bool broadcast = false)
        {
            if (!broadcast && eventParacel.EventTo != null)
                return eventParacel.EventTo.OnEvent(eventParacel);

            foreach (var component in eventComponents)
            {
                foreach (var handler in component.Value.eventHandlers)
                {
                    var result = handler.Value(eventParacel);
                    if (!broadcast && (result != null || result != EventParacel.Reject))
                        return result;
                }
            }

            return EventParacel.Reject;
        }

        private EventParacel GetEvent()
        {
            EventParacel qEvent = EventParacel.Idle;

            if (eventQueue.Count > 0)
            {
                if (eventQueue.TryDequeue(out var e))
                {
                    qEvent = e;
                }
            }

            return qEvent;
        }

        public EventParacel PostEvent(EventParacel eventParacel)
        {
            eventQueue.Enqueue(eventParacel);
            return EventParacel.Accept;
        }

        public EventParacel PostEvent<T>(EventParacel eventParacel)
            where T : EventComponent
        {
            eventParacel.EventTo = GetComponent<T>();
            return PostEvent(eventParacel);
        }

        public EventParacel CallEvent(EventParacel eventParacel,
            uint timeout = 3000)
        {
            return ProcessEvent(eventParacel);
        }

        public EventParacel CallEvent<T>(EventParacel eventParacel)
            where T : EventComponent
        {
            eventParacel.EventTo = GetComponent<T>();
            return CallEvent(eventParacel);
        }

        public void BroadcastEvent(EventParacel eventParacel)
        {
            ProcessEvent(eventParacel, true);
        }

        public void Exit()
            => PostEvent(EventParacel.Exit);

        public T GetComponent<T>()
            where T : EventComponent
        {
            eventComponents.TryGetValue(typeof(T), out var component);
            return (T)component;
        }

        public T TryGetComponent<T>()
            where T : EventComponent
        {
            try
            {
                return GetComponent<T>();
            }
            catch
            {
                return null;
            }
        }

        public void RegisterComponent(EventComponent ec)
            => eventComponents.TryAdd(ec.GetType(), ec);
    }

    public class EventPumperCtl : EventParacel
    {
        public enum CtlType
        {
            Idle,
            Exit,
            Accept,
            Reject,
        }

        public CtlType Type { get; set; }
    }
}
