using System;

using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.SvcRequest;
using Konata.Core.Attributes;

namespace Konata.Core.Services.Friendlist
{
    [Service("friendlist.ModifyGroupCardReq", "Modify group card")]
    [EventDepends(typeof(GroupModifyMemberCardEvent))]
    public class ModifyGroupCardReq : IService
    {
        public bool Parse(SSOFrame input, BotKeyStore signInfo, out ProtocolEvent output)
        {
            throw new NotImplementedException();
        }

        public bool Build(Sequence sequence, GroupModifyMemberCardEvent input,
            BotKeyStore signInfo, BotDevice device, out int newSequence, out byte[] output)
        {
            output = null;
            newSequence = sequence.NewSequence;

            var svcRequest = new SvcReqModifyGroupCard(input.GroupUin, input.MemberUin, input.MemberCard);

            if (SSOFrame.Create("friendlist.ModifyGroupCardReq", PacketType.TypeB,
               newSequence, sequence.Session, svcRequest, out var ssoFrame))
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
            => Build(sequence, (GroupModifyMemberCardEvent)input, signInfo, device, out newSequence, out output);
    }
}
