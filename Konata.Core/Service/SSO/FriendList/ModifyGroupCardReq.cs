using System;
using System.Text;

using Konata.Core.Event;
using Konata.Core.Packet;
using Konata.Core.Manager;
using Konata.Runtime.Base.Event;
using Konata.Core.Packet.SvcRequest;

namespace Konata.Core.Service.Friendlist
{
    [SSOService("friendlist.ModifyGroupCardReq", "Modify group card")]
    public class ModifyGroupCardReq : ISSOService
    {
        public bool HandleInComing(EventSsoFrame ssoFrame, out KonataEventArgs output)
           => throw new NotImplementedException();

        public bool HandleOutGoing(KonataEventArgs eventArg, out byte[] output)
        {
            output = null;

            if (eventArg is EventGroupModifyMemberCard e)
            {
                var sigManager = e.Owner.GetComponent<UserSigManager>();
                var ssoManager = e.Owner.GetComponent<SsoInfoManager>();
                var svcRequest = new SvcReqModifyGroupCard(e.GroupUin, e.MemberUin, e.MemberCard);

                if (EventSsoFrame.Create("friendlist.ModifyGroupCardReq", PacketType.TypeB,
                    ssoManager.NewSequence, ssoManager.Session, svcRequest, out var ssoFrame))
                {
                    if (EventServiceMessage.Create(ssoFrame, AuthFlag.D2Authentication,
                        sigManager.Uin, sigManager.D2Token, sigManager.D2Key, out var toService))
                    {
                        return EventServiceMessage.Build(toService, out output);
                    }
                }
            }

            return false;
        }
    }
}
