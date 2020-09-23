using System;

namespace Konata.Msf.Packets.Tlv
{
    public class T193Body : TlvBody
    {
        public readonly string _ticket;

        public T193Body(string ticket)
            : base()
        {
            _ticket = ticket;

            PutString(_ticket, 2);
        }

        public T193Body(byte[] data)
            : base(data)
        {
            TakeString(out _ticket, Prefix.Uint16);
        }
    }
}
