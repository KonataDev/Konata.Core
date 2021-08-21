using System;

namespace Konata.Core.Packets.Tlv.Model
{
    public class T10dBody : TlvBody
    {
        public readonly byte[] _tgtKey;

        public T10dBody(byte[] tgtKey, object nil)
            : base()
        {
            _tgtKey = tgtKey;

            PutBytes(_tgtKey);
        }

        public T10dBody(byte[] data)
            : base(data)
        {
            TakeBytes(out _tgtKey, Prefix.None);
        }
    }
}
