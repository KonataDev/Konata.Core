using System;

namespace Konata.Core.Packets.Tlv.Model
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
