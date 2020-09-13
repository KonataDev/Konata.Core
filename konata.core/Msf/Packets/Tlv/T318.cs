using Konata.Utils;

namespace Konata.Msf.Packets.Tlvs
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
            return 0x318;
        }

        public override void PutTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            builder.PutBytes(_tgtQr, 2);
            return builder.GetBytes();
        }
    }
}
