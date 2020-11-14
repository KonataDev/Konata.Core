using System;

namespace Konata.Packets.Tlv
{
    public class T401Body : TlvBody
    {
        public readonly byte[] _gMd5;

        public T401Body(byte[] gMd5, object nil)
            : base()
        {
            _gMd5 = gMd5;

            PutBytes(_gMd5);
        }

        public T401Body(byte[] data)
            : base(data)
        {
            TakeBytes(out _gMd5, Prefix.None);
        }
    }
}
