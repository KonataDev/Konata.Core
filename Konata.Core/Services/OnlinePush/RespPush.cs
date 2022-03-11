using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.SvcRequest;

namespace Konata.Core.Services.OnlinePush;

[EventSubscribe(typeof(OnlineRespPushEvent))]
[Service("OnlinePush.RespPush", "Confirm online push")]
public class RespPush : BaseService<OnlineRespPushEvent>
{
    protected override bool Build(Sequence sequence, OnlineRespPushEvent input,
        BotKeyStore keystore, BotDevice device, out int newSequence, out byte[] output)
    {
        output = null;
        newSequence = sequence.NewSequence;

        var svcRequest = new SvcReqPushMsgResp(input.RequestId, input.SelfUin, input.FromSource,
            input.UnknownV1, input.SvrIp, input.UnknownV8, input.UnknownV32);

        if (SSOFrame.Create("OnlinePush.RespPush", PacketType.TypeB,
                newSequence, sequence.Session, svcRequest, out var ssoFrame))
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
