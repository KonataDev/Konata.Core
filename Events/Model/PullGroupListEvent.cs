using System.Collections.Generic;

namespace Konata.Core.Events.Model
{
    public class PullGroupListEvent : ProtocolEvent
    {
        /// <summary>
        /// <b>[In]</b> <br/>
        /// Self uin <br/>
        /// </summary>
        public uint SelfUin { get; }

        /// <summary>
        /// <b>[Out]</b> <br/>
        /// Group info list <br/>
        /// </summary>
        public List<BotGroup> GroupInfo { get; }

        private PullGroupListEvent(uint selfUin)
            : base(2000, true)
        {
            SelfUin = selfUin;
        }

        private PullGroupListEvent(int resultCode,
            List<BotGroup> groupInfo) : base(resultCode)
        {
            GroupInfo = groupInfo;
        }

        /// <summary>
        /// Construct event request
        /// </summary>
        /// <param name="selfUin"></param>
        /// <returns></returns>
        internal static PullGroupListEvent Create(uint selfUin)
            => new(selfUin);

        /// <summary>
        /// Construct event result
        /// </summary>
        /// <param name="resultCode"></param>
        /// <param name="groupInfo"></param>
        /// <returns></returns>
        internal static PullGroupListEvent Result(int resultCode,
            List<BotGroup> groupInfo) => new(resultCode, groupInfo);
    }
}
