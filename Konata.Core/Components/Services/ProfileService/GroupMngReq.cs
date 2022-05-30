using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.SvcRequest;
using Konata.Core.Packets.SvcResponse;

// ReSharper disable UnusedType.Global
// ReSharper disable RedundantAssignment

namespace Konata.Core.Components.Services.ProfileService;

[EventSubscribe(typeof(GroupLeaveEvent))]
[Service("ProfileService.GroupMngReq", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.Managed)]
internal class GroupMngReq : BaseService<GroupLeaveEvent>
{
    protected override bool Parse(SSOFrame input, AppInfo appInfo,
        BotKeyStore keystore, out GroupLeaveEvent output)
    {
        var root = new SvcRspGroupMng(input.Payload.GetBytes());
        output = GroupLeaveEvent.Result(root.Result);
        return true;
    }

    protected override bool Build(int sequence, GroupLeaveEvent input, AppInfo appInfo,
        BotKeyStore keystore, BotDevice device, ref PacketBase output)
    {
        output = new SvcReqGroupMngReq(input.SelfUin, input.GroupUin, input.Dismiss);
        return true;
    }
}
