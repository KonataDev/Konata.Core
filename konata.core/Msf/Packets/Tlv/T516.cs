using Konata.Utils;

namespace Konata.Msf.Packets.Tlv
{
    public class T516 : TlvBase
    {
        private readonly uint _sourceType;

        public T516(uint sourceType = 0)
        {
            _sourceType = sourceType;
        }

        public override void PutTlvCmd()
        {
            PutUshortBE(0x516);
        }

        public override void PutTlvBody()
        {
            PutUintBE(_sourceType);
        }
    }
}
