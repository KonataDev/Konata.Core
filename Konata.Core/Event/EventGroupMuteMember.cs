using System;

using Konata.Runtime.Base.Event;

namespace Konata.Core.Event
{
    public class EventGroupMuteMember : KonataEventArgs
    {
        public uint GroupUin { get; set; }

        public uint MemberUin { get; set; }

        /// <summary>
        /// <b>MuteMember</b>: Mute time <br/>
        /// </summary>
        public uint? TimeSeconds { get; set; }
    }
}
