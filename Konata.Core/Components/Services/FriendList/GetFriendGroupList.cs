using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.SvcRequest;
using Konata.Core.Packets.SvcResponse;

// ReSharper disable RedundantAssignment
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global

namespace Konata.Core.Components.Services.Friendlist;

[EventSubscribe(typeof(PullFriendListEvent))]
[Service("friendlist.getFriendGroupList", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.Managed)]
internal class GetFriendGroupList : BaseService<PullFriendListEvent>
{
    protected override bool Parse(SSOFrame input,
        BotKeyStore keystore, out PullFriendListEvent output)
    {
        var response = new SvcRspGetFriendListResp(input.Payload.GetBytes());
        output = PullFriendListEvent.Result(response.Result, response.ErrorCode, response.Friends, response.TotalFriendCount);
       
        return true;
    }

    protected override bool Build(int sequence, PullFriendListEvent input,
        BotKeyStore keystore, BotDevice device, ref PacketBase output)
    {
        output = new SvcReqGetFriendListReq(input.SelfUin, input.StartIndex, input.LimitNum);
        return true;
    }
}
