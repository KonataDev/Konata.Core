using System;
using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.SvcRequest;
using Konata.Core.Attributes;
using Konata.Core.Common;

namespace Konata.Core.Services.Friendlist;

[Service("friendlist.ModifyGroupCardReq", "Modify group card")]
[EventSubscribe(typeof(GroupModifyMemberCardEvent))]
internal class ModifyGroupCardReq : IService
{
    public bool Parse(SSOFrame input, BotKeyStore keystore, out ProtocolEvent output)
    {
        throw new NotImplementedException();
    }

    public bool Build(Sequence sequence, GroupModifyMemberCardEvent input,
        BotKeyStore keystore, BotDevice device, out int newSequence, out byte[] output)
    {
        output = null;
        newSequence = sequence.NewSequence;

        var svcRequest = new SvcReqModifyGroupCard(input.GroupUin, input.MemberUin, input.MemberCard);

        if (SSOFrame.Create("friendlist.ModifyGroupCardReq", PacketType.TypeB,
                newSequence, sequence.Session, svcRequest, out var ssoFrame))
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
        => Build(sequence, (GroupModifyMemberCardEvent) input, keystore, device, out newSequence, out output);
}
