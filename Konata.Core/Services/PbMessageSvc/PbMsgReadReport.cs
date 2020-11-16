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

        protected override EventParacel OnEvent(EventParacel eventParacel)
        {
            return EventParacel.Reject;
        }
    }
}
