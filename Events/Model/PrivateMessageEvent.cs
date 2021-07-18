using System;
using System.Collections.Generic;

using Konata.Core.Message;

namespace Konata.Core.Events.Model
{
    public class PrivateMessageEvent : ProtocolEvent
    {
        internal byte[] SyncCookie { get; set; }

        public uint FriendUin { get; set; }

        public MessageChain Message { get; set; }

        public bool IsFromTemporary { get; set; }

        public uint SliceTotal { get; set; }

        public uint SliceIndex { get; set; }

        public uint SliceFlags { get; set; }

        public override string ToString()
            => Message.ToString();
    }
}
