using System;
using Konata.Events;

namespace Konata.Services.Heartbeat
{
    public class Alive : ServiceRoutine
    {
        public Alive(EventPumper eventPumper)
            : base("Heartbeat.Alive", eventPumper)
        {

        }

        public override EventParacel OnEvent(EventParacel eventParacel)
        {
            return EventParacel.Reject;
        }
    }
}
