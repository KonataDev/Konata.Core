using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.SvcRequest;
using Konata.Core.Packets.SvcResponse;

// ReSharper disable UnusedType.Global
// ReSharper disable RedundantAssignment

namespace Konata.Core.Components.Services.ProfileService;

[EventSubscribe(typeof(GroupManagementEvent))]
[Service("ProfileService.GroupMngReq", PacketType.TypeB, AuthFlag.D2Authentication, SequenceMode.Managed)]
internal class GroupMngReq : BaseService<GroupManagementEvent>
{
    protected override bool Parse(SSOFrame input,
        BotKeyStore keystore, out GroupManagementEvent output)
    {
        var root = new SvcRspGroupMng(input.Payload.GetBytes());
        output = GroupManagementEvent.Result(root.Result);
        return true;
    }

    protected override bool Build(int sequence, GroupManagementEvent input,
        BotKeyStore keystore, BotDevice device, ref PacketBase output)
    {
        output = new SvcReqGroupMngReq(input.SelfUin, input.GroupCode, input.Dismiss);
        return true;
    }
}
