using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.Oidb.Model;
using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Utils.Protobuf;

// ReSharper disable UnusedType.Global
// ReSharper disable RedundantAssignment

namespace Konata.Core.Components.Services.OidbSvc;

[EventSubscribe(typeof(GroupMuteMemberEvent))]
[Service("OidbSvc.0x570_8", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.Managed)]
internal class Oidb0x570_8 : BaseService<GroupMuteMemberEvent>
{
    protected override bool Parse(SSOFrame input,
        BotKeyStore keystore, out GroupMuteMemberEvent output)
    {
        var tree = ProtoTreeRoot.Deserialize
            (input.Payload.GetBytes(), true);
        {
            output = GroupMuteMemberEvent
                .Result((int) tree.GetLeafVar("18"));
            return true;
        }
    }

    protected override bool Build(int sequence, GroupMuteMemberEvent input,
        BotKeyStore keystore, BotDevice device, ref PacketBase output)
    {
        output = new OidbCmd0x570_8(input.GroupUin, input.MemberUin, input.TimeSeconds);
        return true;
    }
}
