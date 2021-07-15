using System;

namespace Konata.Core.Event.EventModel
{
    public class GroupSpecialTitleEvent : ProtocolEvent
    {
        public uint GroupUin { get; set; }

        public uint MemberUin { get; set; }

        /// <summary>
        /// Title expired time
        /// </summary>
        public uint? TimeSeconds { get; set; }

        /// <summary>
        /// Special title
        /// </summary>
        public string SpecialTitle { get; set; }

        public GroupSpecialTitleEvent()
            => WaitForResponse = true;
    }
}
