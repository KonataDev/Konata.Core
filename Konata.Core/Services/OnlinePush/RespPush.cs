using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.SvcRequest;

// ReSharper disable UnusedType.Global
// ReSharper disable RedundantAssignment

namespace Konata.Core.Services.OnlinePush;

[EventSubscribe(typeof(OnlineRespPushEvent))]
[Service("OnlinePush.RespPush", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.Managed)]
internal class RespPush : BaseService<OnlineRespPushEvent>
{
    protected override bool Build(int sequence, OnlineRespPushEvent input,
        BotKeyStore keystore, BotDevice device, ref PacketBase output)
    {
        output = new SvcReqPushMsgResp(input.RequestId, input.SelfUin, input.FromSource,
            input.UnknownV1, input.SvrIp, input.UnknownV8, input.UnknownV32);
        return true;
    }
}
