using System;
using Konata.Events;

namespace Konata.Services.MessageSvc
{
    public class RequestPushStatus : ServiceRoutine
    {
        public RequestPushStatus(EventPumper eventPumper)
            : base("MessageSvc.RequestPushStatus", eventPumper)
        {

        }

        protected override EventParacel OnEvent(EventParacel eventParacel)
        {
            return EventParacel.Reject;
        }
    }
}
