using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.Oidb.Model;
using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Utils.Protobuf;

// ReSharper disable UnusedType.Global
// ReSharper disable RedundantAssignment

namespace Konata.Core.Components.Services.OidbSvc;

[EventSubscribe(typeof(GroupKickMemberEvent))]
[Service("OidbSvc.0x8a0_1", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.Managed)]
internal class Oidb0x8a0_1 : BaseService<GroupKickMemberEvent>
{
    protected override bool Parse(SSOFrame input,
        BotKeyStore keystore, out GroupKickMemberEvent output)
    {
        var tree = ProtoTreeRoot.Deserialize
            (input.Payload.GetBytes(), true);
        {
            output = GroupKickMemberEvent
                .Result((int) tree.GetLeafVar("18"));
            return true;
        }
    }

    protected override bool Build(int sequence, GroupKickMemberEvent input,
        BotKeyStore keystore, BotDevice device, ref PacketBase output)
    {
        output = new OidbCmd0x8a0_1(input.GroupUin, input.MemberUin, input.ToggleType);
        return true;
    }
}
