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

        public override ushort GetTlvCmd()
        {
            return 0x318;
        }

        public override byte[] GetTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            builder.PutBytes(_tgtQr, false);
            return builder.GetBytes();
        }
    }
}
