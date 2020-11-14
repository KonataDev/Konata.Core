using System;

namespace Konata.Packets.Tlv
{
    public class T10aBody : TlvBody
    {
        public readonly byte[] _tgtToken;

        public T10aBody(byte[] tgtToken, object nil)
            : base()
        {
            _tgtToken = tgtToken;

            PutBytes(_tgtToken);
        }

        public T10aBody(byte[] data)
            : base(data)
        {
            TakeBytes(out _tgtToken, Prefix.None);
        }
    }
}
