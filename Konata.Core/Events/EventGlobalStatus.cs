using System;

namespace Konata.Events
{
    class EventGlobalStatus : EventParacel
    {
        public enum EventType
        {
            Online,
            Offline,
            Disconnect
        }

        public EventType Type { get; set; }
    }
}
