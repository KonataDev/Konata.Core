using System;

using Konata.Runtime.Base.Event;

namespace Konata.Core.Event
{
    public class EventGroupKickMember : KonataEventArgs
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
        /// <b>[Opt] [In]</b> <br/>
        ///  Flag to prevent member request or no. <br/>
        ///  The default value is <b>false</b>
        /// </summary>
        public bool ToggleType { get; set; }
    }
}
