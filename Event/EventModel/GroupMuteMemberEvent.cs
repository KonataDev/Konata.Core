using System;

namespace Konata.Core.Event.EventModel
{
    public class GroupMuteMemberEvent : ProtocolEvent
    {
        public uint GroupUin { get; set; }

        public uint MemberUin { get; set; }

        /// <summary>
        /// <b>MuteMember</b>: Mute time <br/>
        /// </summary>
        public uint? TimeSeconds { get; set; }

        public GroupMuteMemberEvent()
            => WaitForResponse = true;
    }
}
