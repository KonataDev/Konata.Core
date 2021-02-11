using System;

namespace Konata.Core.Event.EventModel
{
    public class GroupModifyMemberCardEvent : ProtocolEvent
    {
        /// <summary>
        /// <b>[In]</b>          <br/>
        ///   Group uin          <br/>
        /// </summary>
        public uint GroupUin { get; set; }

        /// <summary>
        /// <b>[In]</b>           <br/>
        ///   Member uin          <br/>
        /// </summary>
        public uint MemberUin { get; set; }

        /// <summary>
        /// <b>[In]</b>           <br/>
        ///   Member groupd card  <br/>
        /// </summary>
        public string MemberCard { get; set; }

        public GroupModifyMemberCardEvent()
            => WaitForResponse = true;
    }
}
