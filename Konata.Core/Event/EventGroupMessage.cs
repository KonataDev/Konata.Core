using System;

using Konata.Runtime.Base.Event;

namespace Konata.Core.Event
{
    public class EventGroupMessage : KonataEventArgs
    {
        public string GroupName { get; set; }

        public uint GroupUin { get; set; }

        public uint MemberUin { get; set; }

        public string MemberCard { get; set; }

        public string MessageContent { get; set; }
    }
}
