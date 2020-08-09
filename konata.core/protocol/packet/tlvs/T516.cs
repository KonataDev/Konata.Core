using Konata.Utils;

namespace Konata.Protocol.Packet.Tlvs
{    
    public class T516 : TlvBase
    {
        private readonly int _sourceType;

        public T516(int sourceType = 0)
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
            builder.PushInt32(_sourceType);
            return builder.GetPlainBytes();
        }
    }
}
