using System;
using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.Oidb.Model;
using Konata.Core.Attributes;
using Konata.Core.Common;

namespace Konata.Core.Services.OidbSvc;

[Service("OidbSvc.0x55c_1", "Promote admin for member")]
[EventSubscribe(typeof(GroupPromoteAdminEvent))]
internal class Oidb0x55c_1 : IService
{
    public bool Parse(SSOFrame input, BotKeyStore keystore, out ProtocolEvent output)
    {
        throw new NotImplementedException();
    }

    public bool Build(Sequence sequence, GroupPromoteAdminEvent input,
        BotKeyStore keystore, BotDevice device, out int newSequence, out byte[] output)
    {
        output = null;
        newSequence = sequence.NewSequence;

        var oidbRequest = new OidbCmd0x55c_1(input.GroupUin, input.MemberUin, input.ToggleType);

        if (SSOFrame.Create("OidbSvc.0x55c_1", PacketType.TypeB,
                newSequence, sequence.Session, oidbRequest, out var ssoFrame))
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
        => Build(sequence, (GroupPromoteAdminEvent) input, keystore, device, out newSequence, out output);
}
