using System;
using Konata.Packets.Oicq;

namespace Konata.Events
{
    public class EventUpdateSigInfo : EventParacel
    {
        public byte[] TgtKey { get; set; }

        public byte[] TgtToken { get; set; }

        public byte[] D2Key { get; set; }

        public byte[] D2Token { get; set; }

        public byte[] GtKey { get; set; }

        public byte[] StKey { get; set; }

        public byte[] WtSessionTicketSig { get; set; }

        public byte[] WtSessionTicketKey { get; set; }

        public class Info
        {
            public uint Age { get; set; }

            public uint Face { get; set; }

            public string Name { get; set; }
        }

        public Info UinInfo { get; set; }

        public string WtLoginSession { get; set; }

        public string WtLoginSmsToken { get; set; }

        public string WtLoginSmsPhone { get; set; }

        public string WtLoginSmsCountry { get; set; }

        public OicqStatus WtLoginOicqStatus { get; set; }
    }
}
