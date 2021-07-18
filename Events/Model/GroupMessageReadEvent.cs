using System;

namespace Konata.Core.Events.Model
{
    internal class GroupMessageReadEvent : ProtocolEvent
    {
        public uint GroupUin { get; set; }

        public uint RequestId { get; set; }

        public GroupMessageReadEvent()
            => WaitForResponse = false;
    }
}
