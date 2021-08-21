using System;

namespace Konata.Core.Packets.Tlv.Model
{
    public class T318Body : TlvBody
    {
        public readonly byte[] _tgtQr;

        public T318Body(byte[] tgtQr, int tgtQrLength)
            : base()
        {
            _tgtQr = tgtQr;

            PutBytes(_tgtQr);
        }

        public T318Body(byte[] data)
            : base(data)
        {
            TakeBytes(out _tgtQr, Prefix.None);
        }
    }
}