using System.Collections.Generic;

namespace Konata.Core.Events.Model
{
    public class PullGroupListEvent : ProtocolEvent
    {
        /// <summary>
        /// <b>[In]</b>           <br/>
        /// Self uin.             <br/>
        /// </summary>
        public uint SelfUin { get; set; }

        /// <summary>
        /// <b>[Out]</b>          <br/>
        /// Group info list.      <br/>
        /// </summary>
        public List<BotGroup> GroupInfo { get; set; }

        public PullGroupListEvent()
            => WaitForResponse = true;
    }
}
