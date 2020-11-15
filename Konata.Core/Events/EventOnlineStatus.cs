using System;

namespace Konata.Events
{
    class EventOnlineStatus : EventParacel
    {
        public enum EventType
        {
            Online,
            Offline,
        }

        public EventType Type { get; set; }

        /// <summary>
        /// For EventType.Offline
        /// </summary>
        public string Reason { get; set; }
    }
}
