using Konata.Utils;

namespace Konata.Msf.Packets.Tlvs
{
    public class T145 : TlvBase
    {
        private readonly byte[] _guid;

        public T145(byte[] guid)
        {
            _guid = guid;
        }

        public override ushort GetTlvCmd()
        {
            return 0x145;
        }

        public override byte[] GetTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            builder.PushBytes(_guid, false);
            return builder.GetBytes();
        }
    }
}
