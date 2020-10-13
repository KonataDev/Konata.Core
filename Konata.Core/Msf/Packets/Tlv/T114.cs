using System;

namespace Konata.Msf.Packets.Tlv
{
    public class T114Body : TlvBody
    {
        public readonly byte[] _st;

        public T114Body(byte[] st, object nil)
            : base()
        {
            _st = st;

            PutBytes(_st);
        }

        public T114Body(byte[] data)
            : base(data)
        {
            TakeBytes(out _st, Prefix.None);
        }
    }
}
