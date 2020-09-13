using Konata.Utils;

namespace Konata.Msf.Packets.Tlvs
{
    public class T191 : TlvBase
    {
        private readonly int _unknownK;

        public T191(int unknownK = 0x82)
        {
            _unknownK = unknownK;
        }

        public override void PutTlvCmd()
        {
            PutUshortBE(0x191);
        }

        public override void PutTlvBody()
        {
            PutByte((byte)_unknownK);
        }
    }
}
