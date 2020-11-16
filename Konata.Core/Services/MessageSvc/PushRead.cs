using System;
using Konata.Events;

namespace Konata.Services.MessageSvc
{
    public class PushRead : ServiceRoutine
    {
        public PushRead(EventPumper eventPumper)
            : base("MessageSvc.PushReaded", eventPumper)
        {

        }

        protected override EventParacel OnEvent(EventParacel eventParacel)
        {
            return EventParacel.Reject;
        }
    }
}
