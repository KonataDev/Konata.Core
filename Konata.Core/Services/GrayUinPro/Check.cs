using System;
using Konata.Events;

namespace Konata.Services.GrayUinPro
{
    public class Check : ServiceRoutine
    {
        public Check(EventPumper eventPumper)
            : base("GrayUinPro.Check", eventPumper)
        {

        }

        public override EventParacel OnEvent(EventParacel eventParacel)
        {
            return EventParacel.Reject;
        }
    }
}
