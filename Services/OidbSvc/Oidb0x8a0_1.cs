using System;
using System.Text;

using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.Oidb.Model;
using Konata.Core.Attributes;

namespace Konata.Core.Services.OidbSvc
{
    [Service("OidbSvc.0x8a0_1", "Kick member in the group")]
    [EventSubscribe(typeof(GroupKickMemberEvent))]
    class Oidb0x8a0_1 : IService
    {
        public bool Parse(SSOFrame input, BotKeyStore keystore, out ProtocolEvent output)
        {
            throw new NotImplementedException();
        }

        public bool Build(Sequence sequence, GroupKickMemberEvent input,
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

        public bool Build(Sequence sequence, ProtocolEvent input,
            BotKeyStore keystore, BotDevice device, out int newSequence, out byte[] output)
            => Build(sequence, (GroupKickMemberEvent)input, keystore, device, out newSequence, out output);

    }
}
