using Konata.Core.Attributes;
using Konata.Core.Common;
using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.SvcRequest;

namespace Konata.Core.Services.ProfileService
{
    [Service("ProfileService.GroupMngReq", "Group management request")]
    internal class GroupMngReq : BaseService<GroupManagementEvent>
    {
        protected override bool Parse(SSOFrame input,
            BotKeyStore keystore, out GroupManagementEvent output)
        {
            return base.Parse(input, keystore, out output);
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
