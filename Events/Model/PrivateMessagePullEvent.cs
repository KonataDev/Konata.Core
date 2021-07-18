using System;

namespace Konata.Core.Events.Model
{
    internal class PrivateMessagePullEvent : ProtocolEvent
    {
        public byte[] SyncCookie { get; set; }

        public PrivateMessagePullEvent()
            => WaitForResponse = false;
    }
}
