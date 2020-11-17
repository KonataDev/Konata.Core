using System;
using Konata.Events;

namespace Konata
{
    //   Mobileqq Service Framework

    //  ++----> SsoMan <---> PacketMan
    //  ||         +-------+
    //  ||                 |    
    //  ||	 +-> Msf.Core  |   
    //  ||	 |      |      |   
    //  ||   ++  Service <-+
    //  ||	  |   /    \
    //  ++--- WtLogin OnlinePush...etc 
    //   +----------------+

    [Obsolete]
    public class Core : EventComponent
    {
        public SsoMan SsoMan { get; private set; }

        public SigInfoMan SigInfo { get; private set; }

        public Core(EventPumper eventPumper)
            : base(eventPumper)
        {

        }
    }

    public class EventGroupCtlRsp : EventParacel
    {
        public bool Success { get; set; }

        public int ResultCode { get; set; }
    }
}
