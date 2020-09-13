using Konata.Utils;

namespace Konata.Msf.Packets.Tlvs
{
    public class T108 : TlvBase
    {
        private readonly byte[] _ksid;

        public T108(byte[] ksid)
        {
            _ksid = ksid;
        }

        public override void PutTlvCmd()
        {
            PutUshortBE(0x108);
        }

        public override void PutTlvBody()
        {
            PutBytes(_ksid);
        }
    }
}
