using System;
using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.SvcRequest;
using Konata.Core.Attributes;
using Konata.Core.Common;

// ReSharper disable UnusedType.Global
// ReSharper disable RedundantAssignment

namespace Konata.Core.Components.Services.Friendlist;

[EventSubscribe(typeof(GroupModifyMemberCardEvent))]
[Service("friendlist.ModifyGroupCardReq", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.Managed)]
internal class ModifyGroupCardReq : BaseService<GroupModifyMemberCardEvent>
{
    protected override bool Parse(SSOFrame input,
        BotKeyStore keystore, out GroupModifyMemberCardEvent output)
    {
        throw new NotImplementedException();
    }

    protected override bool Build(int sequence, GroupModifyMemberCardEvent input,
        BotKeyStore keystore, BotDevice device, ref PacketBase output)
    {
        output = new SvcReqModifyGroupCard(input.GroupUin, input.MemberUin, input.MemberCard);
        return true;
    }
}
