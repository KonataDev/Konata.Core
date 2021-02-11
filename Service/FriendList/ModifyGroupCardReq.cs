using System;

using Konata.Core.Event;
using Konata.Core.Event.EventModel;
using Konata.Core.Packet;
using Konata.Core.Packet.SvcRequest;

namespace Konata.Core.Service.Friendlist
{
    [Service("friendlist.ModifyGroupCardReq", "Modify group card")]
    [EventDepends(typeof(GroupModifyMemberCardEvent))]
    public class ModifyGroupCardReq : IService
    {
        public bool Parse(SSOFrame input, SignInfo signInfo, out ProtocolEvent output)
        {
            throw new NotImplementedException();
        }

        public bool Build(Sequence sequence, GroupModifyMemberCardEvent input, SignInfo signInfo,
            out int newSequence, out byte[] output)
        {
            output = null;
            newSequence = sequence.NewSequence;

            var svcRequest = new SvcReqModifyGroupCard(input.GroupUin, input.MemberUin, input.MemberCard);

            if (SSOFrame.Create("friendlist.ModifyGroupCardReq", PacketType.TypeB,
               newSequence, sequence.Session, svcRequest, out var ssoFrame))
            {
                if (ServiceMessage.Create(ssoFrame, AuthFlag.D2Authentication,
                    signInfo.UinInfo.Uin, signInfo.D2Token, signInfo.D2Key, out var toService))
                {
                    return ServiceMessage.Build(toService, out output);
                }
            }

            return false;
        }

        public bool Build(Sequence sequence, ProtocolEvent input, SignInfo signInfo,
            out int newSequence, out byte[] output)
            => Build(sequence, (GroupModifyMemberCardEvent)input, signInfo, out newSequence, out output);
    }
}
