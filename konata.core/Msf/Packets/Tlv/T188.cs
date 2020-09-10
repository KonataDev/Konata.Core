using Konata.Utils;
using Konata.Msf.Utils.Crypt;

namespace Konata.Msf.Packets.Tlvs
{
    public class T188 : TlvBase
    {
        private readonly byte[] _androidId;

        public T188(byte[] androidId)
        {
            _androidId = androidId;
        }

        public T188(string androidId)
        {
            _androidId = Hex.HexStr2Bytes(androidId);
        }

        public override ushort GetTlvCmd()
        {
            return 0x188;
        }

        public override byte[] GetTlvBody()
        {
            StreamBuilder builder = new StreamBuilder();
            builder.PushBytes(new Md5Cryptor().Encrypt(_androidId));
            return builder.GetBytes();
        }
    }
}
