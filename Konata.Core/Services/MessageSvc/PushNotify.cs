using System;
using Konata.Events;
using Konata.Packets.Wup;
using Konata.Library.IO;

namespace Konata.Services.MessageSvc
{
    public class PushNotify : ServiceRoutine
    {
        public PushNotify(EventPumper eventPumper)
            : base("MessageSvc.PushNotify", eventPumper)
        {

        }

        public override EventParacel OnEvent(EventParacel eventParacel)
        {
            return EventParacel.Reject;
        }
    }
}
