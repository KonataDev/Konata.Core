using Konata.Core.Message;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace Konata.Core.Events.Model
{
    public class PrivateMessageEvent : ProtocolEvent
    {
        /// <summary>
        /// <b>[In]</b> <br/>
        /// Self uin <br/>
        /// </summary>
        public uint SelfUin { get; private set; }

        /// <summary>
        /// <b>[In]</b> <br/>
        /// Friend uin <br/>
        /// </summary>
        public uint FriendUin { get; private set; }

        /// <summary>
        /// <b>[In]</b> <br/>
        /// Message chain <br/>
        /// </summary>
        public MessageChain Message { get; private set; }

        /// <summary>
        /// <b>[In]</b> <br/>
        /// Sync cookie <br/>
        /// </summary>
        internal byte[] SyncCookie { get; private set; }

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

        private PrivateMessageEvent(uint friendUin, uint selfUin,
            MessageChain messageChain) : base(2000, true)
        {
            FriendUin = friendUin;
            SelfUin = selfUin;
            Message = messageChain;
        }

        private PrivateMessageEvent(int resultCode)
            : base(resultCode)
        {
        }

        /// <summary>
        /// Construct event request
        /// </summary>
        /// <param name="friendUin"></param>
        /// <param name="selfUin"></param>
        /// <param name="messageChain"></param>
        /// <returns></returns>
        internal static PrivateMessageEvent Create(uint friendUin, uint selfUin,
            MessageChain messageChain) => new(friendUin, selfUin, messageChain);

        /// <summary>
        /// Construct event result
        /// </summary>
        /// <param name="resultCode"></param>
        /// <returns></returns>
        internal static PrivateMessageEvent Result(int resultCode)
            => new(resultCode);

        /// <summary>
        /// Construct event push
        /// </summary>
        /// <returns></returns>
        internal static PrivateMessageEvent Push()
            => new(0);

        /// <summary>
        /// Set friend uin
        /// </summary>
        /// <param name="friendUin"></param>
        internal void SetFriendUin(uint friendUin)
            => FriendUin = friendUin;

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
        /// Set sync cookie
        /// </summary>
        /// <param name="syncCookie"></param>
        internal void SetSyncCookie(byte[] syncCookie)
            => SyncCookie = syncCookie;
    }
}
