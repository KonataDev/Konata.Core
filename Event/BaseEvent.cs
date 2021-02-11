using System;

namespace Konata.Core.Event
{
    public class BaseEvent
    {
        public DateTime EventTime { get; set; }

        public string EventMessage { get; set; }

        public BaseEvent()
        {
            EventTime = DateTime.Now;
            EventMessage = "";
        }
    }
}
