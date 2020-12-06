using System;

using Konata.Runtime.Base.Event;

namespace Konata.Core.Event
{
    public class EventGroupSpecialTitle : KonataEventArgs
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
    }
}
