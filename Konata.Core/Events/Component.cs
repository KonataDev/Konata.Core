using System;
using System.Collections.Generic;

namespace Konata.Events
{
    public delegate object EventHandler(EventParacel eventParacel);

    public abstract class EventComponent
    {
        protected readonly EventPumper eventPumper;
        protected EventHandlers eventHandlers;

        public EventComponent(EventPumper pumper)
        {
            eventPumper = pumper;

            eventHandlers = new EventHandlers();
            eventHandlers += OnEvent;
        }

        protected EventParacel PostEvent(EventParacel eventParacel)
            => eventPumper.PostEvent(eventParacel);

        protected EventParacel PostEvent<T>(EventParacel eventParacel)
            where T : EventComponent
            => eventPumper.PostEvent<T>(eventParacel);

        protected EventParacel CallEvent(EventParacel eventParacel)
            => eventPumper.CallEvent(eventParacel);

        protected EventParacel CallEvent<T>(EventParacel eventParacel)
            where T : EventComponent
            => eventPumper.CallEvent<T>(eventParacel);

        protected void BroadcastEvent(EventParacel eventParacel)
            => eventPumper.BroadcastEvent(eventParacel);

        public T GetComponent<T>()
            where T : EventComponent
            => eventPumper.GetComponent<T>();

        protected abstract EventParacel OnEvent(EventParacel eventParacel);
    }

    public class EventHandlers : Dictionary<Type, EventHandler>
    {
        public static EventHandlers operator +(EventHandlers a, EventHandler b)
        {
            a.Add(b.GetType(), b);
            return a;
        }
    }
}
