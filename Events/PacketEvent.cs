using System;

namespace Konata.Core.Events
{
    public class PacketEvent : BaseEvent
    {
        public enum Type
        {
            Send,
            Receive,
            //SendAction
        }

        public Type EventType { get; set; }

        public byte[] Buffer { get; set; }
    }
}
