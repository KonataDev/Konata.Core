using System;

namespace Konata.Events
{
    public delegate EventParacel EventDelegate(EventParacel eventParacel);

    public abstract class EventParacel
    {
        internal static EventPumperCtl Idle { private set; get; }
            = new EventPumperCtl { Type = EventPumperCtl.CtlType.Idle };

        internal static EventPumperCtl Exit { private set; get; }
            = new EventPumperCtl { Type = EventPumperCtl.CtlType.Exit };
        internal static EventPumperCtl Accept { private set; get; }
            = new EventPumperCtl { Type = EventPumperCtl.CtlType.Accept };

        internal static EventPumperCtl Reject { private set; get; }
            = new EventPumperCtl { Type = EventPumperCtl.CtlType.Reject };

        internal EventComponent EventTo { set; get; }

        internal EventComponent EventFrom { set; get; }

        public EventDelegate EventDelegate { set; get; }

        public EventParacel()
        {

        }

        public EventParacel(EventComponent from)
        {
            EventFrom = from;
        }

        public EventParacel(EventComponent from, EventComponent to,
            EventDelegate func)
        {
            EventTo = to;
            EventFrom = from;
            EventDelegate = func;
        }
    }

    public class EventNotify : EventParacel
    {
        public uint NotifyId;
    }
}
