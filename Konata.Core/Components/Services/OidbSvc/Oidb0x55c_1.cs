using System;
using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.Oidb.Model;
using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Utils.Protobuf;

// ReSharper disable UnusedType.Global
// ReSharper disable RedundantAssignment

namespace Konata.Core.Components.Services.OidbSvc;

[EventSubscribe(typeof(GroupPromoteAdminEvent))]
[Service("OidbSvc.0x55c_1", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.Managed)]
internal class Oidb0x55c_1 : BaseService<GroupPromoteAdminEvent>
{
    protected override bool Parse(SSOFrame input, AppInfo appInfo, BotKeyStore keystore,
        out GroupPromoteAdminEvent output)
    {
        var tree = ProtoTreeRoot.Deserialize
            (input.Payload.GetBytes(), true);
        {
            output = GroupPromoteAdminEvent
                .Result((int)tree.GetLeafVar("18"));
            return true;
        }
    }

    protected override bool Build(int sequence, GroupPromoteAdminEvent input, AppInfo appInfo,
        BotKeyStore keystore, BotDevice device, ref PacketBase output)
    {
        output = new OidbCmd0x55c_1(input.GroupUin, input.MemberUin, input.ToggleType);
        return true;
    }
}
