using System;

using Konata.Runtime.Base.Event;

namespace Konata.Core.Event
{
    public class EventGroupPromoteAdmin : KonataEventArgs
    {
        /// <summary>
        /// <b>[In]</b> <br/>
        /// Group uin being operated.
        /// </summary>
        public uint GroupUin { get; set; }

        /// <summary>
        /// <b>[In]</b> <br/>
        /// Member uin being operated.
        /// </summary>
        public uint MemberUin { get; set; }

        /// <summary>
        /// <b>[In]</b> <br/>
        ///  Flag to toggle set or unset. <br/>
        /// </summary>
        public bool ToggleType { get; set; }
    }
}
