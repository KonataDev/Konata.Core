using Konata.Utils;

namespace Konata.Msf.Packets.Tlv
{
    public class T108 : TlvBase
    {
        private readonly byte[] _ksid;

        public T108(byte[] ksid) : base()
        {
            _ksid = ksid;

            PackGeneric();
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
