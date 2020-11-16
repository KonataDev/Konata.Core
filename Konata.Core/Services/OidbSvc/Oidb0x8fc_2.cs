using System;
using Konata.Events;
using Konata.Packets;
using Konata.Packets.Sso;
using Konata.Packets.Oidb;

namespace Konata.Services.OidbSvc
{
    public class Oidb0x8fc_2 : ServiceRoutine
    {
        public Oidb0x8fc_2(EventPumper eventPumper)
            : base("OidbSvc.0x8fc_2", eventPumper)
        {

        }

        protected override EventParacel OnEvent(EventParacel eventParacel)
        {
            if (eventParacel is EventGroupCtl groupCtl
                && groupCtl.Type == EventGroupCtl.EventType.SetSpecialTitle)
                return OnSendRequest(groupCtl.GroupUin, groupCtl.MemberUin,
                    groupCtl.SpecialTitle, groupCtl.TimeSeconds);
            else if (eventParacel is EventSsoMessage ssoEvent)
                return OnRecvResponse(ssoEvent.PayloadMsg);

            return EventParacel.Reject;
        }

        private EventParacel OnSendRequest(uint groupUin, uint memberUin,
            string specialTitle, uint? expiredTime)
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
                                new OidbCmd0x8fc_2(groupUin, memberUin, specialTitle, expiredTime))
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
