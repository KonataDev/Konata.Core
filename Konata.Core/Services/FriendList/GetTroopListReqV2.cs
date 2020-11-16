using System;
using Konata.Events;
using Konata.Packets;
using Konata.Packets.Sso;
using Konata.Packets.SvcRequest;

namespace Konata.Services.FriendList
{
    public class GetTroopListReqV2 : ServiceRoutine
    {
        public GetTroopListReqV2(EventPumper eventPumper)
            : base("friendlist.GetTroopListReqV2", eventPumper)
        {

        }

        protected override EventParacel OnEvent(EventParacel eventParacel)
        {
            if (eventParacel is EventAccountCtl accountCtl
                && accountCtl.Type == EventAccountCtl.EventType.GetTroopList)
                return OnSendRequest(accountCtl.SelfUin);
            else if (eventParacel is EventSsoMessage ssoMsgEvent)
                return OnRecvResponse(ssoMsgEvent.PayloadMsg);

            return EventParacel.Reject;
        }

        private EventParacel OnSendRequest(uint selfUin)
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
                                new SvcReqGetTroopListReqV2Simplify(selfUin))
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
