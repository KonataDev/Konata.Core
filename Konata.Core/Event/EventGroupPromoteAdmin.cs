using System;

using Konata.Runtime.Base.Event;

namespace Konata.Core.Event
{
    public class EventGroupPromoteAdmin : KonataEventArgs
    {
        public uint GroupUin { get; set; }

        public uint MemberUin { get; set; }

        /// <b>PromoteAdmin</b>: Set or Unset
        /// </summary>
        public bool ToggleType { get; set; }
    }
}
