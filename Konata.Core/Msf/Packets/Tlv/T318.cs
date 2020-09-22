using System;

namespace Konata.Msf.Packets.Tlv
{
    public class T318 : TlvBase
    {
        public T318(byte[] tgtQr)
            : base(0x0318, new T318Body(tgtQr, tgtQr.Length))
        {

        }
    }

    public class T318Body : TlvBody
    {
        public readonly byte[] _tgtQr;

        public T318Body(byte[] tgtQr, int tgtQrLength)
            : base()
        {
            _tgtQr = tgtQr;

            PutBytes(_tgtQr, 2);
        }

        public T318Body(byte[] data)
            : base(data)
        {
            TakeBytes(out _tgtQr, Prefix.Uint16);
        }
    }
}
