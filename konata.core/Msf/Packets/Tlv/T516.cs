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

        public override ushort GetTlvCmd()
        {
            return 0x516;
        }

        public override byte[] GetTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            builder.PutUintBE(_sourceType);
            return builder.GetBytes();
        }
    }
}
