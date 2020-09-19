using System;
using Konata.Utils;

namespace Konata.Msf.Packets.Tlv
{
    public class T193 : TlvBase
    {
        private readonly string _ticket;

        public T193(string ticket) : base()
        {
            _ticket = ticket;

            PackGeneric();
        }

        public override void PutTlvCmd()
        {
            PutUshortBE(0x0193);
        }

        public override void PutTlvBody()
        {
            PutString(_ticket, 2);
        }
    }
}
