using System;

namespace Konata.Model.Packet.Tlv.TlvModel
{
    public class T10cBody : TlvBody
    {
        public readonly byte[] _gtKey;

        public T10cBody(byte[] gtKey, object nil)
            : base()
        {
            _gtKey = gtKey;

            PutBytes(_gtKey);
        }

        public T10cBody(byte[] data)
            : base(data)
        {
            TakeBytes(out _gtKey, Prefix.None);
        }
    }
}
