using System;

namespace Konata.Core.Events.Model
{
    public class PullTroopListEvent : ProtocolEvent
    {
        /// <summary>
        /// <b>[In]</b>          <br/>
        ///   Self uin.          <br/>
        /// </summary>
        public uint SelfUin { get; set; }
    }
}
