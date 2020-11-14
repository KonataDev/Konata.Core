using System;

namespace Konata.Packets.Tlv
{
    public class T193Body : TlvBody
    {
        public readonly string _ticket;

        public T193Body(string ticket)
            : base()
        {
            _ticket = ticket;

            PutString(_ticket);
        }

        public T193Body(byte[] data)
            : base(data)
        {
            TakeString(out _ticket, Prefix.None);
        }
    }
}
