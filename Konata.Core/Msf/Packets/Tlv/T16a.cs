using System;

namespace Konata.Msf.Packets.Tlv
{
    public class T16aBody : TlvBody
    {
        public readonly byte[] _noPicSig;

        public T16aBody(byte[] noPicSig, object nil)
            : base()
        {
            _noPicSig = noPicSig;

            PutBytes(_noPicSig);
        }

        public T16aBody(byte[] data)
            : base(data)
        {
            TakeBytes(out _noPicSig, Prefix.None);
        }
    }
}
