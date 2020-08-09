using Konata.Utils;
using Konata.Utils.Crypto;

namespace Konata.Protocol.Packet.Tlvs
{
    public class T188 : TlvBase
    {
        private readonly byte[] _androidId;

        public T188(byte[] androidId)
        {
            _androidId = androidId;
        }

        public override ushort GetTlvCmd()
        {
            return 0x188;
        }

        public override byte[] GetTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            builder.PushBytes(new Md5Cryptor().Encrypt(_androidId));
            return builder.GetPlainBytes();
        }
    }
}
