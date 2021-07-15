using System;

namespace Konata.Core.Event.EventModel
{
    internal class PrivateMessagePullEvent : ProtocolEvent
    {
        public byte[] SyncCookie { get; set; }

        public PrivateMessagePullEvent()
            => WaitForResponse = false;
    }
}
