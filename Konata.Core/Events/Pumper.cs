using System;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace Konata.Events
{
    using EventMutex = Mutex;
    using EventQueue = ConcurrentQueue<EventParacel>;
    using EventWorkers = ThreadPool;
    using EventComponents = Dictionary<Type, EventComponent>;

    public class EventPumper
    {
        private bool isExit;
        private EventQueue eventQueue;
        private EventMutex eventLock;
        private EventComponents eventComponents;

        public EventPumper()
        {
            isExit = true;
            eventLock = new EventMutex();
            eventQueue = new EventQueue();
            eventComponents = new EventComponents();
        }

        public virtual void Run()
        {
            if (!isExit) return;
            isExit = false;

            while (!isExit)
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
            //eventLock.WaitOne();
            {
                EventParacel qEvent = EventParacel.Idle;

                if (eventQueue.Count > 0)
                {
                    if (eventQueue.TryDequeue(out var e))
                    {
                        qEvent = e;
                    }
                }

                //eventLock.ReleaseMutex();
                return qEvent;
            }
        }

        public EventParacel PostEvent(EventParacel eventParacel)
        {
            //eventLock.WaitOne();
            {
                eventQueue.Enqueue(eventParacel);
            }
            //eventLock.ReleaseMutex();

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
            if (!eventComponents.ContainsKey(typeof(T)))
                throw new Exception("No such component.");

            return (T)eventComponents[typeof(T)];
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
            => eventComponents.Add(ec.GetType(), ec);
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
