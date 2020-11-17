using System;
using System.Threading;
using System.Collections.Generic;

namespace Konata.Events
{
    using EventMutex = Mutex;
    using EventQueue = Queue<EventParacel>;
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
                switch (GetEvent())
                {
                    case EventPumperCtl inter:
                        switch (inter.Type)
                        {
                            case EventPumperCtl.CtlType.Idle:
                                Thread.Sleep(1);
                                break;
                            case EventPumperCtl.CtlType.Exit:
                                isExit = true;
                                break;
                        }
                        break;
                    case EventParacel next:
                        EventWorkers.QueueUserWorkItem((object o) => { ProcessEvent(next); }, null);
                        break;
                }
            }
        }

        private EventParacel ProcessEvent(EventParacel eventParacel)
        {
            if (eventParacel.EventTo != null)
                return eventParacel.EventTo.OnEvent(eventParacel);

            foreach (var component in eventComponents)
            {
                foreach (var handler in component.Value.eventHandlers)
                {
                    var result = handler.Value(eventParacel);
                    if (result != null || result != EventParacel.Reject)
                        return result;
                }
            }

            return EventParacel.Reject;
        }

        private EventParacel GetEvent()
        {
            eventLock.WaitOne();
            {
                if (eventQueue.Count <= 0)
                {
                    eventLock.ReleaseMutex();
                    return EventParacel.Idle;
                }
                var qEvent = eventQueue.Dequeue();

                eventLock.ReleaseMutex();
                return qEvent;
            }
        }

        internal EventParacel PostEvent(EventParacel eventParacel)
        {
            eventLock.WaitOne();
            {
                eventQueue.Enqueue(eventParacel);
            }
            eventLock.ReleaseMutex();

            return EventParacel.Accept;
        }

        internal EventParacel PostEvent<T>(EventParacel eventParacel)
            where T : EventComponent
        {
            eventParacel.EventTo = GetComponent<T>();
            return PostEvent(eventParacel);
        }

        internal EventParacel CallEvent(EventParacel eventParacel,
            uint timeout = 3000)
        {
            return null;
        }

        internal EventParacel CallEvent<T>(EventParacel eventParacel)
            where T : EventComponent
        {
            eventParacel.EventTo = GetComponent<T>();
            return CallEvent(eventParacel);
        }

        internal void BroadcastEvent(EventParacel eventParacel)
        {
            foreach (var component in eventComponents)
            {

            }
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
