using System;
using Konata.Events;

namespace Konata.Services.MessageSvc
{
    public class PbSendMsg : ServiceRoutine
    {
        public PbSendMsg(EventPumper eventPumper)
            : base("MessageSvc.PbSendMsg", eventPumper)
        {

        }

        public override EventParacel OnEvent(EventParacel eventParacel)
        {
            return EventParacel.Reject;
        }
    }
}
