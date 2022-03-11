using Konata.Core.Events.Model;
using Konata.Core.Packets;
using Konata.Core.Packets.Oidb.Model;
using Konata.Core.Attributes;
using Konata.Core.Common;

// ReSharper disable UnusedType.Global

namespace Konata.Core.Services.OidbSvc
{
    [EventSubscribe(typeof(GroupSpecialTitleEvent))]
    [Service("OidbSvc.0x8fc_2", "Set special title")]
    public class Oidb0x8fc_2 : BaseService<GroupSpecialTitleEvent>
    {
        protected override bool Parse(SSOFrame input,
            BotKeyStore keystore, out GroupSpecialTitleEvent output)
        {
            // TODO: parse result
            output = GroupSpecialTitleEvent.Result(0);
            return true;
        }

        protected override bool Build(Sequence sequence, GroupSpecialTitleEvent input,
            BotKeyStore keystore, BotDevice device, out int newSequence, out byte[] output)
        {
            output = null;
            newSequence = sequence.NewSequence;

            var oidbRequest = new OidbCmd0x8fc_2(input.GroupUin,
                input.MemberUin, input.SpecialTitle, input.ExpiredTime);

            if (SSOFrame.Create("OidbSvc.0x8fc_2", PacketType.TypeB,
                newSequence, sequence.Session, oidbRequest, out var ssoFrame))
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
