using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.Oidb.Model;
using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Utils.Protobuf;

namespace Konata.Core.Services.OidbSvc;

[EventSubscribe(typeof(GroupMuteMemberEvent))]
[Service("OidbSvc.0x570_8", "Mute member in the group")]
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

    protected override bool Build(Sequence sequence, GroupMuteMemberEvent input, BotKeyStore
        keystore, BotDevice device, out int newSequence, out byte[] output)
    {
        output = null;
        newSequence = sequence.NewSequence;

        var oidbRequest = new OidbCmd0x570_8(input.GroupUin,
            input.MemberUin, input.TimeSeconds);

        if (SSOFrame.Create("OidbSvc.0x570_8", PacketType.TypeB,
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
}
