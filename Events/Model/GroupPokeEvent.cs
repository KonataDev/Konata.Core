using System;

namespace Konata.Core.Events.Model
{
    public class GroupPokeEvent : ProtocolEvent
    {
        public uint GroupUin { get; set; }

        public uint MemberUin { get; set; }

        public uint OperatorUin { get; set; }

        public string ActionPrefix { get; set; }

        public string ActionSuffix { get; set; }
    }
}
