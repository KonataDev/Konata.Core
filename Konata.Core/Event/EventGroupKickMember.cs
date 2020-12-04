using System;

using Konata.Runtime.Base.Event;

namespace Konata.Core.Event
{
    public class EventGroupKickMember : KonataEventArgs
    {
        public uint GroupUin { get; set; }

        public uint MemberUin { get; set; }

        /// <summary>
        /// <b>KickMember</b>: Block the member <br/>
        /// </summary>
        public bool ToggleType { get; set; }
    }
}
