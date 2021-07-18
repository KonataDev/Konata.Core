using System;

namespace Konata.Core.Events.EventModel
{
    internal class GroupMessageReadEvent : ProtocolEvent
    {
        public uint GroupUin { get; set; }

        public uint RequestId { get; set; }

        public GroupMessageReadEvent()
            => WaitForResponse = false;
    }
}
