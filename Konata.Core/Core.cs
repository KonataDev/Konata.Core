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

        #region Core Methods

        /// <summary>
        /// 連接到伺服器
        /// </summary>
        /// <returns></returns>
        public bool Connect() =>
            SsoMan.Connect();

        /// <summary>
        /// 斷開連接
        /// </summary>
        /// <returns></returns>
        public bool DisConnect() =>
            SsoMan.DisConnect();

        #endregion
    }

    public class EventGroupCtlRsp : EventParacel
    {
        public bool Success { get; set; }

        public int ResultCode { get; set; }
    }
}
