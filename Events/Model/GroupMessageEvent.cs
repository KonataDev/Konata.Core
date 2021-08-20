using Konata.Core.Message;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Konata.Core.Events.Model
{
    public class GroupMessageEvent : ProtocolEvent
    {
        /// <summary>
        /// <b>[In] [Out]</b>     <br/>
        /// Group uin
        /// </summary>
        public uint GroupUin { get; private set; }
        
        /// <summary>
        /// <b>[Out]</b>          <br/>
        /// Group name
        /// </summary>
        public string GroupName { get; private set; }

        /// <summary>
        /// <b>[In] [Out]</b> <br/>
        /// Member uin <br/>
        /// </summary>
        public uint MemberUin { get; private set; }

        /// <summary>
        /// <b>[Out]</b> <br/>
        /// Member card name <br/>
        /// </summary>
        public string MemberCard { get; private set; }

        /// <summary>
        /// <b>[In] [Out]</b> <br/>
        /// Message chain <br/>
        /// </summary>
        public MessageChain Message { get; private set; }

        /// <summary>
        /// <b>[Out]</b> <br/>
        /// Message id <br/>
        /// </summary>
        public uint MessageId { get; private set; }

        /// <summary>
        /// <b>[Out]</b> <br/>
        /// Message time <br/>
        /// </summary>
        public uint MessageTime { get; private set; }

        /// <summary>
        /// <b>[Opt] [Out]</b> <br/>
        /// Total slice count <br/>
        /// </summary>
        public uint SliceTotal { get; private set; }

        /// <summary>
        /// <b>[Opt] [Out]</b> <br/>
        /// Current slice id <br/>
        /// </summary>
        public uint SliceIndex { get; private set; }

        /// <summary>
        /// <b>[Opt] [Out]</b> <br/>
        /// Slice flags <br/>
        /// </summary>
        public uint SliceFlags { get; private set; }

        private GroupMessageEvent(uint groupUin, uint selfUin,
            MessageChain messageChain) : base(2000, true)
        {
            GroupUin = groupUin;
            MemberUin = selfUin;
            Message = messageChain;
        }

        private GroupMessageEvent(int resultCode)
            : base(resultCode)
        {
        }

        /// <summary>
        /// Construct event request
        /// </summary>
        /// <param name="groupUin"></param>
        /// <param name="selfUin"></param>
        /// <param name="messageChain"></param>
        /// <returns></returns>
        internal static GroupMessageEvent Create(uint groupUin, uint selfUin,
            MessageChain messageChain) => new(groupUin, selfUin, messageChain);

        /// <summary>
        /// Construct event result
        /// </summary>
        /// <param name="resultCode"></param>
        /// <returns></returns>
        internal static GroupMessageEvent Result(int resultCode)
            => new(resultCode);

        /// <summary>
        /// Set group name
        /// </summary>
        /// <param name="groupName"></param>
        internal void SetGroupName(string groupName)
            => GroupName = groupName;

        /// <summary>
        /// Set group uin
        /// </summary>
        /// <param name="groupUin"></param>
        internal void SetGroupUin(uint groupUin)
            => GroupUin = groupUin;

        /// <summary>
        /// Set message id
        /// </summary>
        /// <param name="messageId"></param>
        internal void SetMessageId(uint messageId)
            => MessageId = messageId;

        /// <summary>
        /// Set message time
        /// </summary>
        /// <param name="messageTime"></param>
        internal void SetMessageTime(uint messageTime)
            => MessageTime = messageTime;

        /// <summary>
        /// Set message 
        /// </summary>
        /// <param name="message"></param>
        internal void SetMessage(MessageChain message)
            => Message = message;

        /// <summary>
        /// Set slice info
        /// </summary>
        /// <param name="sliceTotal"></param>
        /// <param name="sliceIndex"></param>
        /// <param name="sliceFlags"></param>
        internal void SetSliceInfo(uint sliceTotal,
            uint sliceIndex, uint sliceFlags)
        {
            SliceTotal = sliceTotal;
            SliceIndex = sliceIndex;
            SliceFlags = sliceFlags;
        }

        /// <summary>
        /// Set member uin
        /// </summary>
        /// <param name="memberUin"></param>
        internal void SetMemberUin(uint memberUin)
            => MemberUin = memberUin;

        /// <summary>
        /// Set member card
        /// </summary>
        /// <param name="memberCard"></param>
        internal void SetMemberCard(string memberCard)
            => MemberCard = memberCard;
    }
}
