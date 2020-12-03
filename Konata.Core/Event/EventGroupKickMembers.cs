using System;

using Konata.Runtime.Base.Event;

namespace Konata.Core.Event
{
    public class EventGroupKickMembers : KonataEventArgs
    {
        public uint GroupUin { get; set; }

        public uint[] MembersUin { get; set; }

        /// <summary>
        /// <b>KickMembers</b>: Block the members <br/>
        /// </summary>
        public bool ToggleType { get; set; }
    }
}
