using System;

using Konata.Core.Event;
using Konata.Core.Event.EventModel;
using Konata.Core.Packet;
using Konata.Core.Packet.Oidb.OidbModel;

namespace Konata.Core.Service.OidbSvc
{
    [Service("OidbSvc.0x570_8", "Mute member in the group")]
    [EventDepends(typeof(GroupMuteMemberEvent))]
    public class Oidb0x570_8 : IService
    {
        public bool Parse(SSOFrame input, BotKeyStore signInfo, out ProtocolEvent output)
        {
            throw new NotImplementedException();
        }

        public bool Build(Sequence sequence, GroupMuteMemberEvent input,
            BotKeyStore signInfo, BotDevice device, out int newSequence, out byte[] output)
        {
            output = null;
            newSequence = sequence.NewSequence;

            var oidbRequest = new OidbCmd0x570_8(input.GroupUin, input.MemberUin, input.TimeSeconds ?? uint.MaxValue);

            if (SSOFrame.Create("OidbSvc.0x570_8", PacketType.TypeB,
                newSequence, sequence.Session, oidbRequest, out var ssoFrame))
            {
                if (ServiceMessage.Create(ssoFrame, AuthFlag.D2Authentication,
                    signInfo.Account.Uin, signInfo.Session.D2Token, signInfo.Session.D2Key, out var toService))
                {
                    return ServiceMessage.Build(toService, device, out output);
                }
            }

            return false;
        }

        public bool Build(Sequence sequence, ProtocolEvent input,
            BotKeyStore signInfo, BotDevice device, out int newSequence, out byte[] output)
            => Build(sequence, (GroupMuteMemberEvent)input, signInfo, device, out newSequence, out output);
    }
}
