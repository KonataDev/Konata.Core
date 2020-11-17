using System;
using Konata.Events;

namespace Konata.Services.OnlinePush
{
    public class PbPushGroupMsg : ServiceRoutine
    {
        public PbPushGroupMsg(EventPumper eventPumper)
            : base("OnlinePush.PbPushGroupMsg", eventPumper)
        {

        }

        public override EventParacel OnEvent(EventParacel eventParacel)
        {
            return EventParacel.Reject;
        }
    }
}
