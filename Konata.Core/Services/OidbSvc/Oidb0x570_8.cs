using System;
using Konata.Packets;
using Konata.Packets.Sso;
using Konata.Packets.Oidb;
using Konata.Events;

namespace Konata.Services.OidbSvc
{
    public class Oidb0x570_8 : ServiceRoutine
    {
        public Oidb0x570_8(EventPumper eventPumper)
            : base("OidbSvc.0x570_8", eventPumper)
        {

        }

        protected override EventParacel OnEvent(EventParacel eventParacel)
        {
            if (eventParacel is EventGroupCtl groupCtl
                && groupCtl.Type == EventGroupCtl.EventType.MuteMember)
                return OnSendRequest(groupCtl.GroupUin, groupCtl.MemberUin,
                    groupCtl.TimeSeconds ?? uint.MaxValue);
            else if (eventParacel is EventSsoMessage ssoEvent)
                return OnRecvResponse(ssoEvent.PayloadMsg);

            return EventParacel.Reject;
        }

        private EventParacel OnSendRequest(uint groupUin,
            uint memberUin, uint timeSeconds)
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
                                new OidbCmd0x570_8(groupUin, memberUin, timeSeconds))
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
