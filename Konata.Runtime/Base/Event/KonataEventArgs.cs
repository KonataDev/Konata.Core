using System;

namespace Konata.Runtime.Base.Event
{
    /// <summary>
    /// Konata standard event
    /// </summary>
    public class KonataEventArgs : EventArgs
    {
        /// <summary>
        /// Event owner
        /// </summary>
        public Entity Owner { get; set; }

        /// <summary>
        /// Event name
        /// </summary>
        public string EventName { get; set; }

        /// <summary>
        /// Hint message with this event
        /// </summary>
        public string EventMessage { get; set; }
    }
}
