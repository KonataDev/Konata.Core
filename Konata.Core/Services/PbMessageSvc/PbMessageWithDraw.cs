using System;
using Konata.Events;

namespace Konata.Services.PbMessageSvc
{
    public class PbMessageWithDraw : ServiceRoutine
    {
        public PbMessageWithDraw(EventPumper eventPumper)
            : base("PbMessageSvc.PbMessageWithDraw", eventPumper)
        {

        }

        public override EventParacel OnEvent(EventParacel eventParacel)
        {
            return EventParacel.Reject;
        }
    }
}
