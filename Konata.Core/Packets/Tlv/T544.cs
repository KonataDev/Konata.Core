using System;

namespace Konata.Packets.Tlv
{
    public class T544Body : TlvBody
    {
        public readonly string _wtLoginSdk;

        public T544Body(string wtLoginSdk)
            : base()
        {
            _wtLoginSdk = wtLoginSdk;
        }

        public T544Body(byte[] data)
            : base(data)
        {
            EatBytes(RemainLength);
        }
    }
}
