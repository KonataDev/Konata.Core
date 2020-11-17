using System;
using Konata.Events;

namespace Konata.Services.MessageSvc
{
    public class PushForceOffline : ServiceRoutine
    {
        public PushForceOffline(EventPumper eventPumper)
            : base("MessageSvc.PushForceOffline", eventPumper)
        {

        }

        public override EventParacel OnEvent(EventParacel eventParacel)
        {
            return EventParacel.Reject;
        }
    }
}
