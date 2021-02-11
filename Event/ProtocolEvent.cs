using System;

namespace Konata.Core.Event
{
    public class ProtocolEvent : BaseEvent
    {
        public bool WaitForResponse { get; set; }

        public int SessionSequence { get; set; }
    }
}
