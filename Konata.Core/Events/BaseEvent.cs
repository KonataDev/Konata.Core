using System;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Konata.Core.Events
{
    public class BaseEvent : EventArgs
    {
        public DateTime EventTime { get; }

        public string EventMessage { get; protected set; }

        internal BaseEvent()
        {
            EventTime = DateTime.Now;
            EventMessage = "";
        }
    }
}
