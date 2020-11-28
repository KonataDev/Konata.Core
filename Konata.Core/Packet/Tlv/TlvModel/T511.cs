using System;

namespace Konata.Model.Packet.Tlv.TlvModel
{
    public class T511Body : TlvBody
    {
        public readonly string[] _domains;

        public T511Body(string[] domains)
            : base()
        {
            _domains = domains;

            PutUshortBE((ushort)_domains.Length);
            {
                foreach (string element in _domains)
                {
                    PutByte(0x01);
                    PutString(element, Prefix.Uint16);
                }
            }
        }

        public T511Body(byte[] data)
           : base(data)
        {
            TakeUshortBE(out var length);
            
            _domains = new string[length];
            {
                for (int i = 0; i < length; ++i)
                {
                    EatBytes(1);
                    TakeString(out _domains[i], Prefix.Uint16);
                }
            }
        }
    }
}
