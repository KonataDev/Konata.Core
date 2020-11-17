using System;
using Konata.Events;
using Konata.Packets;
using Konata.Packets.Sso;
using Konata.Packets.SvcRequest;

namespace Konata.Services.FriendList
{
    public class ModifyGroupCardReq : ServiceRoutine
    {
        public ModifyGroupCardReq(EventPumper eventPumper)
            : base("friendlist.ModifyGroupCardReq", eventPumper)
        {

        }

        public override EventParacel OnEvent(EventParacel eventParacel)
        {
            if (eventParacel is EventGroupCtl groupCtl
                && groupCtl.Type == EventGroupCtl.EventType.SetGroupCard)
                return OnSendRequest(groupCtl.GroupUin, groupCtl.MemberUin, groupCtl.GroupCard);
            else if (eventParacel is EventSsoMessage ssoMsgEvent)
                return OnRecvResponse(ssoMsgEvent.PayloadMsg);
            return EventParacel.Reject;
        }

        private EventParacel OnSendRequest(uint groupUin,
            uint memberUin, string memberCard)
        {
            return CallEvent<SsoMan>(new EventDraftSsoMessage
            {
                EventDelegate = (EventParacel eventParacel) =>
                {
                    if (eventParacel is EventDraftSsoMessage sso)
                        return new EventSsoMessage
                        {
                            RequestFlag = RequestFlag.D2Authentication,
                            PayloadMsg = new SsoMessageTypeB(ServiceName, sso.Sequence, sso.Session,
                                new SvcReqModifyGroupCard(groupUin, memberUin, memberCard))
                        };
                    return EventParacel.Reject;
                }
            });
        }

        private EventParacel OnRecvResponse(SsoMessage ssoMessage)
        {
            return EventParacel.Accept;
        }
    }
}
