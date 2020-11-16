using System;
using Konata.Events;

namespace Konata.Services.Wtlogin
{
    public class Trans_emp : ServiceRoutine
    {
        public Trans_emp(EventPumper eventPumper)
            : base("wtlogin.trans_emp", eventPumper)
        {

        }

        protected override EventParacel OnEvent(EventParacel eventParacel)
        {
            return EventParacel.Reject;
        }
    }
}
