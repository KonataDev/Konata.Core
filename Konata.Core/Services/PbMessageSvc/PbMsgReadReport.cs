using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.Protobuf;
using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Utils.Protobuf;

namespace Konata.Core.Services.PbMessageSvc;

[Service("PbMessageSvc.PbMsgReadedReport", "Push read signal")]
[EventSubscribe(typeof(GroupMessageReadEvent))]
internal class PbMsgReadReport : IService
{
    public bool Parse(SSOFrame input, BotKeyStore keystore, out ProtocolEvent output)
        => (output = null) == null;

    public bool Build(Sequence sequence, GroupMessageReadEvent input,
        BotKeyStore keystore, BotDevice device, out int newSequence, out byte[] output)
    {
        output = null;
        newSequence = input.SessionSequence;

        var readReport = new GroupMsgReadReport(input.GroupUin, input.RequestId);

        if (SSOFrame.Create("PbMessageSvc.PbMsgReadedReport", PacketType.TypeB,
                newSequence, sequence.Session, ProtoTreeRoot.Serialize(readReport), out var ssoFrame))
        {
            if (ServiceMessage.Create(ssoFrame, AuthFlag.D2Authentication,
                    keystore.Account.Uin, keystore.Session.D2Token, keystore.Session.D2Key, out var toService))
            {
                return ServiceMessage.Build(toService, device, out output);
            }
        }

        return false;
    }

    public bool Build(Sequence sequence, ProtocolEvent input,
        BotKeyStore keystore, BotDevice device, out int newSequence, out byte[] output)
        => Build(sequence, (GroupMessageReadEvent) input, keystore, device, out newSequence, out output);
}
