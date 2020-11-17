using System;
using Konata.Events;

namespace Konata.Services.PbMessageSvc
{
    public class PbMsgReadReport : ServiceRoutine
    {
        public PbMsgReadReport(EventPumper eventPumper)
            : base("PbMessageSvc.PbMsgReadedReport", eventPumper)
        {

        }

        public override EventParacel OnEvent(EventParacel eventParacel)
        {
            return EventParacel.Reject;
        }
    }
}
