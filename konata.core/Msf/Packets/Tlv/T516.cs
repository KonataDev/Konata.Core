using Konata.Utils;

namespace Konata.Msf.Packets.Tlvs
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
            return 0x516;
        }

        public override void PutTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            builder.PutUintBE(_sourceType);
            return builder.GetBytes();
        }
    }
}
