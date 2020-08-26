using Konata.Utils;

namespace Konata.Protocol.Packet.Tlvs
{
    public class T154 : TlvBase
    {
        private readonly int _ssoSequenceId;

        public T154(int ssoSequenceId)
        {
            _ssoSequenceId = ssoSequenceId;
        }

        public override ushort GetTlvCmd()
        {
            return 0x154;
        }

        public override byte[] GetTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            builder.PushInt32(_ssoSequenceId);
            return builder.GetBytes();
        }
    }
}
