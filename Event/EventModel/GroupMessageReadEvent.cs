using System;

namespace Konata.Core.Event.EventModel
{
    internal class GroupMessageReadEvent : ProtocolEvent
    {
        public uint GroupUin { get; set; }

        public uint RequestId { get; set; }

        public GroupMessageReadEvent()
            => WaitForResponse = false;
    }
}
