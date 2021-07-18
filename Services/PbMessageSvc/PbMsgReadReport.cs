using System;

using Konata.Core.Events;
using Konata.Core.Events.EventModel;
using Konata.Core.Packets;
using Konata.Core.Packets.Protobuf;
using Konata.Core.Attributes;
using Konata.Utils.Protobuf;

namespace Konata.Core.Services.PbMessageSvc
{
    [Service("PbMessageSvc.PbMsgReadedReport", "Push read signal")]
    [EventDepends(typeof(GroupMessageReadEvent))]
    internal class PbMsgReadReport : IService
    {
        public bool Parse(SSOFrame input, BotKeyStore signInfo, out ProtocolEvent output)
            => (output = null) == null;

        public bool Build(Sequence sequence, GroupMessageReadEvent input,
            BotKeyStore signInfo, BotDevice device, out int newSequence, out byte[] output)
        {
            output = null;
            newSequence = input.SessionSequence;

            var readReport = new GroupMsgReadedReport(input.GroupUin, input.RequestId);

            if (SSOFrame.Create("PbMessageSvc.PbMsgReadedReport", PacketType.TypeB,
                newSequence, sequence.Session, ProtoTreeRoot.Serialize(readReport), out var ssoFrame))
            {
                if (ServiceMessage.Create(ssoFrame, AuthFlag.D2Authentication,
                    signInfo.Account.Uin, signInfo.Session.D2Token, signInfo.Session.D2Key, out var toService))
                {
                    return ServiceMessage.Build(toService, device, out output);
                }
            }

            return false;
        }

        public bool Build(Sequence sequence, ProtocolEvent input,
            BotKeyStore signInfo, BotDevice device, out int newSequence, out byte[] output)
              => Build(sequence, (GroupMessageReadEvent)input, signInfo, device, out newSequence, out output);
    }
}
