using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.Oidb.Model;
using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Utils.Protobuf;

namespace Konata.Core.Services.OidbSvc
{
    [Service("OidbSvc.0x8a0_1", "Kick a member from group")]
    [EventSubscribe(typeof(GroupKickMemberEvent))]
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

        protected override bool Build(Sequence sequence, GroupKickMemberEvent input,
            BotKeyStore keystore, BotDevice device, out int newSequence, out byte[] output)
        {
            output = null;
            newSequence = sequence.NewSequence;

            var oidbRequest = new OidbCmd0x8a0_1(input.GroupUin, input.MemberUin, input.ToggleType);

            if (SSOFrame.Create("OidbSvc.0x8a0_1", PacketType.TypeB,
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
}
