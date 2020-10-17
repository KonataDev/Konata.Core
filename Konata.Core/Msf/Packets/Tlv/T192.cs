using System;

namespace Konata.Msf.Packets.Tlv
{
    public class T192Body : TlvBody
    {
        public readonly string _url;

        public T192Body(string url)
            : base()
        {
            _url = url;

            PutString(_url, Prefix.Uint16);
        }

        public T192Body(byte[] data)
            : base(data)
        {
            TakeString(out _url, Prefix.None);
        }
    }
}
