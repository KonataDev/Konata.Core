using Konata.Core.Packets;
using Konata.Core.Packets.SvcRequest;
using Konata.Core.Events.Model;
using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Packets.SvcResponse;

// ReSharper disable RedundantAssignment
// ReSharper disable UnusedType.Global

namespace Konata.Core.Components.Services.Friendlist;

[EventSubscribe(typeof(PullGroupListEvent))]
[Service("friendlist.GetTroopListReqV2", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.Managed)]
internal class GetTroopListReqV2 : BaseService<PullGroupListEvent>
{
    protected override bool Parse(SSOFrame input, AppInfo appInfo,
        BotKeyStore keystore, out PullGroupListEvent output)
    {
        var response = new SvcRspGetTroopListRespV2(input.Payload.GetBytes());
        output = PullGroupListEvent.Result(0, response.Groups);

        return true;
    }

    protected override bool Build(int sequence, PullGroupListEvent input, AppInfo appInfo,
        BotKeyStore keystore, BotDevice device, ref PacketBase output)
    {
        output = new SvcReqGetTroopListReqV2Simplify(input.SelfUin);
        return true;
    }
}
