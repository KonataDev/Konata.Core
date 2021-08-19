using System.Collections.Generic;

namespace Konata.Core.Events.Model
{
    public class PullFriendListEvent : ProtocolEvent
    {
        /// <summary>
        /// <b>[In]</b>           <br/>
        /// Self uin.             <br/>
        /// </summary>
        public uint SelfUin { get; set; }

        /// <summary>
        /// <b>[In]</b>           <br/>
        /// Start index.          <br/>
        /// </summary>
        public uint StartIndex { get; set; }

        /// <summary>
        /// <b>[In]</b>           <br/>
        /// Limit length.         <br/>
        /// </summary>
        public uint LimitNum { get; set; }

        /// <summary>
        /// <b>[Out]</b>          <br/>
        /// Error code.           <br/>
        /// </summary>
        public short ErrorCode { get; set; }

        /// <summary>
        /// <b>[Out]</b>          <br/>
        /// Total friend count.   <br/>
        /// </summary>
        public uint TotalFriendCount { get; set; }

        /// <summary>
        /// <b>[Out]</b>          <br/>
        /// Friend info list.     <br/>
        /// </summary>
        public List<BotFriend> FriendInfo { get; set; }

        public PullFriendListEvent()
            => WaitForResponse = true;
    }
}
