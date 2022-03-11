// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Konata.Core.Events.Model
{
    public class GroupMessageRecallEvent : ProtocolEvent
    {
        /// <summary>
        /// <b>[In] [Out]</b> <br/>
        /// Group uin <br/>
        /// </summary>
        public uint GroupUin { get; }

        /// <summary>
        /// <b>[In] [Out]</b> <br/>
        /// Member uin <br/>
        /// </summary>
        public uint MemberUin { get; }

        /// <summary>
        /// <b>[Opt] [Out]</b> <br/>
        /// Operator uin <br/>
        /// </summary>
        public uint OperatorUin { get; }

        /// <summary>
        /// <b>[Opt] [Out]</b> <br/>
        /// Recall suffix <br/>
        /// </summary>
        public string RecallSuffix { get; }

        /// <summary>
        /// <b>[In]</b> <br/>
        /// Message id <br/>
        /// </summary>
        public uint MessageId { get; }

        private GroupMessageRecallEvent(uint groupUin, uint memberUin,
            uint messageId) : base(2000, true)
        {
            GroupUin = groupUin;
            MemberUin = memberUin;
            MessageId = messageId;
        }

        private GroupMessageRecallEvent(int resultCode)
            : base(resultCode)
        {
        }

        private GroupMessageRecallEvent(uint groupUin, uint memberUin,
            uint operatorUin, string recallSuffix) : base(0)
        {
            GroupUin = groupUin;
            MemberUin = memberUin;
            OperatorUin = operatorUin;
            RecallSuffix = recallSuffix;
        }

        /// <summary>
        /// Construct event request
        /// </summary>
        /// <param name="groupUin"></param>
        /// <param name="memberUin"></param>
        /// <param name="messageId"></param>
        /// <returns></returns>
        internal static GroupMessageRecallEvent Create(uint groupUin,
            uint memberUin, uint messageId) => new(groupUin, memberUin, messageId);

        /// <summary>
        /// Construct event result
        /// </summary>
        /// <param name="resultCode"></param>
        /// <returns></returns>
        internal static GroupMessageRecallEvent Result(int resultCode)
            => new(resultCode);

        /// <summary>
        /// Construct event push
        /// </summary>
        /// <param name="groupUin"></param>
        /// <param name="memberUin"></param>
        /// <param name="operatorUin"></param>
        /// <param name="recallSuffix"></param>
        /// <returns></returns>
        internal static GroupMessageRecallEvent Push(uint groupUin, uint memberUin,
            uint operatorUin, string recallSuffix) => new(groupUin, memberUin, operatorUin, recallSuffix);
    }
}
