using System;
using System.Collections.Generic;

using Konata.Core.Message;

namespace Konata.Core.Event.EventModel
{
    public class GroupMessageEvent : ProtocolEvent
    {
        public string GroupName { get; set; }

        public uint GroupUin { get; set; }

        public uint MemberUin { get; set; }

        public string MemberCard { get; set; }

        public List<MessageChain> Message { get; set; }

        public uint MessageId { get; set; }

        public uint MessageTime { get; set; }

        public uint SliceTotal { get; set; }

        public uint SliceIndex { get; set; }

        public uint SliceFlags { get; set; }

        public override string ToString()
        {
            if (Message == null)
                return "";

            var content = "";
            foreach (var element in Message)
            {
                content += element.ToString();
            }

            return content;
        }

        public GroupMessageEvent()
            => WaitForResponse = true;
    }
}
