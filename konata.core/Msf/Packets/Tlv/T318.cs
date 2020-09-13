using Konata.Utils;

namespace Konata.Msf.Packets.Tlv
{
    public class T318 : TlvBase
    {
        private readonly byte[] _tgtQr;

        public T318(byte[] tgtQr)
        {
            _tgtQr = tgtQr;
        }

        public override void PutTlvCmd()
        {
            PutUshortBE(0x318);
        }

        public override void PutTlvBody()
        {
            PutBytes(_tgtQr, 2);
        }
    }
}
