using Konata.Core.Packets;
using Konata.Core.Packets.SvcRequest;
using Konata.Core.Events;
using Konata.Core.Events.Model;
using Konata.Core.Attributes;
using Konata.Core.Packets.SvcResponse;

// ReSharper disable UnusedType.Global

namespace Konata.Core.Services.Friendlist
{
    [EventSubscribe(typeof(PullGroupListEvent))]
    [Service("friendlist.GetTroopListReqV2", "Pull group list")]
    public class GetTroopListReqV2 : IService
    {
        public bool Parse(SSOFrame input, BotKeyStore signInfo, out ProtocolEvent output)
        {
            var response = new SvcRspGetTroopListRespV2(input.Payload.GetBytes());

            output = new PullGroupListEvent
            {
                ResultCode = 0,
                GroupInfo = response.Groups
            };

            return true;
        }

        private bool Build(Sequence sequence, PullGroupListEvent input,
            BotKeyStore signInfo, BotDevice device, out int newSequence, out byte[] output)
        {
            output = null;
            newSequence = sequence.NewSequence;

            var svcRequest = new SvcReqGetTroopListReqV2Simplify(input.SelfUin);

            if (SSOFrame.Create("friendlist.GetTroopListReqV2", PacketType.TypeB,
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
            => Build(sequence, (PullGroupListEvent) input, signInfo, device, out newSequence, out output);
    }
}
