using System.Collections.Generic;

namespace Konata.Core.Events.Model
{
    public class PullGroupMemberListEvent : ProtocolEvent
    {
        /// <summary>
        /// <b>[In]</b>            <br/>
        /// Self uin.              <br/>
        /// </summary>
        public uint SelfUin { get; set; }

        /// <summary>
        /// <b>[In]</b>            <br/>
        /// Group code.            <br/>
        /// </summary>
        public ulong GroupCode { get; set; }

        /// <summary>
        /// <b>[In]</b>            <br/>
        /// Group uin.             <br/>
        /// </summary>
        public uint GroupUin { get; set; }

        /// <summary>
        /// <b>[In] [Out]</b>      <br/>
        /// Next uin.              <br/>
        /// </summary>
        public uint NextUin { get; set; }

        /// <summary>
        /// <b>[Out]</b>           <br/>
        /// Office mode.           <br/>
        /// </summary>
        public uint OfficeMode { get; set; }

        /// <summary>
        /// <b>[Out]</b>           <br/>
        /// Time for next get op.  <br/>
        /// </summary>
        public uint NextGetTime { get; set; }

        /// <summary>
        /// <b>[Out]</b>           <br/>
        /// Error code             <br/>
        /// </summary>
        public short ErrorCode { get; set; }

        /// <summary>
        /// <b>[Out]</b>          <br/>
        /// Partial member list   <br/>
        /// </summary>
        public List<BotMember> MemberInfo { get; set; }

        public PullGroupMemberListEvent()
            => WaitForResponse = true;
    }
}
