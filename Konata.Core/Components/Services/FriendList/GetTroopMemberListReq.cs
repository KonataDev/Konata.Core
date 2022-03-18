using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.SvcRequest;
using Konata.Core.Packets.SvcResponse;

// ReSharper disable RedundantAssignment
// ReSharper disable UnusedType.Global

namespace Konata.Core.Components.Services.Friendlist;

[EventSubscribe(typeof(PullGroupMemberListEvent))]
[Service("friendlist.GetTroopMemberListReq", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.Managed)]
internal class GetTroopMemberListReq : BaseService<PullGroupMemberListEvent>
{
    protected override bool Parse(SSOFrame input,
        BotKeyStore keystore, out PullGroupMemberListEvent output)
    {
        var response = new SvcRspGetTroopMemberListResp(input.Payload.GetBytes());
        {
            output = PullGroupMemberListEvent.Result(
                response.Result, response.ErrorCode, response.GroupUin,
                response.GroupCode, response.Members, response.NextUin
            );
        }
        return true;
    }

    protected override bool Build(int sequence, PullGroupMemberListEvent input,
        BotKeyStore keystore, BotDevice device, ref PacketBase output)
    {
        output = new SvcReqGetTroopMemberListReq
            (input.SelfUin, input.GroupUin, input.GroupCode, input.NextUin);
        return true;
    }
}
