using System;

using Konata.Core.Event;
using Konata.Core.Event.EventModel;
using Konata.Core.Packet;
using Konata.Core.Packet.Protobuf;
using Konata.Utils.Protobuf;

namespace Konata.Core.Service.PbMessageSvc
{
    [Service("PbMessageSvc.PbMsgReadedReport", "Push read signal")]
    [EventDepends(typeof(GroupMessageReadEvent))]
    internal class PbMsgReadReport : IService
    {
        public bool Parse(SSOFrame input, SignInfo signInfo, out ProtocolEvent output)
            => (output = null) == null;

        public bool Build(Sequence sequence, GroupMessageReadEvent input, SignInfo signInfo,
            out int newSequence, out byte[] output)
        {
            output = null;
            newSequence = input.SessionSequence;

            var readReport = new GroupMsgReadedReport(input.GroupUin, input.RequestId);

            if (SSOFrame.Create("PbMessageSvc.PbMsgReadedReport", PacketType.TypeB,
                newSequence, sequence.Session, ProtoTreeRoot.Serialize(readReport), out var ssoFrame))
            {
                if (ServiceMessage.Create(ssoFrame, AuthFlag.D2Authentication,
                    signInfo.UinInfo.Uin, signInfo.D2Token, signInfo.D2Key, out var toService))
                {
                    return ServiceMessage.Build(toService, out output);
                }
            }

            return false;
        }

        public bool Build(Sequence sequence, ProtocolEvent input, SignInfo signInfo,
            out int newSequence, out byte[] output)
              => Build(sequence, (GroupMessageReadEvent)input, signInfo, out newSequence, out output);
    }
}
