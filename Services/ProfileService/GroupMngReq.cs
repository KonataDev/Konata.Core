using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.SvcRequest;
using Konata.Core.Packets.SvcResponse;

namespace Konata.Core.Services.ProfileService
{
    [EventSubscribe(typeof(GroupManagementEvent))]
    [Service("ProfileService.GroupMngReq", "Group management request")]
    internal class GroupMngReq : BaseService<GroupManagementEvent>
    {
        protected override bool Parse(SSOFrame input,
            BotKeyStore keystore, out GroupManagementEvent output)
        {
            var root = new SvcRspGroupMng(input.Payload.GetBytes());
            output = GroupManagementEvent.Result(root.Result);
            return true;
        }

        protected override bool Build(Sequence sequence, GroupManagementEvent input,
            BotKeyStore keystore, BotDevice device, out int newSequence, out byte[] output)
        {
            output = null;
            newSequence = sequence.NewSequence;

            var svcRequest = new SvcReqGroupMngReq(input.SelfUin, input.GroupCode, input.Dismiss);

            if (SSOFrame.Create("ProfileService.GroupMngReq", PacketType.TypeB,
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
}
