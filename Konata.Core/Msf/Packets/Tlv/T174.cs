using System;

namespace Konata.Msf.Packets.Tlv
{
    public class T174Body : TlvBody
    {
        public readonly byte[] _data;

        public T174Body(byte[] data, object nil)
            : base()
        {
            _data = data;

            PutBytes(_data);
        }

        public T174Body(byte[] data)
            : base(data)
        {
            TakeBytes(out _data, Prefix.None);
        }
    }
}
