using System;
using Konata.Events;
using Konata.Packets;
using Konata.Packets.Sso;
using Konata.Packets.Protobuf;

namespace Konata.Services.MessageSvc
{
    public class PbGetMsg : ServiceRoutine
    {
        public PbGetMsg(EventPumper eventPumper)
            : base("MessageSvc.PbGetMsg", eventPumper)
        {

        }

        protected override EventParacel OnEvent(EventParacel eventParacel)
        {
            //if (eventParacel is EventAccountCtl accountCtl
            //    && accountCtl.Type == EventAccountCtl.EventType.GetTroopList)
            //    return OnSendRequest(accountCtl.SelfUin);
            //else if (eventParacel is EventSsoMessage ssoMsgEvent)
            //    return OnRecvResponse(ssoMsgEvent.PayloadMsg);

            return EventParacel.Reject;
        }

        private EventParacel OnSendRequest()
        {
            //var ssoSeq = core.SsoMan.GetNewSequence();
            //var ssoSession = core.SsoMan.GetSsoSession();

            //var ssoMessage = new SsoMessageTypeB(ssoSeq, name, ssoSession,
            //    new ProtoGetMsg(core.SigInfo.SyncCookie).Serialize());

            //return core.SsoMan.PostMessage(RequestFlag.D2Authentication,
            //    ssoMessage, core.SigInfo.D2Token, core.SigInfo.D2Key);
            return EventParacel.Reject;
        }

        private EventParacel OnRecvResponse()
        {
            return EventParacel.Reject;
        }


    }
}
