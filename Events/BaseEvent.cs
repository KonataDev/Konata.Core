using System;

namespace Konata.Core.Events
{
    public class BaseEvent : EventArgs
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
