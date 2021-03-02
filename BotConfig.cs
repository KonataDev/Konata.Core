using System;

namespace Konata.Core
{
    public class BotConfig
    {
        //public bool UseIPv6Connection { get; set; }

        public bool ReConnectWhileLinkDown { get; set; }

        public uint ReConnectTryCount { get; set; }
    }
}
