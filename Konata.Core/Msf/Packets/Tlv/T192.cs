using System;

namespace Konata.Msf.Packets.Tlv
{
    public class T192 : TlvBase
    {
        public T192(string url)
            : base(0x0192, new T192Body(url))
        {

        }
    }

    public class T192Body : TlvBody
    {
        private readonly string _url;

        public T192Body(string url)
            : base()
        {
            _url = url;

            PutString(_url, 2);
        }

        public T192Body(byte[] data)
            : base(data)
        {
            TakeString(out _url, Prefix.Uint16);
        }
    }
}
