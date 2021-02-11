using System;

namespace Konata.Core.Event.EventModel
{
    public class GroupKickMembersEvent : ProtocolEvent
    {
        public uint GroupUin { get; set; }

        public uint[] MembersUin { get; set; }

        /// <summary>
        /// <b>KickMembers</b>: Block the members <br/>
        /// </summary>
        public bool ToggleType { get; set; }

        public GroupKickMembersEvent()
            => WaitForResponse = true;
    }
}
