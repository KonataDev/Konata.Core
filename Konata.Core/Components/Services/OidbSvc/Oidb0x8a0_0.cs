using System;
using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.Oidb.Model;
using Konata.Core.Attributes;
using Konata.Core.Common;

// ReSharper disable InconsistentNaming
// ReSharper disable RedundantAssignment
// ReSharper disable UnusedType.Global

namespace Konata.Core.Components.Services.OidbSvc;

[EventSubscribe(typeof(GroupKickMembersEvent))]
[Service("OidbSvc.0x8a0_0", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.Managed)]
internal class Oidb0x8a0_0 : BaseService<GroupKickMembersEvent>
{
    protected override bool Parse(SSOFrame input, AppInfo appInfo,
        BotKeyStore keystore, out GroupKickMembersEvent output)
    {
        throw new NotImplementedException();
    }

    protected override bool Build(int sequence, GroupKickMembersEvent input, AppInfo appInfo,
        BotKeyStore keystore, BotDevice device, ref PacketBase output)
    {
        output = new OidbCmd0x8a0_0(input.GroupUin, input.MembersUin, input.ToggleType);
        return true;
    }
}
