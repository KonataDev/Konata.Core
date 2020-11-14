using System;

namespace Konata.Packets.Tlv
{
    public class T305Body : TlvBody
    {
        public readonly byte[] _d2Key;

        public T305Body(byte[] d2tKey, object nil)
            : base()
        {
            _d2Key = d2tKey;

            PutBytes(_d2Key);
        }

        public T305Body(byte[] data)
            : base(data)
        {
            TakeBytes(out _d2Key, Prefix.None);
        }
    }
}
