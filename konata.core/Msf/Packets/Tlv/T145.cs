using Konata.Utils;

namespace Konata.Msf.Packets.Tlv
{
    public class T145 : TlvBase
    {
        private readonly byte[] _guid;

        public T145(byte[] guid)
        {
            _guid = guid;
        }

        public override void PutTlvCmd()
        {
            PutUshortBE(0x145);
        }

        public override void PutTlvBody()
        {
            PutBytes(_guid);
        }
    }
}
