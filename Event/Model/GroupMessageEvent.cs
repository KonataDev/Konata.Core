using System;
using System.Collections.Generic;

using Konata.Core.Message;

namespace Konata.Core.Event.EventModel
{
    public class GroupMessageEvent : ProtocolEvent
    {
        public string GroupName { get; set; }

        public uint GroupUin { get; set; }

        public uint MemberUin { get; set; }

        public string MemberCard { get; set; }

        public MessageChain Message { get; set; }

        public uint MessageId { get; set; }

        public uint MessageTime { get; set; }

        public uint SliceTotal { get; set; }

        public uint SliceIndex { get; set; }

        public uint SliceFlags { get; set; }

        public GroupMessageEvent()
            => WaitForResponse = true;

        public override string ToString()
            => Message.ToString();
    }
}
