using System;
using System.Text;

using Konata.Core.Event;
using Konata.Core.Packet;
using Konata.Utils.Protobuf;
using Konata.Runtime.Base.Event;

namespace Konata.Core.Service.OnlinePush
{
    [SSOService("OnlinePush.PbPushGroupMsg", "Receive group message from server")]
    public class PbPushGroupMsg : ISSOService
    {
        public bool HandleInComing(EventSsoFrame ssoFrame, out KonataEventArgs output)
        {
            var protoRoot = new ProtoTreeRoot(ssoFrame.Payload.GetBytes());
            {
                // Simplest way to read group message.
                output = new EventGroupMessage
                {
                    GroupUin = (uint)protoRoot.GetLeafVar("0A.0A.4A.08", out var _),
                    GroupName = protoRoot.GetLeafString("0A.0A.4A.42", out var _),

                    MemberUin = (uint)protoRoot.GetLeafVar("0A.0A.08", out var _),
                    MemberCard = protoRoot.GetLeafString("0A.0A.4A.22", out var _),

                    MessageContent = protoRoot.GetLeafString("0A.1A.0A.12.0A.0A", out var _),
                };
            }

            return true;
        }

        public bool HandleOutGoing(KonataEventArgs eventArg, out byte[] output)
            => (output = null) == null;
    }
}
