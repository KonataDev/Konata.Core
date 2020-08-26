using System;
using Konata.Utils;

namespace Konata.Protocol.Packet.Tlvs
{
    public class T193 : TlvBase
    {
        private readonly string _ticket;

        public T193(string ticket)
        {
            _ticket = ticket;
        }

        public override ushort GetTlvCmd()
        {
            return 0x0193;
        }

        public override byte[] GetTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            builder.PushString(_ticket, false);
            return builder.GetBytes();
        }
    }
}