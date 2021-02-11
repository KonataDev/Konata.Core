using System;

using Konata.Core.Packet;
using Konata.Core.Packet.SvcRequest;
using Konata.Core.Event;
using Konata.Core.Event.EventModel;

namespace Konata.Core.Service.Friendlist
{
    [Service("friendlist.GetTroopListReqV2", "Pull group list")]
    [EventDepends(typeof(PullTroopListEvent))]
    public class GetTroopListReqV2 : IService
    {
        public bool Parse(SSOFrame input, SignInfo signInfo, out ProtocolEvent output)
        {
            throw new NotImplementedException();
        }

        public bool Build(Sequence sequence, PullTroopListEvent input, SignInfo signInfo,
            out int newSequence, out byte[] output)
        {
            output = null;
            newSequence = sequence.NewSequence;

            var svcRequest = new SvcReqGetTroopListReqV2Simplify(input.SelfUin);

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
            => Build(sequence, (PullTroopListEvent)input, signInfo, out newSequence, out output);
    }
}
