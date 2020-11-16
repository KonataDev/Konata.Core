using System;
using Konata.Events;
using Konata.Packets;
using Konata.Packets.Sso;
using Konata.Packets.Oidb;

namespace Konata.Services.OidbSvc
{
    public class Oidb0x8a0_0 : ServiceRoutine
    {
        public Oidb0x8a0_0(EventPumper eventPumper)
            : base("OidbSvc.0x8a0_0", eventPumper)
        {

        }

        protected override EventParacel OnEvent(EventParacel eventParacel)
        {
            if (eventParacel is EventGroupCtl groupCtl
                && groupCtl.Type == EventGroupCtl.EventType.KickMember)
                return OnSendRequest(groupCtl.GroupUin, groupCtl.MembersUin, groupCtl.ToggleType);
            else if (eventParacel is EventSsoMessage ssoEvent)
                return OnRecvResponse(ssoEvent.PayloadMsg);

            return EventParacel.Reject;
        }

        private EventParacel OnSendRequest(uint groupUin,
            uint[] memberUin, bool preventRequest)
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
                                new OidbCmd0x8a0_0(groupUin, memberUin, preventRequest))
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
