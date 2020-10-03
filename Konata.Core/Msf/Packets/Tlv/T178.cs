using System;

namespace Konata.Msf.Packets.Tlv
{
    public class T178Body : TlvBody
    {
        public readonly string _phone;

        public T178Body(string phone)
            : base()
        {
            _phone = phone;

            PutString(phone);
        }

        public T178Body(byte[] data)
            : base(data)
        {
            TakeString(out _phone, Prefix.None);
        }
    }
}
