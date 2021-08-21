using System;

namespace Konata.Core.Packets.Tlv.Model
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
