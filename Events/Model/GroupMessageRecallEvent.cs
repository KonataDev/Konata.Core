using System;

namespace Konata.Core.Events.Model
{
    public class GroupMessageRecallEvent : ProtocolEvent
    {
        public uint GroupUin { get; set; }

        public uint MemberUin { get; set; }

        public uint OperatorUin { get; set; }

        public string RecallSuffix { get; set; }
    }
}
