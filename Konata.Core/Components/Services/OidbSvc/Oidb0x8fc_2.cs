using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.Oidb.Model;
using Konata.Core.Attributes;
using Konata.Core.Common;

// ReSharper disable UnusedType.Global
// ReSharper disable RedundantAssignment

namespace Konata.Core.Components.Services.OidbSvc;

[EventSubscribe(typeof(GroupSpecialTitleEvent))]
[Service("OidbSvc.0x8fc_2", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.Managed)]
internal class Oidb0x8fc_2 : BaseService<GroupSpecialTitleEvent>
{
    protected override bool Parse(SSOFrame input,
        BotKeyStore keystore, out GroupSpecialTitleEvent output)
    {
        // TODO: parse result
        output = GroupSpecialTitleEvent.Result(0);
        return true;
    }

    protected override bool Build(int sequence, GroupSpecialTitleEvent input,
        BotKeyStore keystore, BotDevice device, ref PacketBase output)
    {
        output = new OidbCmd0x8fc_2(input.GroupUin,
            input.MemberUin, input.SpecialTitle, input.ExpiredTime);
        return true;
    }
}
