using System;

namespace Konata.Model.Packet.Tlv.TlvModel
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
