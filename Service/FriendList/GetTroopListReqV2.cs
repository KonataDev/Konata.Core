using System;

using Konata.Core.Packet;
using Konata.Core.Packet.SvcRequest;
using Konata.Core.Event;
using Konata.Core.Event.EventModel;
using Konata.Core.Attributes;

namespace Konata.Core.Service.Friendlist
{
    [Service("friendlist.GetTroopListReqV2", "Pull group list")]
    [EventDepends(typeof(PullTroopListEvent))]
    public class GetTroopListReqV2 : IService
    {
        public bool Parse(SSOFrame input, BotKeyStore signInfo, out ProtocolEvent output)
        {
            throw new NotImplementedException();
        }

        public bool Build(Sequence sequence, PullTroopListEvent input,
            BotKeyStore signInfo, BotDevice device, out int newSequence, out byte[] output)
        {
            output = null;
            newSequence = sequence.NewSequence;

            var svcRequest = new SvcReqGetTroopListReqV2Simplify(input.SelfUin);

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
            => Build(sequence, (PullTroopListEvent)input, signInfo, device, out newSequence, out output);
    }
}
